using System;
using System.Linq;
using Godot;
using Game.Systems.Interaction;
using static Game.GameGlobals;

namespace Game.Scenes
{
    //The base scene node which ALL scenes with gameplay should inherit from
    public abstract class SceneBase : Spatial
    {

        // - - - GD Methods - - - 
        public override void _Ready()
        {
            base._Ready();

            Engine.TargetFps = TARGETFPS;
        }

        // - - Linking event senders and listeners - - 
        public void LinkEventByID(string senderID, ObjectEvent @event)
        {
            var children = GetChildren();

            foreach(var c in children)
            {
                if(c is IEventSender) 
                {
                    var evtSndr = c as IEventSender;
                    if(evtSndr.EventSenderID == senderID)
                    {
                        evtSndr.LinkEvent(@event);
                    }
                }
            }
        }

        // - - Locating scene root - - 
        public static SceneBase FindBase(Node child)
        {
            Node search = child;
            while(true)
            {
                if(search is SceneBase) { return (SceneBase)search; }
                else
                {
                    var parent = search.GetParent();
                    if(parent == null) {
                        throw new Exception("The base of this scene does not inherit from scene base! " +
                                            "\nAll scene root nodes should inherit from this abstract class!");
                    }
                    else { search = parent; }
                }
            }
        }
    }
}
