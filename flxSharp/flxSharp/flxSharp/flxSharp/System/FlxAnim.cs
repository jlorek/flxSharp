using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fliXNA_xbox
{
    public class FlxAnim
    {
        public String name;
        public float delay;
        public int[] frames;
        public Boolean looped;

        public FlxAnim(String Name, int[] Frames, float FrameRate, Boolean Looped)
        {
            name = Name;
            delay = 0;
            if (FrameRate > 0)
                delay = 1.0f / FrameRate;
            frames = Frames;
            looped = Looped;
        }

        public void destroy()
        {
            frames = null;
        }
    }
}
