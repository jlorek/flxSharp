using System;

namespace flxSharp.flxSharp.System
{
    /// <summary>
    /// Just a helper structure for the FlxSprite animation system.
    /// </summary>
    public class FlxAnim
    {
        /// <summary>
        /// String name of the animation (e.g. "walk").
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Seconds between frames (basically the framerate).
        /// </summary>
        public float Delay { get; set; }
        
        /// <summary>
        /// A list of frames stored as <code>uint</code> objects.
        /// </summary>
        public int[] Frames { get; set; }
        
        /// <summary>
        /// Whether or not the animation is looped.
        /// </summary>
        public Boolean Looped { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">What this animation should be called (e.g. "run").</param>
        /// <param name="frames">An array of numbers indicating what frames to play in what order (e.g. 0, 1, 2, 3).</param>
        /// <param name="frameRate">The speed in frames per second that the animation should play at (e.g. 40).</param>
        /// <param name="looped">Whether or not the animation is looped or just plays once</param>
        public FlxAnim(String name, int[] frames, float frameRate, Boolean looped)
        {
            Name = name;
            
            Delay = 0;
            if (frameRate > 0)
            {
                Delay = 1.0f / frameRate;                
            }
            
            Frames = frames;
            Looped = looped;
        }

        /// <summary>
        /// Clean up memory.
        /// </summary>
        public void destroy()
        {
            Frames = null;
        }
    }
}
