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
            bg.makeGraphic(FlxG.graphicsDevice.Viewport.Width - 160, FlxG.graphicsDevice.Viewport.Height - 80, FlxColor.WHITE * 0.45f);
            bg.alpha = 0.5f;
            add(bg);
            ScrollFactor.x = ScrollFactor.y = 0;
            Visible = false;
            text = new FlxText(100, 60, FlxG.graphicsDevice.Viewport.Width - 160, "internal console is a work in progress", FlxG.defaultFont);
            text.setFormat(FlxColor.WHITE);
            text.alpha = 0.75f;
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
