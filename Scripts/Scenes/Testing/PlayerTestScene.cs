using Godot;
using System;
using Game.Entities;

namespace Game.Scenes
{
    public class PlayerTestScene : SceneBase
    {
        private PackedScene PlayerScene { get; set; }

        public override void _Ready()
        {
            base._Ready();

            PlayerScene = GD.Load<PackedScene>("res://Scenes/Player/Player.tscn");

            var inst = PlayerScene.Instance() as Player;
            inst.Translation = new Vector3(0, 5, 0);
            
            AddChild(inst);
        }
    }
}