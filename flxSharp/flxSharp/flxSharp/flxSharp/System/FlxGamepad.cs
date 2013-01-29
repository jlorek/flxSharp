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
using flxSharp.flxSharp;

namespace fliXNA_xbox
{
    public class FlxGamepad
    {
        private GamePadState old;
        private GamePadState current;
        private PlayerIndex index;
        private float leftVibe;
        private float rightVibe;
        private float vibeDuration;
        static public Buttons buttons;
        public float leftAnalogX;
        public float leftAnalogY;
        public float rightAnalogX;
        public float rightAnalogY;
        public float leftTrigger;
        public float rightTrigger;

        public FlxGamepad(PlayerIndex IndexOfPlayer)
        {
            index = IndexOfPlayer;
            leftVibe = 0;
            rightVibe = 0;
            vibeDuration = 0;
        }

        public void update()
        {
            old = current;
            current = GamePad.GetState(index);
            leftAnalogX = current.ThumbSticks.Left.X;
            leftAnalogY = current.ThumbSticks.Left.Y;
            rightAnalogX = current.ThumbSticks.Right.X;
            rightAnalogY = current.ThumbSticks.Right.Y;
            leftTrigger = current.Triggers.Left;
            rightTrigger = current.Triggers.Right;
            if (vibeDuration > 0)
            {
                vibeDuration -= FlxG.elapsed;
                if (vibeDuration <= 0)
                {
                    leftVibe = rightVibe = 0;
                }
                GamePad.SetVibration(index, leftVibe, rightVibe);
            }
        }

        /// <summary>
        /// Returns true if the Left Analog is pushed right by any amount
        /// </summary>
        /// <returns>bool</returns>
        public bool leftAnalogPushedRight()
        {
            return leftAnalogX > 0f;
        }

        /// <summary>
        /// Returns true if the Left Analog is pushed left by any amount
        /// </summary>
        /// <returns>bool</returns>
        public bool leftAnalogPushedLeft()
        {
            return leftAnalogX < 0f;
        }

        /// <summary>
        /// Returns true if the Left Analog is pushed up by any amount
        /// </summary>
        /// <returns>bool</returns>
        public bool leftAnalogPushedUp()
        {
            return leftAnalogY > 0f;
        }

        /// <summary>
        /// Returns true if the Left Analog is pushed down by any amount
        /// </summary>
        /// <returns>bool</returns>
        public bool leftAnalogPushedDown()
        {
            return leftAnalogY < 0f;
        }

        /// <summary>
        /// Returns true if the Left Analog is centered and not being pushed in any direction
        /// </summary>
        /// <returns>bool</returns>
        public bool leftAnalogStill()
        {
            return (leftAnalogX == 0) && (leftAnalogY == 0);
        }

        /// <summary>
        /// Returns true if the Right Analog is pushed right by any amount
        /// </summary>
        /// <returns>bool</returns>
        public bool rightAnalogPushedRight()
        {
            return rightAnalogX > 0f;
        }

        /// <summary>
        /// Returns true if the Right Analog is pushed left by any amount
        /// </summary>
        /// <returns>bool</returns>
        public bool rightAnalogPushedLeft()
        {
            return rightAnalogX < 0f;
        }

        /// <summary>
        /// Returns true if the Right Analog is pushed up by any amount
        /// </summary>
        /// <returns>bool</returns>
        public bool rightAnalogPushedUp()
        {
            return rightAnalogY > 0f;
        }

        /// <summary>
        /// Returns true if the Right Analog is pushed down by any amount
        /// </summary>
        /// <returns>bool</returns>
        public bool rightAnalogPushedDown()
        {
            return rightAnalogY < 0f;
        }

        /// <summary>
        /// Returns true if the Right Analog is centered and not being pushed in any direction
        /// </summary>
        /// <returns>bool</returns>
        public bool rightAnalogStill()
        {
            return (rightAnalogX == 0) && (rightAnalogY == 0);
        }

        /// <summary>
        /// Simple Controller vibration
        /// </summary>
        /// <param name="Duration">The length in seconds the vibration should last</param>
        public void vibrate(float Duration = 0.5f)
        {
            vibrate(Duration, 0.15f, 0.15f);
        }

        /// <summary>
        /// Simple Controller vibration
        /// </summary>
        /// <param name="Duration">The length in seconds the vibration should last</param>
        /// <param name="Intensity">The intensity of the vibration for both Motors</param>
        public void vibrate(float Duration=0.5f, float Intensity=0.15f)
        {
            vibrate(Duration, Intensity, Intensity);
        }

        /// <summary>
        /// Simple Controller vibration
        /// </summary>
        /// <param name="Duration">The length in seconds the vibration should last</param>
        /// <param name="IntensityLeftMotor">The intensity of the Left Motor vibration</param>
        /// <param name="IntensityRightMotor">The intensity of the Right Motor vibration</param>
        /// <param name="ShakeScreen">Should the screen shake in unison with the controller vibration</param>
        public void vibrate(float Duration=0.5f, float IntensityLeftMotor=0.15f, float IntensityRightMotor=0.15f, bool ShakeScreen=false)
        {
            vibeDuration = Duration;
            leftVibe = IntensityLeftMotor;
            rightVibe = IntensityRightMotor;
            if (ShakeScreen)
                FlxG.shake(IntensityLeftMotor/20, Duration);
        }

        /// <summary>
        /// Returrns true if the specified button was just pressed
        /// </summary>
        /// <param name="Button">Buttons Button</param>
        /// <returns>bool</returns>
        public bool justPressed(Buttons Button)
        {
            if ((current.IsButtonDown(Button) && old.IsButtonUp(Button)))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Returrns true if the specified button was just released
        /// </summary>
        /// <param name="Button">Buttons Button</param>
        /// <returns>bool</returns>
        public bool justReleased(Buttons button)
        {
            if ((old.IsButtonDown(button) && current.IsButtonUp(button)))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Returrns true if the specified button is held down
        /// </summary>
        /// <param name="Button">Buttons Button</param>
        /// <returns>bool</returns>
        public bool pressed(Buttons button)
        {
            if (current.IsButtonDown(button))
                return true;
            else
                return false;
        }
    }
}
