using Godot;
using System;
using Game.Systems.Interaction;

namespace Game.Entities
{
    public class Player : KinematicBody
    {
        // - - Player stats - - 

        //Physics stats
        [Export] public float MoveSpeed { get; private set; } = 25;
        [Export] public float SprintBoost { get; private set; } = 10;
        [Export] public float JumpHeight { get; private set; } = 5;
        private float Acceleration { get; set; } = 20;

        //Physics state of player in current frame
        public Vector3 TickVelocity { get; private set; } = Vector3.Zero;
        private bool TickOnFloor { get; set; } //Is the player on the ground in the current physics tick
        public bool TickSprinting { get; private set; } //Is the player sprinting in the current physics tick
        public bool TickMoving { get; private set; } //Is the player moving in the current physics tick

        //Control values
        public float CameraSpeed { get; set; } = 0.1f;

        //Interactivity values
        [Export] public float ObjectThrowForce { get; private set; } = 120;
        private float ObjectGrabDistance { get; set; } = 5;
        public bool HoldingItem { get { return HeldItemInterface != null; } } //Setting the held items references to null implicitly means that there is no held item
        private PhysicsBody HeldItemBody { get; set; } = null;
        public IPickable HeldItemInterface { get; private set; } = null;
        

        //Nodes
        private Camera FPCamera { get; set; }
        private Spatial HeadSpatial { get; set; }
        private Label TestSpeedLabel { get; set; }
        private Label TestFPSLabel { get; set; }
        private RayCast InteractRayCast { get; set; }
        private Position3D HeldItemPosition { get; set; }

        // - - - GD Methods - - -
        public override void _Ready()
        {
            base._Ready();

            FPCamera = GetNode<Camera>("Head/Camera");
            HeadSpatial = GetNode<Spatial>("Head");
            TestSpeedLabel = GetNode<Label>("Head/Camera/Direction/FPUI/SpeedLabel");
            TestFPSLabel = GetNode<Label>("Head/Camera/Direction/FPUI/FPSLabel");
            InteractRayCast = GetNode<RayCast>("Head/InteractCast");
            HeldItemPosition = GetNode<Position3D>("Head/HeldItemPos");

            Input.SetMouseMode(Input.MouseMode.Captured);
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            TestFPSLabel.Text = $"FPS: {Engine.GetFramesPerSecond()}";
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            ProcessMovement(delta);

            //If there is an actively held item, it should be coaxed to move to the held item position.
            //This might need to move to its own method as it is outside of the scope of physics process
            if (HoldingItem)
            {
                Transform trans = HeldItemBody.GlobalTransform;               
                trans.origin = 
                    FPCamera.GlobalTransform.origin + (-FPCamera.GlobalTransform.basis.z.Normalized() * ObjectGrabDistance);
                HeldItemBody.GlobalTransform = trans;
            }
        }

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);
            //Capture mouse input for the camera
            if(@event is InputEventMouseMotion && Input.GetMouseMode() == Input.MouseMode.Captured)
            {
                ProcessRotation(@event as InputEventMouseMotion);
            }
            //Capture keypresses
            if (@event.IsActionPressed("interact_grab")) { ProcessInteract(); }
            if (@event.IsActionPressed("combat_fireweapon")) 
            {
                if (HoldingItem) { ThrowObject(); }
                else
                { //Shoot weapon 
                }
            }
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

            //Test label
            TestSpeedLabel.Text = ((int)TickVelocity.Length()).ToString() + " m/sec\n" + TickOnFloor;

            TickOnFloor = IsOnFloor(); //Always call IsOnFloor() after moveand* is called
        }

        private void ProcessRotation(InputEventMouseMotion mouseMov)
        {
            RotateY(Mathf.Deg2Rad(-mouseMov.Relative.x * CameraSpeed));
            HeadSpatial.RotateX(Mathf.Deg2Rad(-mouseMov.Relative.y * CameraSpeed));
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
                    PhysicsBody body = null;
                    if (pickable.RequestPickup(out body, this))
                    {
                        HeldItemBody = body;
                        HeldItemInterface = pickable;
                        GrabObject(body); 
                    }
                    else 
                    {
                        //Maybe a sound effect could play here ah-la half life?
                        GD.Print("Request to pickup object was denied.");
                    }
                }
            }
        }

        private void GrabObject(PhysicsBody body)
        {
            body.GlobalTransform = HeldItemPosition.GlobalTransform;
        }

        private void ThrowObject()
        {
            var throwAccept =
                HeldItemInterface.RequestThrow(this, Vector3.Zero, -FPCamera.GlobalTransform.basis.z.Normalized() * ObjectThrowForce);
            if (throwAccept)
            {
                HeldItemBody = null;
                HeldItemInterface = null;
            }
            else
            {
                GD.Print("Could not throw object, denied.");
            }
        }

        private void DropObject()
        {
            HeldItemBody = null;
            HeldItemInterface = null;
        }
    }   
}