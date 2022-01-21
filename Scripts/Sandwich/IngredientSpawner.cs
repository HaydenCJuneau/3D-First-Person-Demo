using Godot;
using System;


namespace Game.Sandwich
{
    public class IngredientSpawner : Area
    {
        //--Nodes
        private Position3D SpawnPosition { get; set; }
        //--Properties
        [Export] public IngredientType SpawnIngredient { get; private set; }
        //--Packed Scenes
        private PackedScene ToppingScene { get; } = GD.Load<PackedScene>("res://Scenes/Sandwich/Props/ToppingIngredient.tscn");
        private PackedScene MeatScene { get; } = GD.Load<PackedScene>("res://Scenes/Sandwich/Props/ToppingIngredient.tscn");
        private PackedScene BottomBunScene { get; } = GD.Load<PackedScene>("res://Scenes/Sandwich/Props/ToppingIngredient.tscn");

        // - - - GD Methods - - -
        public override void _Ready()
        {
            base._Ready();

            SpawnPosition = GetNode<Position3D>("SpawnPoint");

            //Spawn the set ingredient type (move to own method after testing)
            
        }
    }
}