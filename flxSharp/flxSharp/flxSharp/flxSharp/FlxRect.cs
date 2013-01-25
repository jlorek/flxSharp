using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fliXNA_xbox
{
    public class FlxRect
    {
        /**
         * @default 0
         */
        public float x;
        /**
         * @default 0
         */
        public float y;
        /**
         * @default 0
         */
        public float width;
        /**
         * @default 0
         */
        public float height;

        /**
         * Instantiate a new rectangle.
         * 
         * @param	X		The X-coordinate of the point in space.
         * @param	Y		The Y-coordinate of the point in space.
         * @param	Width	Desired width of the rectangle.
         * @param	Height	Desired height of the rectangle.
         */
        public FlxRect(float X = 0, float Y = 0, float Width = 0, float Height = 0)
        {
            x = X;
            y = Y;
            width = Width;
            height = Height;
        }

        /**
         * The X coordinate of the left side of the rectangle.  Read-only.
         */
        public float left
        {
            get { return x; }
        }


        /**
         * The X coordinate of the right side of the rectangle.  Read-only.
         */
        public float right
        {
            get { return x + width; }
        }

        /**
         * The Y coordinate of the top of the rectangle.  Read-only.
         */
        public float top
        {
            get { return y; }
        }

        /**
         * The Y coordinate of the bottom of the rectangle.  Read-only.
         */
        public float bottom
        {
            get { return y + height; }
        }


        public float midX
        {
            get { return x + ((width) / 2); }
        }

        public float midY
        {
            get { return y + ((height) / 2); }
        }

        /**
         * Instantiate a new rectangle.
         * 
         * @param	X		The X-coordinate of the point in space.
         * @param	Y		The Y-coordinate of the point in space.
         * @param	Width	Desired width of the rectangle.
         * @param	Height	Desired height of the rectangle.
         * 
         * @return	A reference to itself.
         */
        public FlxRect make(float X = 0, float Y = 0, float Width = 0, float Height = 0)
        {
            x = X;
            y = Y;
            width = Width;
            height = Height;
            return this;
        }

        /**
         * Checks to see if some <code>FlxRect</code> object overlaps this <code>FlxRect</code> object.
         * 
         * @param	Rect	The rectangle being tested.
         * 
         * @return	Whether or not the two rectangles overlap.
         */
        public Boolean overlaps(FlxRect Rect)
        {
            return (Rect.x + Rect.width > x) && (Rect.x < x + width) && (Rect.y + Rect.height > y) && (Rect.y < y + height);
        }

        public FlxRect copyFrom(FlxRect Rect)
        {
            x = Rect.x;
            y = Rect.y;
            width = Rect.width;
            height = Rect.height;
            return this;
        }

        public FlxRect copyTo(FlxRect Rect)
        {
            Rect.x = x;
            Rect.y = y;
            Rect.width = width;
            Rect.height = height;
            return this;
        }
    }
}
