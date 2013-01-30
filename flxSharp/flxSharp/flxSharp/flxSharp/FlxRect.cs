using System;
using Microsoft.Xna.Framework;

namespace flxSharp.flxSharp
{
    /// <summary>
    /// Stores a rectangle.
    /// </summary>
    public class FlxRect
    {
        /// <summary>
        /// The X-coordinate of the rectangle in space.
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// The Y-coordinate of the rectangle in space.
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// Desired width of the rectangle.
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// Desired height of the rectangle.
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        /// The X coordinate of the left side of the rectangle. Read-only.
        /// </summary>
        public float Left
        {
            get { return X; }
        }

        /// <summary>
        /// The X coordinate of the right side of the rectangle. Read-only.
        /// </summary>
        public float Right
        {
            get { return X + Width; }
        }

        /// <summary>
        /// The Y coordinate of the top of the rectangle. Read-only.
        /// </summary>
        public float Top
        {
            get { return Y; }
        }

        /// <summary>
        /// The Y coordinate of the bottom of the rectangle. Read-only.
        /// </summary>
        public float Bottom
        {
            get { return Y + Height; }
        }

        /// <summary>
        /// Instantiate a new rectangle.
        /// </summary>
        /// <param name="x">The X-coordinate of the rectangle in space.</param>
        /// <param name="y">The Y-coordinate of the rectangle in space.</param>
        /// <param name="width">Desired width of the rectangle.</param>
        /// <param name="height">Desired height of the rectangle.</param>
        public FlxRect(float x = 0, float y = 0, float width = 0, float height = 0)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Instantiate a new rectangle.
        /// Protip: Values of the current instance are set, but nothing is created here.
        /// </summary>
        /// <param name="x">The X-coordinate of the rectangle in space.</param>
        /// <param name="y">The Y-coordinate of the rectangle in space.</param>
        /// <param name="width">Desired width of the rectangle.</param>
        /// <param name="height">Desired height of the rectangle.</param>
        /// <returns></returns>
        public FlxRect make(float x = 0, float y = 0, float width = 0, float height = 0)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            return this;
        }

        /// <summary>
        /// Helper function, just copies the values from the specified rectangle.
        /// </summary>
        /// <param name="rect">Any <code>FlxRect</code>.</param>
        /// <returns>A reference to itself.</returns>
        public FlxRect copyFrom(FlxRect rect)
        {
            X = rect.X;
            Y = rect.Y;
            Width = rect.Width;
            Height = rect.Height;
            return this;
        }

        /// <summary>
        /// Helper function, just copies the values from this rectangle to the specified rectangle.
        /// </summary>
        /// <param name="rect">Any <code>FlxRect</code>.</param>
        /// <returns>A reference to the altered rectangle parameter.</returns>
        public FlxRect copyTo(FlxRect rect)
        {
            rect.X = X;
            rect.Y = Y;
            rect.Width = Width;
            rect.Height = Height;
            return this;
        }

        /// <summary>
        /// Helper function, just copies the values from the specified Flash rectangle.
        /// </summary>
        /// <param name="flashRect">Any <code>Rectangle</code>.</param>
        /// <returns>A reference to itself.</returns>
        public FlxRect copyFromFlash(object flashRect)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Helper function, just copies the values from this rectangle to the specified Flash rectangle.
        /// </summary>
        /// <param name="flashRect">Any <code>Rectangle</code>.</param>
        /// <returns>A reference to the altered rectangle parameter.</returns>
        public object copyToFlash(object flashRect)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Checks to see if some <code>FlxRect</code> object overlaps this <code>FlxRect</code> object.
        /// </summary>
        /// <param name="rect">The rectangle being tested.</param>
        /// <returns>Whether or not the two rectangles overlap.</returns>
        public Boolean overlaps(FlxRect rect)
        {
            return (rect.X + rect.Width > X) && (rect.X < X + Width) && (rect.Y + rect.Height > Y) && (rect.Y < Y + Height);
        }

        /// <summary>
        /// Create a new <code>Rectangle</code> from this <code>FlxRect</code>.
        /// </summary>
        /// <returns>A new <code>Rectangle</code> with the same position and dimensions.</returns>
        public Rectangle ToRectangle()
        {
            return new Rectangle((int) X, (int) Y, (int) Width, (int) Height);
        }
    }
}
