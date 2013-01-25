using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace fliXNA_xbox
{
    public class FlxPoint
    {

        /// <summary>
        /// Default 0
        /// </summary>
        public float x;

        /// <summary>
        /// Default 0
        /// </summary>
        public float y;

        /// <summary>
        /// Instantiate a new point object
        /// </summary>
        /// <param name="X">The X-coordinate of the point in space</param>
        /// <param name="Y">The Y-coordinate of the point in space</param>
        /// <returns></returns>
        public FlxPoint(float X = 0, float Y = 0)
        {
            make(X, Y);
        }

        /// <summary>
        /// Instantiate a new point object
        /// </summary>
        /// <param name="X">The X-coordinate of the point in space</param>
        /// <param name="Y">The Y-coordinate of the point in space</param>
        /// <returns></returns>
        public FlxPoint make(float X = 0, float Y = 0)
        {
            x = X;
            y = Y;
            return this;
        }

        /// <summary>
        /// Helper function, just copies the values from the specified point
        /// </summary>
        /// <param name="Point">Any FlxPoint</param>
        /// <returns>a reference to itself</returns>
        public FlxPoint copyFrom(FlxPoint Point)
        {
            x = Point.x;
            y = Point.y;
            return this;
        }

        /// <summary>
        /// Helper function, just copies the values to the specified point
        /// </summary>
        /// <param name="Point">Any FlxPoint</param>
        /// <returns>a reference to the altered point parameter</returns>
        public FlxPoint copyTo(FlxPoint Point)
        {
            Point.x = x;
            Point.y = y;
            return Point;
        }

        /// <summary>
        /// Return this FlxPoint as a Vector2.
        /// Useful for drawing to the SpriteBatch since it requires a Vector2
        /// </summary>
        /// <returns>Vector2</returns>
        public Vector2 getVec2()
        {
            return new Vector2(x, y);
        }
    }
}
