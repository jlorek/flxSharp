using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using flxSharp.flxSharp;

namespace fliXNA_xbox
{
    static public class FlxU
    {

        /// <summary>
        /// Absolute value
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        internal static float abs(float Value)
        {
            return (Value > 0) ? Value : -Value;
        }

        /// <summary>
        /// Round
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        internal static float round(float Value)
        {
            float number = (int)(Value + ((Value > 0) ? 0.5 : -0.5));
            return (Value > 0) ? (number) : ((number != Value) ? (number - 1) : (number));
        }

        /// <summary>
        /// Random number based on the seed provided.
        /// </summary>
        /// <param name="Seed">Random number between 0 and 1, used to generate a predictable random number</param>
        /// <returns></returns>
        internal static float srand(float Seed)
		{
			return ((69621 * (Seed * 0x7FFFFFFF)) % 0x7FFFFFFF) / 0x7FFFFFFF;
		}

        /// <summary>
        /// Minimum value
        /// </summary>
        /// <param name="Number1"></param>
        /// <param name="Number2"></param>
        /// <returns></returns>
        internal static float min(float Number1, float Number2)
        {
            return (Number1 <= Number2) ? Number1 : Number2;
        }
        
        /// <summary>
        /// Maximum value
        /// </summary>
        /// <param name="Number1"></param>
        /// <param name="Number2"></param>
        /// <returns></returns>
        internal static float max(float Number1, float Number2)
		{
			return (Number1 >= Number2) ? Number1 : Number2;
		}

        /// <summary>
        /// Rounded up
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        internal static float ceil(float Value) 
		{
            float number = (Value);
			return (Value>0)?((number!=Value)?(number+1):(number)):(number);
		}

        /// <summary>
        /// Rounded down
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        internal static float floor(float Value)
		{
            float number = (Value);
			return (Value>0)?(number):((number!=Value)?(number-1):(number));
		}

        /// <summary>
        /// Bound a number by aq minimum and maximum.  Ensure that this number is no smaller than the min, and no larger than max.
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Min"></param>
        /// <param name="Max"></param>
        /// <returns></returns>
        internal static float bound(float Value, float Min, float Max)
        {
            float lowerBound = (Value < Min) ? Min : Value;
            return (lowerBound > Max) ? Max : lowerBound;
        }

        /// <summary>
        /// Used by FlxObjects to calculate velocity
        /// </summary>
        /// <param name="Velocity"></param>
        /// <param name="Acceleration"></param>
        /// <param name="Drag"></param>
        /// <param name="Max"></param>
        /// <returns></returns>
        static public float computeVelocity(float Velocity, float Acceleration, float Drag, float Max)
        {
            if (Acceleration != 0)
                Velocity += Acceleration * FlxG.elapsed;
            else if (Drag != 0)
            {
                float drag = Drag * FlxG.elapsed;
                if (Velocity - drag > 0)
                    Velocity = Velocity - drag;
                else if (Velocity + drag < 0)
                    Velocity += drag;
                else
                    Velocity = 0;
            }
            if ((Velocity != 0) && (Max != 10000))
            {
                if (Velocity > Max)
                    Velocity = Max;
                else if (Velocity < -Max)
                    Velocity = -Max;
            }
            return Velocity;
        }

        /// <summary>
        /// Random float between 0 and 1
        /// </summary>
        /// <returns></returns>
        internal static float random()
        {
            Random r = new Random();
            return (float)r.NextDouble(); ;
        }

        ///// <summary>
        ///// Random float between a Min and Max
        ///// </summary>
        ///// <param name="Min"></param>
        ///// <param name="Max"></param>
        ///// <returns></returns>
        internal static float randomBetween(int Min, int Max)
        {
            Random r = new Random();
            return r.Next(Min, Max);
            //return (float)r.Next(Min, Max);
        }

        /// <summary>
        /// Distance between two FlxPoints
        /// </summary>
        /// <param name="Point1"></param>
        /// <param name="Point2"></param>
        /// <returns></returns>
        internal static float getDistance(FlxPoint Point1, FlxPoint Point2)
		{
			float dx = Point1.X - Point2.X;
			float dy = Point1.Y - Point2.Y;
			return (float)Math.Sqrt(dx * dx + dy * dy);
		}

