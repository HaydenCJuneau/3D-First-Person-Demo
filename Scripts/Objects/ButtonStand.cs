using Godot;
using System;
using static Game.GameGlobals;
using Game.Systems.Interaction;

namespace Game.Objects
{
    public class ButtonStand : StaticBody, IInteractable, IEventSender
    {
        //--Control info
        [Export] public bool Enabled { get; private set; }
        [Export] public float PressCooldown { get; private set; }
        public bool OnCooldown { get; private set; } = false;
        //--Button event       
        [Export] public string EventSenderID { get; private set; }
        public event ObjectEvent ButtonPressedEvent;
        //--Nodes
        private CSGCylinder ButtonModel { get; set; }
        private Timer CooldownTimer { get; set; }
        

        // - - GD Methods - - 
        public override void _Ready()
        {
            base._Ready();

            ButtonModel = GetNode<CSGCylinder>("ButtonModel");
            CooldownTimer = GetNode<Timer>("CooldownTimer");

            var mat = ButtonModel.Material as SpatialMaterial;
            
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);


        }

        // - - Interface - - 
        public void Interact(object sender)
        {
            if(Enabled && !OnCooldown)
            {
                GD.Print($"Button pressed by {nameof(sender)}");
                ButtonPressedEvent?.Invoke();
                OnCooldown = true;
                //Visual effect. Lower button model and change color to green, then start cooldown timer.
                ButtonModel.Translation = new Vector3(0, 0.95f, 0);
                var mat = ButtonModel.Material as SpatialMaterial;
                mat.AlbedoColor = new Color(0, 1, 0);
                CooldownTimer.Start(PressCooldown);
            }
            else
            {
                GD.Print("Button disabled!");
            }
        }

        public void LinkEvent(ObjectEvent @event)
        {
            if(@event != null)
            {
                ButtonPressedEvent += @event;
            }
            else
            {
                GD.PrintErr("Event was not able to be linked to button!");
            }
        }

        // - - Signals - - 
        public void CooldownTimeOut()
        {
            OnCooldown = false;
            //Visual effect.
            ButtonModel.Translation = new Vector3(0, 1, 0);
            var mat = ButtonModel.Material as SpatialMaterial;
            mat.AlbedoColor = new Color(1, 0, 0);
        }

        
    }
}