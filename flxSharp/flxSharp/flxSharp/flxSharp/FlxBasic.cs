using System;
using System.Collections.Generic;

namespace flxSharp.flxSharp
{
    /// <summary>
    /// This is a useful "generic" Flixel object.
    /// Both <code>FlxObject</code> and <code>FlxGroup</code> extend this class,
    /// as do the plugins.  Has no size, position or graphical data.
    /// </summary>
    public class FlxBasic
    {
        static internal uint activeCount;

		static internal uint visibleCount;

        /// <summary>
        /// IDs seem like they could be pretty useful, huh?
        /// They're not actually used for anything yet though.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Controls whether <code>update()</code> and <code>draw()</code> are automatically called by FlxState/FlxGroup.
        /// </summary>
        public Boolean Exists { get; set; }

        /// <summary>
        /// Controls whether <code>update()</code> is automatically called by FlxState/FlxGroup.
        /// </summary>
        public Boolean Active { get; set; }

        /// <summary>
        /// Controls whether <code>draw()</code> is automatically called by FlxState/FlxGroup.
        /// </summary>
        public Boolean Visible { get; set; }

        /// <summary>
        /// Useful state for many game objects - "dead" (!alive) vs alive.
        /// <code>kill()</code> and <code>revive()</code> both flip this switch (along with exists, but you can override that).
        /// </summary>
        public Boolean Alive { get; set; }

        /// <summary>
        /// An array of camera objects that this object will use during <code>draw()</code>.
        /// This value will initialize itself during the first draw to automatically
        /// point at the main camera list out in <code>FlxG</code> unless you already set it.
        /// You can also change it afterward too, very flexible!
        /// </summary>
        public List<FlxCamera> Cameras;

        /// <summary>
        /// Setting this to true will prevent the object from appearing
        /// when the visual debug mode in the debugger overlay is toggled on.
        /// </summary>
        public Boolean IgnoreDrawDebug { get; set; }

        /// <summary>
        /// flx# only - Internal position reference, safest if left untouched!
        /// </summary>
        public FlxPoint Position;

        /// <summary>
        /// Instantiate the basic flixel object.
        /// </summary>
        public FlxBasic()
        {
            ID = -1;
            Exists = true;
            Active = true;
            Visible = true;
            Alive = true;
            IgnoreDrawDebug = false;

            Position = new FlxPoint();
        }

        /// <summary>
        /// Override this function to null out variables or manually call
        /// <code>destroy()</code> on class members if necessary.
        /// Don't forget to call <code>super.destroy()</code>!
        /// </summary>
        public virtual void destroy()
        {
            Position = null;
        }

        /// <summary>
        /// Pre-update is called right before <code>update()</code> on each object in the game loop.
        /// </summary>
        public virtual void preUpdate()
        {
            activeCount++;
        }

        /// <summary>
        /// Override this function to update your class's position and appearance.
        /// This is where most of your game rules and behavioral code will go.
        /// </summary>
        public virtual void update()
        { }

        /// <summary>
        /// Post-update is called right after <code>update()</code> on each object in the game loop.
        /// </summary>
        public virtual void postUpdate()
        { }

        /// <summary>
        /// Override this function to control how the object is drawn.
        /// Overriding <code>draw()</code> is rarely necessary, but can be very useful.
        /// </summary>
        public virtual void draw()
        {
            if (Cameras == null)
            {
                Cameras = FlxG.cameras;                
            }

            foreach (FlxCamera camera in Cameras)
            {
                visibleCount++;

                if (FlxG.visualDebug && !IgnoreDrawDebug)
                {
                    drawDebug(camera);
                }
            }
        }

        /// <summary>
        /// Override this function to draw custom "debug mode" graphics to the
        /// specified camera while the debugger's visual mode is toggled on.
        /// </summary>
        /// <param name="camera">Which camera to draw the debug visuals to.</param>
        public virtual void drawDebug(FlxCamera camera = null)
        { }

        /// <summary>
        /// Handy function for "killing" game objects.
        /// Default behavior is to flag them as nonexistent AND dead.
        /// However, if you want the "corpse" to remain in the game,
        /// like to animate an effect or whatever, you should override this,
        /// setting only alive to false, and leaving exists true.
        /// </summary>
        public virtual void kill()
		{
			Alive = false;
			Exists = false;
		}

        /// <summary>
        /// Handy function for bringing game objects "back to life". Just sets alive and exists back to true.
        /// In practice, this function is most often called by <code>FlxObject.reset()</code>.
        /// </summary>
        public void revive()
		{
			Alive = true;
			Exists = true;
		}

        /// <summary>
        /// Convert object to readable string name.  Useful for debugging, save games, etc.
        /// </summary>
        /// <returns></returns>
        public string toString()
        {
            return this.GetType().FullName;
        }

    }
}
