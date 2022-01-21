using Godot;
using System;
using Game.Systems.Interaction;

namespace Game.Sandwich.Objects
{
    //Toppings are ingredients that do not have their own behavior and just act as parts of items.
    public class ToppingIngredient : Ingredient, IPickable
    {

        // - - Model - - 
        public override void SetIngredient(IngredientType type)
        {
            if(type == IngredientType.TopBun) { GetNode<CollisionShape>("BunHitbox").Disabled = false; }
            else { GetNode<CollisionShape>("ToppingHitbox").Disabled = false; }

            base.SetIngredient(type);
        }

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