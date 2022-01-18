using Godot;
using System;
using Game.Systems.Interaction;

namespace Game.Sandwich.Objects
{
    public class BottomBunIngredient : RigidBody, IPickable
    {

        // - - Interactivity - - 
        public bool RequestDrop(object sender)
        {
            return true;
        }

        public bool RequestPickup(out RigidBody body, object sender)
        {
            body = this;
            return true;
        }

        public bool RequestThrow(object sender)
        {
            return true;
        }
    }
}