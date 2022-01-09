using Godot;
using System;
using Game.Scenes;

namespace Game.Objects
{
    public class BallSpawner : Spatial
    {
        //--Scenes and nodes
        private PackedScene BallScene { get; set; }
        private SceneBase Base { get; set; }
        //--Event linking
        [Export] public string EventSenderID { get; private set; }


        // - - GD Methods - - 
        public override void _Ready()
        {
            base._Ready();

            BallScene = GD.Load<PackedScene>("res://Scenes/Props/Testing/RainbowBall.tscn");
            Base = SceneBase.FindBase(this);
            //Link events
            Base.LinkEventByID(EventSenderID, SpawnBall);
        }


        // - - Event code - - 
        public void SpawnBall()
        {
            var instance = BallScene.Instance<RigidBody>();
            instance.Translation = Translation;
            Base.AddChild(instance, true);
        }
    }
}