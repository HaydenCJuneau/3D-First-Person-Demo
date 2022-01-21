using Godot;
using System;
using System.Collections.Generic;
using Game.Systems.Interaction;

namespace Game.Sandwich.Objects
{
    //The bottom bun handles stacking and destruction, and holds data about a completed item.
    /*
        TODO:
        - Methods that accepts a new ingredient, if that ingredient is the top bun, complete the item
        - Behavior to accept an initial ingredient
        - When completed item is thrown against wall, shatter item
        - Manage the list of ingreds in the item
    */
    public class BottomBunIngredient : RigidBody, IPickable
    {
        //--State
        public bool PartOfCompleteItem { get; private set; } = false;
        private bool BeingThrown { get; set; } = false;
        //--Properties
        public List<Ingredient> IngredientStack { get; private set; }
        //--Nodes
        private RayCast IngredCast { get; set; }

        // - - GD Methods - - 
        public override void _Ready()
        {
            base._Ready();

            IngredCast = GetNode<RayCast>("IngredientCast");

            IngredientStack = new List<Ingredient>();
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            //Glue all ingredients to the x and z pos of this bun
            foreach(var ingred in IngredientStack)
            {

            }
        }

        // - - Collision - - 
        public void OnCollisionBodyEntered(Node body)
        {
            if(IngredientStack.Count == 0) //Only scan if this is the first item
            {
                var cast = IngredCast.GetCollider();
                if (cast == null || !(cast is Ingredient)) { return; }

            }
        }

        // - - Stacking and Ingredients - - 
        public void Stack(Ingredient newIngred)
        {
            if(IngredientStack.Count != 0) 
            { 
                IngredientStack[IngredientStack.Count - 1].SetMiddleOfStack(this); 
            }
            //Set new ingredient to the top
            newIngred.SetTopOfStack(this);
            IngredientStack.Add(newIngred);
            //Check for completion
            if(newIngred.Type == IngredientType.TopBun)
            {
                foreach(var ingred in IngredientStack)
                {
                    ingred.SetComplete();
                }

                PartOfCompleteItem = true;
            }
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