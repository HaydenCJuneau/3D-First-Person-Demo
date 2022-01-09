using System;
using Godot;
using static Game.GameGlobals;

namespace Game.Systems.Interaction
{
    //An event sender is any object that has events that need to be searched for and linked
    //This system allows us to have many different objects connected by events, while not needing to hard code
    //The links for the events. Instead the event retriever just needs to share a search id with the event sender.
    public interface IEventSender
    {
        //TODO: This setup will only work when the object has one event to link.
        //For objects that have multiple objects to link, we will need a dictionary with events and event ids
        string EventSenderID { get; }

        void LinkEvent(ObjectEvent @event);
        //void UnlinkEvent(ObjectEvent @event);
    }
}
