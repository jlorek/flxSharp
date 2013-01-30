using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace fliXNA_xbox
{
    public class FlxKeyboard
    {
        private KeyboardState old;
        private KeyboardState current;
        static public Buttons buttons;

        public FlxKeyboard()
        {
        }

        public void update()
        {
            old = current;
            current = Keyboard.GetState();
        }

        /// <summary>
        /// Returns true if the specified Key was just pressed
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public bool justPressed(Keys Key)
        {
            if ((current.IsKeyDown(Key) && old.IsKeyUp(Key)))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Returns true if the specified Key was just released
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public bool justReleased(Keys Key)
        {
            if ((old.IsKeyDown(Key) && current.IsKeyUp(Key)))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Returns true if the specified Key is held down
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public bool pressed(Keys Key)
        {
            if (current.IsKeyDown(Key))
                return true;
            else
                return false;
        }

        public void reset()
        {
            //throw new NotImplementedException();
        }
    }
}
