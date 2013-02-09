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
using flxSharp.flxSharp;
using flxSharp.flxSharp.System;

namespace fliXNA_xbox
{
    public class FlxTileblock : FlxSprite
    {
        /// <summary>
        /// Creates a new rectangular FlxSprite object with specified position and size.
        /// Great for walls and floors.
        /// </summary>
        /// <param name="X">X position of the block</param>
        /// <param name="Y">Y position of the block</param>
        /// <param name="Width">Width of the block</param>
        /// <param name="Height">Height of the block</param>
        public FlxTileblock(float X, float Y, float Width, float Height)
            : base(X, Y)
        {
            makeGraphic((uint)Width, (uint)Height, FlxColor.WHITE);
            Active = false;
            Immovable = true;
        }
    }
}