        /// <summary>
        /// Angle between two FlxPoints
        /// </summary>
        /// <param name="Point1"></param>
        /// <param name="Point2"></param>
        /// <returns></returns>
        internal static float getAngle(FlxPoint Point1, FlxPoint Point2)
		{
			float x = Point2.X - Point1.X;
			float y = Point2.Y - Point1.Y;
			if((x == 0) && (y == 0))
				return 0;
			float c1 = 3.14159265f * 0.25f;
			float c2 = 3 * c1;
			float ay = (y < 0)?-y:y;
			float angle = 0;
			if (x >= 0)
				angle = c1 - c1 * ((x - ay) / (x + ay));
			else
				angle = c2 - c1 * ((x + ay) / (ay - x));
			angle = ((y < 0)?-angle:angle)*57.2957796f;
			if(angle > 90)
				angle = angle - 270;
			else
				angle += 90;
			return angle;
		}

        /// <summary>
        /// A more precise angle between two FlxPoints???
        /// </summary>
        /// <param name="Point1"></param>
        /// <param name="Point2"></param>
        /// <returns></returns>
        internal static float getAnglePrecise(FlxPoint Point1, FlxPoint Point2)
        {
            float x = Point2.X - Point1.X;
            float y = Point2.Y - Point1.Y;
            return (float)Math.Atan2(y, x) * 180 / (float)Math.PI;
        }

        /**
		 * Rotates a point in 2D space around another point by the given angle.
		 * 
		 * @param	X		The X coordinate of the point you want to rotate.
		 * @param	Y		The Y coordinate of the point you want to rotate.
		 * @param	PivotX	The X coordinate of the point you want to rotate around.
		 * @param	PivotY	The Y coordinate of the point you want to rotate around.
		 * @param	Angle	Rotate the point by this many degrees.
		 * @param	Point	Optional <code>FlxPoint</code> to store the results in.
		 * 
		 * @return	A <code>FlxPoint</code> containing the coordinates of the rotated point.
		 */
		static public FlxPoint rotatePoint(float X, float Y, float PivotX, float PivotY, float Angle, FlxPoint Point=null)
		{
			float sin = 0;
			float cos = 0;
			float radians = Angle * -0.017453293f;
			while (radians < -3.14159265)
				radians += 6.28318531f;
			while (radians >  3.14159265)
				radians = radians - 6.28318531f;

			if (radians < 0)
			{
				sin = 1.27323954f * radians + .405284735f * radians * radians;
				if (sin < 0)
					sin = .225f * (sin *-sin - sin) + sin;
				else
					sin = .225f * (sin * sin - sin) + sin;
			}
			else
			{
				sin = 1.27323954f * radians - 0.405284735f * radians * radians;
				if (sin < 0)
					sin = .225f * (sin *-sin - sin) + sin;
				else
					sin = .225f * (sin * sin - sin) + sin;
			}

			radians += 1.57079632f;
			if (radians >  3.14159265)
				radians = radians - 6.28318531f;
			if (radians < 0)
			{
				cos = 1.27323954f * radians + 0.405284735f * radians * radians;
				if (cos < 0)
					cos = .225f * (cos *-cos - cos) + cos;
				else
					cos = .225f * (cos * cos - cos) + cos;
			}
			else
			{
				cos = 1.27323954f * radians - 0.405284735f * radians * radians;
				if (cos < 0)
					cos = .225f * (cos *-cos - cos) + cos;
				else
					cos = .225f * (cos * cos - cos) + cos;
			}

			float dx = X-PivotX;
			float dy = PivotY+Y; //Y axis is inverted in flash, normally this would be a subtract operation
			if(Point == null)
				Point = new FlxPoint();
			Point.X = PivotX + cos*dx - sin*dy;
			Point.Y = PivotY - sin*dx - cos*dy;
			return Point;
		}

    }
}
