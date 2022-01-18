using Godot;
using System;
using Game.Systems.Interaction;
using Game.Scenes.UI;

namespace Game.Entities
{
    public class Player : KinematicBody
    {
        // - - Player stats - - 

        //--Physics stats
        public float MoveSpeed { get; private set; } = 25;
        public float SprintBoost { get; private set; } = 10;
        public float JumpHeight { get; private set; } = 5;
        private float Acceleration { get; set; } = 20;

        //--Physics state of player in current frame
        public Vector3 TickVelocity { get; private set; } = Vector3.Zero;
        private bool TickOnFloor { get; set; } //Is the player on the ground in the current physics tick
        public bool TickSprinting { get; private set; } //Is the player sprinting in the current physics tick
        public bool TickMoving { get; private set; } //Is the player moving in the current physics tick

        //--Control values
        public float CameraSensitivity { get; private set; } = 0.1f;
        public float CameraFOV { 
            get { return FPCamera.Fov; } 
            private set { FPCamera.Fov = value; } 
        }

        //--Interactivity values
        public float ObjectThrowForce { get; private set; } = 120;
        public float ObjectHoldDistance { get; private set; } = 3;
        public bool HoldingItem { get { return HeldItemInterface != null; } }
        private RigidBody HeldItemBody { get; set; } = null;
        public IPickable HeldItemInterface { get; private set; } = null;

        //--Nodes
        private Camera FPCamera { get; set; }
        private Spatial HeadSpatial { get; set; }
        private Label TestFPSLabel { get; set; }
        private RayCast InteractRayCast { get; set; }
        private Position3D HeldItemPosition { get; set; }

        //--UIs
        private Control FirstPersonUI { get; set; }
        private SettingsUI SettingsUI { get; set; }

        // - - - GD Methods - - -
        public override void _Ready()
        {
            base._Ready();

            FPCamera = GetNode<Camera>("Head/Camera");
            HeadSpatial = GetNode<Spatial>("Head");
            TestFPSLabel = GetNode<Label>("Head/Camera/Direction/FirstPersonUI/FPSLabel");
            InteractRayCast = GetNode<RayCast>("Head/InteractCast");
            HeldItemPosition = GetNode<Position3D>("Head/HeldItemPos");

            FirstPersonUI = GetNode<Control>("Head/Camera/Direction/FirstPersonUI");
            SettingsUI = GetNode<SettingsUI>("Head/Camera/Direction/SettingsUI");

            Input.SetMouseMode(Input.MouseMode.Captured);
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            //Print data to the testing label
            var tSpeed = ((int)TickVelocity.Length());
            TestFPSLabel.Text = $"FPS: { Engine.GetFramesPerSecond() }\nSpeed: {tSpeed}";
            if(HoldingItem) { TestFPSLabel.Text += $"\nObjDist: { GetHeldObjectDistance() }"; }   
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            ProcessMovement(delta);

            if(HoldingItem)
            {
                Vector3 carryPos = FPCamera.GlobalTransform.origin + (-FPCamera.GlobalTransform.basis.z.Normalized() * ObjectHoldDistance);
                Vector3 moveDirection = carryPos - HeldItemBody.GlobalTransform.origin;
                HeldItemBody.AddCentralForce(moveDirection * 800);
                //Rotation
                HeldItemBody.AddTorque((Rotation - HeldItemBody.Rotation) * 10);
                //Drop the object if it is stuck too far away
                if(GetHeldObjectDistance() > 4.5) { DropObject(); }
            }
        }

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);

