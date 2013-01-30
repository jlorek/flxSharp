using Microsoft.Xna.Framework.Input;
using System;

namespace flxSharp.flxSharp.System
{
    public class FlxMouse
    {
        private MouseState old;
        private MouseState current;
        public ButtonState button;
        public float screenX;
        public float screenY;
        public int wheel;

        public FlxMouse()
        {
        }

        public void show()
        {
           
        }

        public void update()
        {
            old = current;
            current = Mouse.GetState();
            screenX = current.X;
            screenY = current.Y;
        }

        /// <summary>
        /// Returns true if the Left Mouse button was just pressed
        /// </summary>
        /// <returns></returns>
        public bool justPressedLeft()
        {
            if ((current.LeftButton == ButtonState.Pressed) && (old.LeftButton == ButtonState.Released))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Returns true if the Left Mouse button was just released
        /// </summary>
        /// <returns></returns>
        public bool justReleasedLeft()
        {
            if ((old.LeftButton == ButtonState.Pressed) && (current.LeftButton == ButtonState.Released))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Returns true if the Left Mouse button is held down
        /// </summary>
        /// <returns></returns>
        public bool pressedLeft()
        {
            if ((current.LeftButton == ButtonState.Pressed) && (old.LeftButton == ButtonState.Pressed))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Returns true if the Middle Mouse button was just pressed
        /// </summary>
        /// <returns></returns>
        public bool justPressedMiddle()
        {
            if ((current.MiddleButton == ButtonState.Pressed) && (old.MiddleButton == ButtonState.Released))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Returns true if the Middle Mouse button was just released
        /// </summary>
        /// <returns></returns>
        public bool justReleasedMiddle()
        {
            if ((old.MiddleButton == ButtonState.Pressed) && (current.MiddleButton == ButtonState.Released))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Returns true if the Middle Mouse button is held down
        /// </summary>
        /// <returns></returns>
        public bool pressedMiddle()
        {
            if ((current.MiddleButton == ButtonState.Pressed) && (old.MiddleButton == ButtonState.Pressed))
                return true;
            else
                return false;
        }


        /// <summary>
        /// Returns true if the Right Mouse button was just pressed
        /// </summary>
        /// <returns></returns>
        public bool justPressedRight()
        {
            if ((current.RightButton == ButtonState.Pressed) && (old.RightButton == ButtonState.Released))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Returns true if the Right Mouse button was just released
        /// </summary>
        /// <returns></returns>
        public bool justReleasedRight()
        {
            if ((old.RightButton == ButtonState.Pressed) && (current.RightButton == ButtonState.Released))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Returns true if the Right Mouse button is held down
        /// </summary>
        /// <returns></returns>
        public bool pressedRight()
        {
            if ((current.RightButton == ButtonState.Pressed) && (old.RightButton == ButtonState.Pressed))
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
