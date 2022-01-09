using System;
using Godot;
using Game.Systems.Interaction;

namespace Game.Objects
{
    public class RainbowBallProp : RigidBody, IPickable
    {
        //The CSG Box of the cube
        private CSGSphere Model { get; set; }
        //The cubes color value
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
        //Random
        private Random rng = new Random();
        //Float for elapsed time
        private float ElapsedTime = 0f;

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
        public bool RequestPickup(out PhysicsBody body, object sender)
        {
            body = this;
            Mode = ModeEnum.Static;
            GD.Print($"Ball picked up by {nameof(sender)}");
            return true;
        }

        public bool RequestDrop(object sender)
        {
            Mode = ModeEnum.Rigid;
            GD.Print($"Ball dropped by {nameof(sender)}");
            return true;
        }

        public bool RequestThrow(object sender, Vector3 impulsePosition, Vector3 impulse)
        {
            Mode = ModeEnum.Rigid;
            ApplyImpulse(impulsePosition, impulse);
            GD.Print($"Ball thrown by {nameof(sender)}");
            return true;
        }
    }
}
