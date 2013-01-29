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
    public class FlxState : FlxGroup
    {
        /// <summary>
        /// Internal HUD FlxGroup.
        /// Needed because scrollFactor currently does not work so if you want things to stick to the screen
        /// add them to this.
        /// </summary>
        protected FlxGroup hud;

        /// <summary>
        /// Constructs a new state
        /// </summary>
        public FlxState() : base() { }

        /// <summary>
        /// Override create() and initialize your game objects here
        /// </summary>
        public virtual void create()
        {
            hud = new FlxGroup();
            add(hud);    
        }

        /// <summary>
        /// Override this only if you are familiar with SpriteBatches and Viewports.  Otherwise leave it alone to avoid
        /// any issues with rendering.
        /// </summary>
        public override void draw()
        {
            // first clear the screen with the background color of your choice
            FlxG.graphicsDevice.Clear(FlxG.bgColor);

            // then loop through all the cameras and viewports, and render them to the screen
            int i;
            int l = FlxG.cameras.Count;
            for (i = 0; i < l; i++) //(l>1)?1:
            {
                FlxG.graphicsDevice.Viewport = FlxG.viewports[i];     //viewport must be selected BEFORE spritebatch drawing
                FlxG.spriteBatch.Begin(SpriteSortMode.Immediate,
                                            BlendState.AlphaBlend,
                                            SamplerState.PointClamp,  //PointClamp makes sure that the tiles render properly without tearing
                                            null,
                                            null,
                                            null,
                                            FlxG.cameras[i].TransformMatrix);
                base.draw();
                FlxG.spriteBatch.End();
            }
            
            // switch to the whole screen in order to draw the HUD
            FlxG.graphicsDevice.Viewport = FlxG.defaultWholeScreenViewport;

            // draw the HUD
            FlxG.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            hud.draw();
            FlxG.spriteBatch.End();
        }
    }
}
