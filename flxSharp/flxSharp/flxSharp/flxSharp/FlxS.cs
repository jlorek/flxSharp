using Microsoft.Xna.Framework;

namespace flxSharp.flxSharp
{
    public static class FlxS
    {
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
