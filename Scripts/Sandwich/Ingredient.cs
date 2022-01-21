using System;
using Game.Sandwich.Objects;
using Godot;

namespace Game.Sandwich
{
    public abstract class Ingredient : RigidBody
    {
        //--Properties
        public IngredientType Type { get; protected set; }
        protected MeshInstance VisualInstance { get; private set; } = null;
        protected BottomBunIngredient ItemBottom { get; private set; }
        //--State
        public bool TopOfItem { get; private set; } = false; //Is this ingredient at the top of the item (should it scan for new ingreds)
        public bool PartOfItem { get; private set; } = false; //Is this ingred part of an item
        public bool PartOfCompleteItem { get; private set; } = false; //Is this ingred part of a complete item
        //--Nodes
        private RayCast IngredCast { get; set; }

        // - - GD Methods - - 
        public override void _Ready()
        {
            base._Ready();

            IngredCast = GetNode<RayCast>("IngredientCast");
        }

        // - - Stacking and Ingredients - - 
        public void SetTopOfStack(BottomBunIngredient bottom)
        {
            ItemBottom = bottom;
            TopOfItem = true;
            PartOfItem = true;
        }

        public void SetMiddleOfStack(BottomBunIngredient bottom)
        {
            ItemBottom = bottom;
            TopOfItem = false;
            PartOfItem = true;
        }

        public void SetComplete()
        {
            TopOfItem = false;
            PartOfItem = true;
            PartOfCompleteItem = true;
        }

        public void RemoveFromStack()
        {

        }

        // - - Collision - - 
        public virtual void OnCollisionBodyEntered(Node body)
        {
            //Check the raycast, if this ingredient is at the top of the stack and not complete yet
            if(!PartOfCompleteItem && TopOfItem)
            {
                var cast = IngredCast.GetCollider();
                if(cast == null || !(cast is Ingredient)) { return; }
                //If the cast detects an ingredient
                //Send a reference to that object to the bottom bun
            }
        }

        // - - Visuals - - 
        public virtual void SetIngredient(IngredientType type)
        {
            if (VisualInstance != null) { VisualInstance.Visible = false; }
            switch (type)
            {
                //Meats
                case IngredientType.BeefRaw:
                    VisualInstance = GetNode<MeshInstance>("BeefRawModel");
                    break;
                case IngredientType.BeefCooked:
                    VisualInstance = GetNode<MeshInstance>("BeefCookedModel");
                    break;
                case IngredientType.ChickenRaw:
                    VisualInstance = GetNode<MeshInstance>("ChickenRawModel");
                    break;
                case IngredientType.ChickenCooked:
                    VisualInstance = GetNode<MeshInstance>("ChickenCookedModel");
                    break;
                case IngredientType.Burnt:
                    VisualInstance = GetNode<MeshInstance>("BurntModel");
                    break;
                //Toppings
                case IngredientType.Cheese:
                    VisualInstance = GetNode<MeshInstance>("CheeseModel");
                    break;
                case IngredientType.Lettuce:
                    VisualInstance = GetNode<MeshInstance>("LettuceModel");
                    break;
                case IngredientType.Pickle:
                    VisualInstance = GetNode<MeshInstance>("PickleModel");
                    break;
                case IngredientType.Tomato:
                    VisualInstance = GetNode<MeshInstance>("TomatoModel");
                    break;
                //The top bun is set as a topping because it does not hold any additional logic
                case IngredientType.TopBun:
                    VisualInstance = GetNode<MeshInstance>("TopBunModel");
                    break;
            }
            //Set model to visible
            VisualInstance.Visible = true;
        }
    }
}
