using System;
using Microsoft.Xna.Framework;

namespace flxSharp.flxSharp
{
    /// <summary>
    /// Stores a 2D floating point coordinate.
    /// </summary>
    public class FlxPoint
    {
        /// <summary>
        /// Default 0.
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Default 0.
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// Instantiate a new point object.
        /// </summary>
        /// <param name="x">The X-coordinate of the point in space.</param>
        /// <param name="y">The Y-coordinate of the point in space.</param>
        /// <returns></returns>
        public FlxPoint(float x = 0, float y = 0)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Instantiate a new point object.
        /// 
        /// The naming is very confusing... This method does not
        /// make or create a point (like the NEW keyword). Instead
        /// it only set's the X and Y value of the current instance
        /// and returnes this. So this is NOT a factory method.
        /// </summary>
        /// <param name="x">The X-coordinate of the point in space.</param>
        /// <param name="y">The Y-coordinate of the point in space.</param>
        /// <returns></returns>
        public FlxPoint make(float x = 0, float y = 0)
        {
            X = x;
            Y = y;
            return this;
        }

        /// <summary>
        /// Helper function, just copies the values from the specified point.
        /// </summary>
        /// <param name="point">Any <code>FlxPoint</code>.</param>
        /// <returns>A reference to itself.</returns>
        public FlxPoint copyFrom(FlxPoint point)
        {
            X = point.X;
            Y = point.Y;
            return this;
        }

        /// <summary>
        /// Helper function, just copies the values to the specified point.
        /// </summary>
        /// <param name="point">Any <code>FlxPoint</code>.</param>
        /// <returns>A reference to itself.</returns>
        public FlxPoint copyTo(FlxPoint point)
        {
            point.X = X;
            point.Y = Y;
            return point;
        }

        /// <summary>
        /// Helper function, just copies the values from the specified Flash point.
        /// </summary>
        /// <param name="flashPoint">Point	Any <code>Point</code>.</param>
        /// <returns>A reference to itself.</returns>
        public FlxPoint copyFromFlash(object flashPoint)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Helper function, just copies the values from this point to the specified Flash point.
        /// </summary>
        /// <param name="flashPoint">Any <code>Point</code>.</param>
        /// <returns>A reference to the altered point parameter.</returns>
        public object copyToflash(object flashPoint)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Create a <code>Vector2</code> from this <code>FlxPoint</code>.
        /// </summary>
        /// <returns>A <code>Vector2</code> with the same X and Y values.</returns>
        public Vector2 ToVector2()
        {
            return new Vector2(X, Y);
        }
    }
}
