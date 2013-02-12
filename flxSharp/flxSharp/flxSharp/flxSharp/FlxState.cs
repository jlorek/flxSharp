using Microsoft.Xna.Framework.Graphics;

namespace flxSharp.flxSharp
{
    /// <summary>
    /// This is the basic game "state" object - e.g. in a simple game
    /// you might have a menu state and a play state.
    /// It is for all intents and purpose a fancy FlxGroup.
    /// And really, it's not even that fancy.
    /// </summary>
    public class FlxState : FlxGroup
    {
        /// <summary>
        /// This function is called after the game engine successfully switches states.
        /// Override this function, NOT the constructor, to initialize or set up your game state.
        /// We do NOT recommend overriding the constructor, unless you want some crazy unpredictable things to happen!
        /// </summary>
        public virtual void create()
        {

        }

        /*
        /// <summary>
        /// Override this only if you are familiar with SpriteBatches and Viewports.  Otherwise leave it alone to avoid
        /// any issues with rendering.
        /// </summary>
        public override void draw()
        {
            // xnaFlixel
            //we don't need no new-fangled pixel processing
            //in our retro engine!
            //FlxG.graphics.PreferMultiSampling = false;

            // first clear the screen with the background color of your choice
            FlxS.GraphicsDevice.Clear(FlxG.bgColor);

            // then loop through all the cameras and viewports, and render them to the screen
            int i;
            int l = FlxG.cameras.Count;
            for (i = 0; i < l; i++) //(l>1)?1:
            {
                FlxS.GraphicsDevice.Viewport = FlxS.Viewports[i];     //viewport must be selected BEFORE spritebatch drawing

                FlxS.SpriteBatch.Begin(SpriteSortMode.Immediate,
                                       BlendState.AlphaBlend,
                                       SamplerState.PointClamp,  //PointClamp makes sure that the tiles render properly without tearing
                                       null,
                                       null,
                                       null,
                                       FlxG.cameras[i].FxMatrix);
                base.draw();
                
                FlxS.SpriteBatch.End();
            }
        }
        */
    }
}
