using System;
using Godot;

namespace Game.Systems.Interaction
{
    public interface IPickable
    {
        /// <summary>
        /// Send a request to the object to be picked up.
        /// Object will return true or false depending on if it can be picked up in the frame the request is sent.
        /// </summary>
        bool RequestPickup(out RigidBody body, object sender);
        //Request pickup must also return a reference to the objects physics body so that its position can be clamped to the 'hold' point
        //This means, however, that this interface can only be implemented into classes that derive from physicsbody, which should not be much of 
        //a problem anyways.

        /// <summary>
        /// Send a request to the object to be dropped.
        /// Object will return true or false depending on if it can be dropped in the frame the request is sent.
        /// </summary>
        bool RequestDrop(object sender);

        /// <summary>
        /// Send a request to the object to be thrown.
        /// Object will return true or false depending on if this is allowed. 
        /// Also takes data to apply to the throw impulse
        bool RequestThrow(object sender);

        //I think it is a good idea to have these methods return booleans because it will allow us to let 
        //pickable objects decide on whether they can be picked up without changing their collision groups. 
        //ie: when meat is cooking, it should return false to being picked up, but we dont want it to have to disable the 'pickables' collision group
        //This means that in the code for the player, remember to check for this return value when trying to pick up an object, to make sure it
        //respects the permission. - HCJ
    }
}
