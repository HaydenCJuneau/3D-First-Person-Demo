using System;
using Godot;
using Game.Systems.Interaction;

namespace Game.Objects
{
    public class ColorBoxProp : RigidBody, IInteractable
    {
        //--Cube Model
        private CSGBox Model { get; set; }
        //--Properties
        private Color CubeColor
        {
            get {
                var box = Model ?? GetNode<CSGBox>("Model");
                var mat = box.Material as SpatialMaterial;
                return mat.AlbedoColor;
            }
            set {
                var box = Model ?? GetNode<CSGBox>("Model");
                var mat = box.Material as SpatialMaterial;
                mat.AlbedoColor = value;
            }
        }
        //--RNG
        private Random rng = new Random();

        // - - - GD methods - - - 
        public override void _Ready()
        {
            base._Ready();

            Model = GetNode<CSGBox>("Model");
        }

        public void Interact(object sender)
        {
            CubeColor = new Color((float)rng.NextDouble(), (float)rng.NextDouble(), (float)rng.NextDouble());
        }
    }
}
