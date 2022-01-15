using Godot;
using System;
using Game.Scenes;

namespace Game.Objects
{
    public class PropSpawner : Spatial
    {
        //--Scenes and nodes
        [Export] public string PropScenePath { get; private set; }
        private PackedScene PropScene { get; set; }
        private SceneBase Base { get; set; }
        //--Event linking
        [Export] public string EventSenderID { get; private set; }


        // - - GD Methods - - 
        public override void _Ready()
        {
            base._Ready();

            PropScene = GD.Load<PackedScene>(PropScenePath);
            Base = SceneBase.FindBase(this);
            //Link events
            Base.LinkEventByID(EventSenderID, SpawnProp);
        }


        // - - Event code - - 
        public void SpawnProp()
        {
            var instance = PropScene.Instance<Spatial>();
            instance.Translation = Translation;
            Base.AddChild(instance, true);
        }
    }
}