using System;

namespace Game.Sandwich
{
    //An enum representing all the components for sandwiches.
    //Since the default value for enums is always 0, ChickenRaw will be the default type, should there be an empty type?
    public enum IngredientType
    {
        //Meats
        ChickenRaw = 0,
        ChickenCooked = 1,
        BeefRaw = 2,
        BeefCooked = 3,
        Burnt = 4,
        //Toppings
        Cheese = 5,
        Lettuce = 6,
        Pickle = 7,
        Tomato = 8,
        //Buns
        BottomBun = 9,
        TopBun = 10
    }
    /* NOTE: The numbers assigned to each ingredient is used when determining what scene to get its model from.
     * These could be changed to better search for its scene, eg: using odd numbers for toppings and even for meats.
     * This might be overengineering though, so we can see how far this takes us.
    */
}