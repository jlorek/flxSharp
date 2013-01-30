using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace flxSharp.flxSharp
{
    public class FlxS
    {
        public static GraphicsDevice GraphicsDevice { get; set; }

        public static GraphicsDeviceManager GraphicsDeviceManager { get; set; }

        public static ContentManager ContentManager { get; set; }

        public static SpriteBatch SpriteBatch { get; set; }

        public static GameTime GameTime { get; set; }

        public static List<Viewport> Viewports { get; set; }

        public static RenderTarget2D RenderTarget { get; set; }

        static FlxS()
        {
            Viewports = new List<Viewport>();
        }

        public static Color UIntToColor(uint color)
        {
            byte a = (byte)(color >> 24);
            byte r = (byte)((color << 8) >> 24);
            byte g = (byte)((color << 16) >> 24);
            byte b = (byte)((color << 24) >> 24);

            return new Color(r, g, b, a);
        }
    }
}
