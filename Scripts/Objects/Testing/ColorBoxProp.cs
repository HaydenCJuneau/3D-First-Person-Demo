using System;
using Godot;
using Game.Systems.Interaction;

namespace Game.Objects
{
    public class ColorBoxProp : RigidBody, IInteractable
    {
        //The CSG Box of the cube
        private CSGBox Box { get; set; }
        //The cubes color value
        private Color CubeColor
        {
            get {
                var box = Box ?? GetNode<CSGBox>("Model");
                var mat = box.Material as SpatialMaterial;
                return mat.AlbedoColor;
            }
            set {
                var box = Box ?? GetNode<CSGBox>("Model");
                var mat = box.Material as SpatialMaterial;
                mat.AlbedoColor = value;
            }
        }
        //Random
        private Random rng = new Random();

        // - - - GD methods - - - 
        public override void _Ready()
        {
            base._Ready();

            Box = GetNode<CSGBox>("Box2");
        }

        public void Interact(object sender)
        {
            GD.Print($"{nameof(sender)} interacted with cube!");
            CubeColor = new Color((float)rng.NextDouble(), (float)rng.NextDouble(), (float)rng.NextDouble());
        }
    }
}
