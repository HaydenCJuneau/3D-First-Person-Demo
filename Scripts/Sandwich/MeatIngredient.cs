using Godot;
using System;
using Game.Systems.Interaction;

namespace Game.Sandwich.Objects
{
    //Meat ingredients can be cooked in stoves or fryers so they need their own script for behavior
    public class MeatIngredient : Ingredient, IPickable
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