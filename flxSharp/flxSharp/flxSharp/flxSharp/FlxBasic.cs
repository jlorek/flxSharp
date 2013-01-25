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


namespace fliXNA_xbox
{
    public class FlxBasic
    {
        static internal uint _ACTIVECOUNT;
		static internal uint _VISIBLECOUNT;

        /// <summary>
        /// IDs seem useful, but don't actually do anything yet
        /// </summary>
        public int ID;

        /// <summary>
        /// Controls whether <code>update()</code> and <code>draw()</code> are automatically called
        /// </summary>
        public Boolean exists;

        /// <summary>
        /// Controls whether <code>update()</code>is automatically called
        /// </summary>
        public Boolean active;

        /// <summary>
        /// Controls whether <code>draw()</code> is automatically called
        /// </summary>
        public Boolean visible;

        /// <summary>
        /// Useful state for many game objects - "dead" (!alive) vs alive.
        /// <code>kill()</code> and <code>revive()</code> both flip this switch (along with exists, but you can override that.)
        /// </summary>
        public Boolean alive;

        /// <summary>
        /// Setting this to true will prevent objects from appearing when visual debug mode is on.  Not yet implemented
        /// </summary>
        public Boolean ignoreDrawDebug;

        /// <summary>
        /// Internal position reference, safest if left untouched!
        /// </summary>
        public FlxPoint position;

        public List<FlxCamera> cameras;

        public FlxBasic()
        {
            ID = -1;
            exists = true;
            active = true;
            visible = true;
            alive = true;
            ignoreDrawDebug = false;
            position = new FlxPoint();
        }

        /// <summary>
        /// Override this to null out variables or manually call <code>destroy()</code> on class members.  Dont forget to call <code>base.destroy()</code>
        /// </summary>
        public virtual void destroy()
        {
            position = null;
        }

        /// <summary>
        /// Pre-update is called right before <code>update()</code>
        /// </summary>
        public virtual void preUpdate()
        {
            _ACTIVECOUNT++;
        }

        /// <summary>
        /// Override this to update your class's position and appearance.  Most of your game rules and behavioral code will go here.
        /// </summary>
        public virtual void update()
        {
        }

        /// <summary>
        /// Post-update is called right after <code>update()</code>
        /// </summary>
        public virtual void postUpdate()
        {
        }

        /// <summary>
        /// Override this to control how object is drawn.  Refer to <example>SpriteBatch.Draw()</example> in MSDN documentation
        /// </summary>
        public virtual void draw()
        {
            if (cameras == null)
                cameras = FlxG.cameras;
            FlxCamera camera;// = FlxG.camera;
            int i = 0;
            int l = cameras.Count;
            while (i < l)
            {
                camera = cameras[(int)i++];
                //camera = FlxG.camera;
                _VISIBLECOUNT++;
                if (FlxG.visualDebug && !ignoreDrawDebug)
                    drawDebug(FlxG.camera);
            }
        }

        /// <summary>
        /// Debugging, not yet implemented
        /// </summary>
        /// <param name="camera"></param>
        public virtual void drawDebug(FlxCamera camera=null)
        {
        }

        /// <summary>
        /// Killing objects, alive and exists are switched to false
        /// </summary>
        public virtual void kill()
		{
			alive = false;
			exists = false;
		}

        /// <summary>
        /// Reviving objects, alive and exists are switched to true
        /// </summary>
        public void revive()
		{
			alive = true;
			exists = true;
		}

        /// <summary>
        /// Returns the name of this class
        /// </summary>
        /// <returns></returns>
        public string toString()
        {
            return this.ToString();
        }

    }
}
