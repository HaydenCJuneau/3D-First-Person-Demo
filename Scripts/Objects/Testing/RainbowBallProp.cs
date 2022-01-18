using System;
using Godot;
using Game.Systems.Interaction;

namespace Game.Objects
{
    public class RainbowBallProp : RigidBody, IPickable
    {
        //--Nodes
        private CSGSphere Model { get; set; }
        //--Control values
        private float ElapsedTime { get; set; } = 0f;
        private Color MeshColor
        {
            get {
                var box = Model ?? GetNode<CSGSphere>("Model");
                var mat = box.Material as SpatialMaterial;
                return mat.AlbedoColor;
            }
            set {
                var box = Model ?? GetNode<CSGSphere>("Model");
                var mat = box.Material as SpatialMaterial;
                mat.AlbedoColor = value;
            }
        }

        // - - - GD methods - - - 
        public override void _Ready()
        {
            base._Ready();

            Model = GetNode<CSGSphere>("Model");
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            ElapsedTime += delta;
            //This is stupid lmao but it make rainbow ball!!! XDD
            MeshColor = new Color((Mathf.Sin(ElapsedTime * 5) * 0.5f) + 0.5f, (Mathf.Sin((ElapsedTime - 0.4f) * 5) * 0.5f) + 0.5f, 
                (Mathf.Sin((ElapsedTime - 2.1f) * 5) * 0.5f) + 0.5f);
        }

        // - - Object pickup - - 
        public bool RequestPickup(out RigidBody body, object sender)
        {
            body = this;
            return true;
        }

        public bool RequestDrop(object sender)
        {
            return true;
        }

        public bool RequestThrow(object sender)
        {
            return true;
        }
    }
}
