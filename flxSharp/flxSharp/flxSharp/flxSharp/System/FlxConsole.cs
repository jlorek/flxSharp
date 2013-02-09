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
    public class FlxConsole : FlxGroup
    {

        public FlxSprite bg;
        public FlxText text;
        
        public FlxConsole()
            : base()
        {
            bg = new FlxSprite(80, 40);
            bg.makeGraphic((uint)FlxS.GraphicsDevice.Viewport.Width - 160, (uint)FlxS.GraphicsDevice.Viewport.Height - 80, FlxColor.WHITE * 0.45f);
            bg.Alpha = 0.5f;
            add(bg);
            ScrollFactor.X = ScrollFactor.Y = 0;
            Visible = false;
            text = new FlxText(100, 60, FlxS.GraphicsDevice.Viewport.Width - 160, "internal console is a work in progress", FlxG.defaultFont);
            text.setFormat(FlxColor.WHITE);
            text.Alpha = 0.75f;
            add(text);
        }

        public override void update()
        {
            base.update();
            //if(FlxG.justPressed(Keys.OemTilde) || FlxG.justPressed(FlxG.pad1, Buttons.Back))
            //    visible = !visible;
            //text.text = "updates: "+FlxObject._ACTIVECOUNT+"\ndraws: "+FlxObject._VISIBLECOUNT;
        }
    }
}
