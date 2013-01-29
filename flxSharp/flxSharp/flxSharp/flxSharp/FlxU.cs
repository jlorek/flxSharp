using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace flxSharp.flxSharp
{
    /// <summary>
    /// General purpose helper functions.
    /// </summary>
    public static class FlxU
    {
        /// <summary>
        /// flx# startup time. Used for getTicks().
        /// </summary>
        private static readonly DateTime startupTime = DateTime.Now;

        /// <summary>
        /// Random instance for shuffeling.
        /// </summary>
        public static readonly Random Random = new Random();

        /// <summary>
        /// Opens a web page in a new tab or window.
        /// MUST be called from the UI thread or else badness.
        /// </summary>
        /// <param name="url">The address of the web page.</param>
        static public void openURL(string url)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Calculate the absolute value of a number.
        /// </summary>
        /// <param name="value">Any number.</param>
        /// <returns>The absolute value of that number.</returns>
        public static float abs(float value)
        {
            //return (value > 0) ? value : -value;

            return Math.Abs(value);
        }

        /// <summary>
        /// Round down to the next whole number. E.g. floor(1.7) == 1, and floor(-2.7) == -2.
        /// </summary>
        /// <param name="value">Any number.</param>
        /// <returns>The rounded value of that number.</returns>
        public static float floor(float value)
        {
            //float number = (int) value;
            //return (value > 0) ? (number) : ((number != value) ? (number - 1) : (number));

            return (int) Math.Floor(value);
        }

        /// <summary>
        /// Round up to the next whole number.  E.g. ceil(1.3) == 2, and ceil(-2.3) == -3.
        /// </summary>
        /// <param name="value">Any number.</param>
        /// <returns>The rounded value of that number.</returns>
        public static float ceil(float value)
        {
            //float number = (int) value;
            //return (value > 0) ? ((number != value) ? (number + 1) : (number)) : (number);

            return (int) Math.Ceiling(value);
        }

        /// <summary>
        /// Round to the closest whole number. E.g. round(1.7) == 2, and round(-2.3) == -2.
        /// </summary>
        /// <param name="value">Any number.</param>
        /// <returns>The rounded value of that number.</returns>
        public static float round(float value)
        {
            float number = (int)(value + ((value > 0) ? 0.5 : -0.5));
            return (value > 0) ? (number) : ((number != value) ? (number - 1) : (number));
        }

        /// <summary>
        /// Figure out which number is smaller.
        /// </summary>
        /// <param name="number1">Any number.</param>
        /// <param name="number2">Any number.</param>
        /// <returns>The smaller of the two numbers.</returns>
        public static float min(float number1, float number2)
        {
            //return (Number1 <= Number2) ? Number1 : Number2;

            return Math.Min(number1, number2);
        }

        /// <summary>
        /// Figure out which number is larger.
        /// </summary>
        /// <param name="number1">Any number.</param>
        /// <param name="number2">Any number.</param>
        /// <returns>The larger of the two numbers.</returns>
        public static float max(float number1, float number2)
        {
            //return (number1 >= number2) ? number1 : number2;

            return Math.Max(number1, number2);
        }

        /// <summary>
        /// Bound a number by a minimum and maximum.
        /// Ensures that this number is no smaller than the minimum,
        /// and no larger than the maximum.
        /// </summary>
        /// <param name="value">Any number.</param>
        /// <param name="min">Any number.</param>
        /// <param name="max">Any number.</param>
        /// <returns>The bounded value of the number.</returns>
        public static float bound(float value, float min, float max)
        {
            float lowerBound = (value < min) ? min : value;
            return (lowerBound > max) ? max : lowerBound;
        }

        /// <summary>
        /// Generates a random number based on the seed provided.
        /// </summary>
        /// <param name="seed">A number between 0 and 1, used to generate a predictable random number (very optional).</param>
        /// <returns>A <code>Number</code> between 0 and 1.</returns>
        public static float srand(float seed)
		{
			return ((69621 * (seed * 0x7FFFFFFF)) % 0x7FFFFFFF) / 0x7FFFFFFF;
		}

        /// <summary>
        /// Shuffles the entries in an array into a new random order.
        /// <code>FlxG.shuffle()</code> is deterministic and safe for use with replays/recordings.
        /// HOWEVER, <code>FlxU.shuffle()</code> is NOT deterministic and unsafe for use with replays/recordings.
        /// </summary>
        /// <param name="objects">A Flash <code>Array</code> object containing...stuff.</param>
        /// <param name="howManyTimes">How many swaps to perform during the shuffle operation.  Good rule of thumb is 2-4 times as many objects are in the list.</param>
        /// <returns>The same Flash <code>Array</code> object that you passed in in the first place.</returns>
        public static Array shuffle(Array objects, uint howManyTimes)
        {
            var shuffleList = objects.Cast<object>().ToList();

            for (uint i = 0; i < howManyTimes; ++i)
            {
                // this is awesome sexy ^^
                // http://stackoverflow.com/questions/273313/randomize-a-listt-in-c-sharp
                shuffleList = shuffleList.OrderBy(item => Guid.NewGuid()).ToList();
            }

            return shuffleList.ToArray();
        }

        /// <summary>
        /// Fetch a random entry from the given array.
        /// Will return null if random selection is missing, or array has no entries.
        /// <code>FlxG.getRandom()</code> is deterministic and safe for use with replays/recordings.
        /// HOWEVER, <code>FlxU.getRandom()</code> is NOT deterministic and unsafe for use with replays/recordings.
        /// </summary>
        /// <param name="objects">A Flash array of objects.</param>
        /// <param name="startIndex">Optional offset off the front of the array. Default value is 0, or the beginning of the array.</param>
        /// <param name="length">Optional restriction on the number of values you want to randomly select from.</param>
        /// <returns></returns>
        public static object getRandom(Array objects, uint startIndex = 0, uint length = 0)
        {
            if (objects != null)
            {
                if ((length == 0) || (length > objects.Length - startIndex))
                {
                    length = ((uint) objects.Length) - startIndex;
                }

                if (length > 0)
                {
                    return objects.GetValue(startIndex + (int)(Random.NextDouble() * length));
                }
            }
            
            return null;
        }

        /// <summary>
        /// Just grabs the current "ticks" or time in milliseconds that has passed since Flash Player started up.
        /// Useful for finding out how long it takes to execute specific blocks of code.
        /// </summary>
        /// <returns>A <code>uint</code> to be passed to <code>FlxU.endProfile()</code>.</returns>
        public static long getTicks()
        {
            return (DateTime.Now - startupTime).Ticks;
        }

        /// <summary>
        /// Takes two "ticks" timestamps and formats them into the number of seconds that passed as a String.
        /// Useful for logging, debugging, the watch window, or whatever else.
        /// </summary>
        /// <param name="startTicks">The first timestamp from the system.</param>
        /// <param name="endTicks">The second timestamp from the system.</param>
        /// <returns>A <code>String</code> containing the formatted time elapsed information.</returns>
        public static string formatTicks(long startTicks, long endTicks)
        {
            return string.Format("{0}s", (endTicks - startTicks) / TimeSpan.TicksPerSecond);
        }

        /// <summary>
        /// Generate a Flash <code>uint</code> color from RGBA components.
        /// </summary>
        /// <param name="red">The red component, between 0 and 255.</param>
        /// <param name="green">The green component, between 0 and 255.</param>
        /// <param name="blue">The blue component, between 0 and 255.</param>
        /// <param name="alpha">How opaque the color should be, either between 0 and 1 or 0 and 255.</param>
        /// <returns>The color as a <code>uint</code></returns>
        public static uint makeColor(uint red, uint green, uint blue, float alpha = 1.0f)
        {
            return ((alpha > 1 ? (uint) alpha : (uint)(alpha * 255)) & 0xFF) << 24 | (red & 0xFF) << 16 | (green & 0xFF) << 8 | (blue & 0xFF);
        }

        /// <summary>
        /// Generate a Flash <code>uint</code> color from HSB components.
        /// </summary>
        /// <param name="hue">A number between 0 and 360, indicating position on a color strip or wheel.</param>
        /// <param name="saturation">A number between 0 and 1, indicating how colorful or gray the color should be.  0 is gray, 1 is vibrant.</param>
        /// <param name="brightness">A number between 0 and 1, indicating how bright the color should be.  0 is black, 1 is full bright.</param>
        /// <param name="alpha">How opaque the color should be, either between 0 and 1 or 0 and 255.</param>
        /// <returns>The color as a <code>uint</code>.</returns>
        public static uint makeColorFromHSB(float hue, float saturation, float brightness, float alpha = 1.0f)
        {
			float red;
			float green;
			float blue;

			if(saturation == 0)
			{
				red = brightness;
				green = brightness;        
				blue = brightness;
			}       
			else
			{
				if (hue == 360)
				{
					hue = 0;
				}

				int slice = (int)(hue / 60);
				float hf = hue / 60 - slice;
				float aa = brightness * (1 - saturation);
				float bb = brightness * (1 - saturation * hf);
				float cc = brightness * (1 - saturation * (1.0f - hf));

				switch (slice)
				{
					case 0: red = brightness;   green = cc;             blue = aa;  break;
					case 1: red = bb;           green = brightness;     blue = aa;  break;
					case 2: red = aa;           green = brightness;     blue = cc;  break;
					case 3: red = aa;           green = bb;             blue = brightness; break;
					case 4: red = cc;           green = aa;             blue = brightness; break;
					case 5: red = brightness;   green = aa;             blue = bb;  break;
					default: red = 0;           green = 0;              blue = 0;   break;
				}
			}
			
			return ((alpha > 1 ? (uint)alpha : (uint)(alpha * 255)) & 0xFF) << 24 | (uint)(red * 255) << 16 | (uint)(green*255) << 8 | (uint)(blue*255);
        }

        /// <summary>
        /// Loads an array with the RGBA values of a Flash <code>uint</code> color.
        /// RGB values are stored 0-255.  Alpha is stored as a floating point number between 0 and 1.
        /// </summary>
        /// <param name="color">The color you want to break into components.</param>
        /// <param name="result">Results	An optional parameter, allows you to use an array that already exists in memory to store the result.</param>
        /// <returns>An <code>Array</code> object containing the Red, Green, Blue and Alpha values of the given color.</returns>
        public static Array getRGBA(uint color, Array result = null)
        {
            if (result != null)
            {
                throw new NotSupportedException();
            }

            var colorList = new List<float>();
            colorList[0] = (color >> 16) & 0xFF;
            colorList[1] = (color >> 8) & 0xFF;
            colorList[2] = color & 0xFF;
            colorList[3] = (float)((color >> 24) & 0xFF) / 255;

            return colorList.ToArray();
        }


        /// <summary>
        /// Loads an array with the HSB values of a Flash <code>uint</code> color.
        /// Hue is a value between 0 and 360.  Saturation, Brightness and Alpha
        /// are as floating point numbers between 0 and 1.
        /// </summary>
        /// <param name="color">The color you want to break into components.</param>
        /// <param name="result">An optional parameter, allows you to use an array that already exists in memory to store the result.</param>
        /// <returns>An <code>Array</code> object containing the Red, Green, Blue and Alpha values of the given color.</returns>
        public static Array getHSB(uint color, Array result = null)
        {
            throw new NotSupportedException("CurrentlyUnusedAndWillLaterBeReplacedBySomeXNAColorSpecificStuff");
        }

        /// <summary>
        /// Format seconds as minutes with a colon, an optionally with milliseconds too.
        /// </summary>
        /// <param name="seconds">The number of seconds (for example, time remaining, time spent, etc).</param>
        /// <param name="showMS">Whether to show milliseconds after a "." as well.  Default value is false.</param>
        /// <returns>A nicely formatted <code>String</code>, like "1:03".</returns>
        public static string formatTime(ulong seconds, bool showMS = false)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);

            string formatted = string.Format("{0}{1}{2}{3}",
                timeSpan.Duration().Hours > 0 ? string.Format("{0:0} h, ", timeSpan.Hours) : string.Empty,
                timeSpan.Duration().Minutes > 0 ? string.Format("{0:0} m, ", timeSpan.Minutes) : string.Empty,
                timeSpan.Duration().Seconds > 0 ? string.Format("{0:0} s, ", timeSpan.Seconds) : string.Empty,
                timeSpan.Duration().Milliseconds > 0 && showMS ? string.Format("{0:0} ms", timeSpan.Seconds) : string.Empty);

            if (formatted.EndsWith(", ")) formatted = formatted.Substring(0, formatted.Length - 2);
            if (string.IsNullOrEmpty(formatted)) formatted = "0 seconds";

            return formatted;

            /*
			var timeString:String = int(Seconds/60) + ":";
			var timeStringHelper:int = int(Seconds)%60;
			if(timeStringHelper < 10)
				timeString += "0";
			timeString += timeStringHelper;
			if(ShowMS)
			{
				timeString += ".";
				timeStringHelper = (Seconds-int(Seconds))*100;
				if(timeStringHelper < 10)
					timeString += "0";
				timeString += timeStringHelper;
			}
			return timeString;
            */
        }

        /// <summary>
        /// Generate a comma-separated string from an array.
        /// Especially useful for tracing or other debug output.
        /// </summary>
        /// <param name="anyArray">Any <code>Array</code> object.</param>
        /// <returns>A comma-separated <code>String</code> containing the <code>.toString()</code> output of each element in the array.</returns>
        public static string formatArray(Array anyArray)
        {
            if ((anyArray == null) || (anyArray.Length == 0))
            {
                return string.Empty;                
            }

            var strBuilder = new StringBuilder();

            strBuilder.Append(anyArray.GetValue(0).ToString());
            for (int i = 1; i < anyArray.Length; ++i)
            {
                strBuilder.Append(", ");
                strBuilder.Append(anyArray.GetValue(i).ToString());
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// Automatically commas and decimals in the right places for displaying money amounts.
        /// Does not include a dollar sign or anything, so doesn't really do much
        /// if you call say <code>String results = FlxU.formatMoney(10,false);</code>
        /// However, very handy for displaying large sums or decimal money values.
        /// </summary>
        /// <param name="amount">How much moneys (in dollars, or the equivalent "main" currency - i.e. not cents).</param>
        /// <param name="showDecimal">Whether to show the decimals/cents component. Default value is true.</param>
        /// <param name="englishStyle">Major quantities (thousands, millions, etc) separated by commas, and decimal by a period. Default value is true.</param>
        /// <returns>A nicely formatted <code>String</code>. Does not include a dollar sign or anything!</returns>
        public static string formatMoney(float amount, bool showDecimal = true, bool englishStyle = true)
        {
            throw new NotImplementedException();

            /*
			var helper:int;
			var amount:int = Amount;
			var string:String = "";
			var comma:String = "";
			var zeroes:String = "";
			while(amount > 0)
			{
				if((string.length > 0) && comma.length <= 0)
				{
					if(EnglishStyle)
						comma = ",";
					else
						comma = ".";
				}
				zeroes = "";
				helper = amount - int(amount/1000)*1000;
				amount /= 1000;
				if(amount > 0)
				{
					if(helper < 100)
						zeroes += "0";
					if(helper < 10)
						zeroes += "0";
				}
				string = zeroes + helper + comma + string;
			}
			if(ShowDecimal)
			{
				amount = int(Amount*100)-(int(Amount)*100);
				string += (EnglishStyle?".":",") + amount;
				if(amount < 10)
					string += "0";
			}
			return string;
            */
        }

        /// <summary>
        /// Get the <code>String</code> name of any <code>Object</code>.
        /// </summary>
        /// <param name="obj">The <code>Object</code> object in question.</param>
        /// <param name="simple">Returns only the class name, not the package or packages.</param>
        /// <returns>The name of the <code>Class</code> as a <code>String</code> object.</returns>
        public static string getClassName(object obj, bool simple = false)
        {
            if (simple)
            {
                throw new NotSupportedException();
            }

            return obj.GetType().FullName;

            /*
			var string:String = getQualifiedClassName(Obj);
			string = string.replace("::",".");
			if(Simple)
				string = string.substr(string.lastIndexOf(".")+1);
			return string;
            */
        }

        /// <summary>
        /// Check to see if two objects have the same class name.
        /// </summary>
        /// <param name="object1">The first object you want to check.</param>
        /// <param name="object2">The second object you want to check.</param>
        /// <returns>Whether they have the same class name or not.</returns>
        public static bool compareClassNames(object object1, object object2)
        {
            throw new NotImplementedException();

            /*
			return getQualifiedClassName(Object1) == getQualifiedClassName(Object2);
            */
        }

        /// <summary>
        /// Look up a <code>Class</code> object by its string name.
        /// </summary>
        /// <param name="name">The <code>String</code> name of the <code>Class</code> you are interested in.</param>
        /// <returns>A <code>Class</code> object.</returns>
        public static object getClass(string name)
        {
            throw new NotImplementedException();

            /*
            return getDefinitionByName(Name) as Class;
            */
        }

        /// <summary>
        /// A tween-like function that takes a starting velocity
        /// and some other factors and returns an altered velocity.
        /// </summary>
        /// <param name="velocity">Any component of velocity (e.g. 20).</param>
        /// <param name="acceleration">Rate at which the velocity is changing.</param>
        /// <param name="drag">Really kind of a deceleration, this is how much the velocity changes if Acceleration is not set.</param>
        /// <param name="max">An absolute value cap for the velocity.</param>
        /// <returns>The altered Velocity value.</returns>
        static public float computeVelocity(float velocity, float acceleration = 0, float drag = 0, float max = 10000.0f)
        {
            if (acceleration != 0)
            {
                velocity += acceleration * FlxG.elapsed;                
            }
            else if (drag != 0)
            {
                float deltaDrag = drag * FlxG.elapsed;

                if (velocity - deltaDrag > 0)
                {
                    velocity = velocity - deltaDrag;                    
                }
                else if (velocity + deltaDrag < 0)
                {
                    velocity += deltaDrag;
                }
                else
                {
                    velocity = 0;                    
                }
            }

            if ((velocity != 0) && (max != 10000))
            {
                if (velocity > max)
                {
                    velocity = max;                    
                }
                else if (velocity < -max)
                {
                    velocity = -max;                    
                }
            }

            return velocity;
        }

        //*** NOTE: THESE LAST THREE FUNCTIONS REQUIRE FLXPOINT ***//

        /// <summary>
        /// Rotates a point in 2D space around another point by the given angle.
        /// </summary>
        /// <param name="x">The X coordinate of the point you want to rotate.</param>
        /// <param name="y">The Y coordinate of the point you want to rotate.</param>
        /// <param name="pivotX">The X coordinate of the point you want to rotate around.</param>
        /// <param name="pivotY">The Y coordinate of the point you want to rotate around.</param>
        /// <param name="angle">Rotate the point by this many degrees.</param>
        /// <param name="point">Optional <code>FlxPoint</code> to store the results in.</param>
        /// <returns>A <code>FlxPoint</code> containing the coordinates of the rotated point.</returns>
        static public FlxPoint rotatePoint(float x, float y, float pivotX, float pivotY, float angle, FlxPoint point = null)
        {
            float sin = 0;
            float cos = 0;
            float radians = angle * -0.017453293f;

            while (radians < -3.14159265)
            {
                radians += 6.28318531f;                
            }

            while (radians > 3.14159265)
            {
                radians = radians - 6.28318531f;                
            }

            if (radians < 0)
            {
                sin = 1.27323954f * radians + .405284735f * radians * radians;

                if (sin < 0)
                {
                    sin = .225f * (sin * -sin - sin) + sin;
                }
                else
                {
                    sin = .225f * (sin * sin - sin) + sin;
                }
            }
            else
            {
                sin = 1.27323954f * radians - 0.405284735f * radians * radians;

                if (sin < 0)
                {
                    sin = .225f * (sin * -sin - sin) + sin;
                }
                else
                {
                    sin = .225f * (sin * sin - sin) + sin;
                }
            }

            radians += 1.57079632f;
            if (radians > 3.14159265)
            {
                radians = radians - 6.28318531f;                
            }

            if (radians < 0)
            {
                cos = 1.27323954f * radians + 0.405284735f * radians * radians;

                if (cos < 0)
                {
                    cos = .225f * (cos * -cos - cos) + cos;
                }
                else
                {
                    cos = .225f * (cos * cos - cos) + cos;
                }
            }
            else
            {
                cos = 1.27323954f * radians - 0.405284735f * radians * radians;

                if (cos < 0)
                {
                    cos = .225f * (cos * -cos - cos) + cos;
                }
                else
                {
                    cos = .225f * (cos * cos - cos) + cos;
                }
            }

            float dx = x - pivotX;
            float dy = pivotY + y; // Y axis is inverted in flash, normally this would be a subtract operation // flx# - Hmmmmm...
            if (point == null)
            {
                point = new FlxPoint();                
            }
            point.X = pivotX + cos * dx - sin * dy;
            point.Y = pivotY - sin * dx - cos * dy;
            return point;
        }

        /// <summary>
        /// Calculates the angle between two points. 0 degrees points straight up.
        /// </summary>
        /// <param name="point1">The X coordinate of the point.</param>
        /// <param name="point2">The Y coordinate of the point.</param>
        /// <returns>The angle in degrees, between -180 and 180.</returns>
        internal static float getAngle(FlxPoint point1, FlxPoint point2)
        {
            float x = point2.X - point1.X;
            float y = point2.Y - point1.Y;

            if ((x == 0) && (y == 0))
            {
                return 0;                
            }

            float c1 = 3.14159265f * 0.25f;
            float c2 = 3 * c1;
            float ay = (y < 0) ? -y : y;
            float angle = 0;
            
            if (x >= 0)
            {
                angle = c1 - c1 * ((x - ay) / (x + ay));
            }
            else
            {
                angle = c2 - c1 * ((x + ay) / (ay - x));
            }

            angle = ((y < 0) ? -angle : angle) * 57.2957796f;

            if (angle > 90)
            {
                angle = angle - 270;
            }
            else
            {
                angle += 90;                
            }

            return angle;
        }

        /// <summary>
        /// Calculate the distance between two points.
        /// </summary>
        /// <param name="point1">A <code>FlxPoint</code> object referring to the first location.</param>
        /// <param name="point2">A <code>FlxPoint</code> object referring to the second location.</param>
        /// <returns>The distance between the two points as a floating point <code>Number</code> object.</returns>
        internal static float getDistance(FlxPoint point1, FlxPoint point2)
        {
            float dx = point1.X - point2.X;
            float dy = point1.Y - point2.Y;
            return (float) Math.Sqrt(dx * dx + dy * dy);
        }

        /*
        /// <summary>
        /// flx# - A more precise angle between two FlxPoints???
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
        */
    }
}
