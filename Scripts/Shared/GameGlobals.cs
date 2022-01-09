using System;
using Godot;

namespace Game
{
    public static class GameGlobals
    {
        //If there will ever be a need to change gravity during runtime, this should be a static property
        public const float GLOBALGRAVITY = 12.8f;
        //We are not running VSync during testing, so set the desired fps here.
        //It is not reccomended to let it go for unlimited fps because it will overload your GPU
        public const int TARGETFPS = 120;

        public delegate void ObjectEvent();
    }
}