            //Capture mouse input for the camera
            if (@event is InputEventMouseMotion && Input.GetMouseMode() == Input.MouseMode.Captured)
            {
                ProcessRotation(@event as InputEventMouseMotion);
            }
            //Gameplay input
            if (@event.IsActionPressed("interact_grab")) { ProcessInteract(); }
            if (@event.IsActionPressed("combat_fireweapon"))
            {
                if (HoldingItem) { ThrowObject(); }
                else
                { //Shoot weapon 
                }
            }
        }

        //Use the unhandled input method for anything that should be consumed by the GUI first
        public override void _UnhandledKeyInput(InputEventKey @event)
        {
            base._UnhandledKeyInput(@event);

            //UI Input
            if (@event.IsActionPressed("ui_cancel")) { PauseAndShowSettings(); } 
        }

        // - - - UI funtionality & pausing - - -
        public void ApplySettings(SettingsUI sett, bool newChanges, bool unpause)
        {
            if(newChanges)
            {
                CameraFOV = sett.FOVSetting;
                CameraSensitivity = sett.SensitivitySetting;
                MoveSpeed = sett.PlayerMoveSpeedSetting;
                JumpHeight = sett.PlayerJumpHeightSetting;
                ObjectHoldDistance = sett.ObjectHoldDistanceSetting;
            }

            if (unpause) { UnpauseAndHideSettings(); }          
        }

        private void PauseAndShowSettings()
        {
            SettingsUI.ImportSettings(this);
            SettingsUI.Visible = true;
            FirstPersonUI.Visible = false;
            Input.SetMouseMode(Input.MouseMode.Visible);
            GetTree().Paused = true;
        }

        private void UnpauseAndHideSettings()
        {
            SettingsUI.Visible = false;
            FirstPersonUI.Visible = true;
            Input.SetMouseMode(Input.MouseMode.Captured);
            GetTree().Paused = false;
        }

        // - - - Movement and FP Controller - - - 
        private void ProcessMovement(float delta)
        {
            //Local variables for movement
            Vector3 tickDirection = Vector3.Zero;
            float vertVelocity = TickVelocity.y; //Retain vertical velocity between ticks
            float totalSpeed;
            //Reset tick-based stats
            TickMoving = false;
            TickSprinting = false;
            TickVelocity = Vector3.Zero;

            //Gather a 2d vector of movement
            if (Input.IsActionPressed("movement_forward")) { tickDirection -= Transform.basis.z; }
            if (Input.IsActionPressed("movement_left")) { tickDirection -= Transform.basis.x; }
            if (Input.IsActionPressed("movement_right")) { tickDirection += Transform.basis.x; }
            if (Input.IsActionPressed("movement_backward")) { tickDirection += Transform.basis.z; }
            if (Input.IsActionPressed("movement_sprint")) { TickSprinting = true; }

            //Process Jumping & Gravity
            vertVelocity -= GameGlobals.GLOBALGRAVITY * delta;
            if(TickOnFloor && Input.IsActionJustPressed("movement_jump")) { vertVelocity += JumpHeight; }

            //Check if player is moving in this tick
            if (tickDirection != Vector3.Zero || vertVelocity != 0)
            {
                TickMoving = true;
                //Apply sprinting boost if spring key is pressed
                totalSpeed =  TickSprinting ? MoveSpeed + SprintBoost : MoveSpeed;               
                //Normalize the Direction Vector (so diagonal movement doesnt cause 2x speed)
                tickDirection = tickDirection.Normalized();
                //Interpolate acceleration and apply M&S
                TickVelocity = TickVelocity.LinearInterpolate(tickDirection * totalSpeed, Acceleration * delta);
                TickVelocity = new Vector3(TickVelocity.x, vertVelocity, TickVelocity.z);
                TickVelocity = MoveAndSlide(TickVelocity, Vector3.Up);
            }

            TickOnFloor = IsOnFloor(); //Always call IsOnFloor() after moveand* is called
        }

        private void ProcessRotation(InputEventMouseMotion mouseMov)
        {
            RotateY(Mathf.Deg2Rad(-mouseMov.Relative.x * CameraSensitivity));
            HeadSpatial.RotateX(Mathf.Deg2Rad(-mouseMov.Relative.y * CameraSensitivity));
            //Clamp the x (up and down) rotation on the head so there cannot be any roll-over
            Vector3 headRotation = HeadSpatial.RotationDegrees;
            headRotation.x = Mathf.Clamp(headRotation.x, -70, 70);
            HeadSpatial.RotationDegrees = headRotation;
        }

        // - - - Interactivity - - - 
        private void ProcessInteract()
        {
            //If an item is being held, and we are allowed to drop it, drop item
            if (HoldingItem && 
                HeldItemInterface.RequestDrop(this)) 
            { 
                DropObject();
                return; 
            }

            var collider = InteractRayCast.GetCollider();
            if(collider != null)
            {
                if(collider is IInteractable) 
                {
                    var interactable = collider as IInteractable;
                    interactable.Interact(this);
                }
                else if(collider is IPickable)
                {
                    var pickable = collider as IPickable;
                    RigidBody body = null;
                    if (pickable.RequestPickup(out body, this))
                    {
                        
                        GrabObject(body, pickable); 
                    }
                    else 
                    {
                        //Maybe a sound effect could play here ah-la half life?
                        GD.Print("Request to pickup object was denied.");
                    }
                }
            }
        }

        private void GrabObject(RigidBody body, IPickable pick)
        {
            //Logic
            HeldItemBody = body;
            HeldItemInterface = pick;
            //Physics
            HeldItemBody.LinearDamp = 35;
            HeldItemBody.AngularDamp = 40;
        }

        private void ThrowObject() //TODO: adding impulse should be done by this script, not the physics objects script
        {
            if (HeldItemInterface.RequestThrow(this))
            {
                var impulse = -FPCamera.GlobalTransform.basis.z.Normalized() * ObjectThrowForce;
                HeldItemBody.ApplyImpulse(Vector3.Zero, impulse);
                DropObject();
            }
            else
            {
                GD.Print("Could not throw object, denied.");
            }
        }

        private void DropObject()
        {
            //Physics 
            HeldItemBody.LinearDamp = -1;
            HeldItemBody.AngularDamp = -1;
            HeldItemBody.AddCentralForce(-HeldItemBody.LinearVelocity * 35);
            //Logic 
            HeldItemBody = null;
            HeldItemInterface = null;
        }

        private float GetHeldObjectDistance()
        {
            if (!HoldingItem) { return 0; }
            Vector3 carryPos = FPCamera.GlobalTransform.origin + (-FPCamera.GlobalTransform.basis.z.Normalized() * ObjectHoldDistance);
            return HeldItemBody.GlobalTransform.origin.DistanceTo(carryPos);
        }
    }   
}