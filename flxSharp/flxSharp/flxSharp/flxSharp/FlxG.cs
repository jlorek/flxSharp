using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using flxSharp.flxSharp;

namespace fliXNA_xbox
{
    public class FlxG
    {
        /// <summary>
        /// Reference to the ContentManager, important for retrieving Content such as images and sound
        /// </summary>
        static public ContentManager content;

        /// <summary>
        /// Array of viewports, could be used for splitscreen
        /// </summary>
        static public List<Viewport> viewports;

        /// <summary>
        /// The current viewport
        /// </summary>
        static public Viewport viewport;

        /// <summary>
        /// A viewport that references the whole screen, used to render the HUD
        /// </summary>
        static public Viewport defaultWholeScreenViewport;

        /// <summary>
        /// Array of cameras, used for splitscreen
        /// </summary>
        static public List<FlxCamera> cameras;

        /// <summary>
        /// Reference to a storage device, perhaps the harddrive on the Xbox?
        /// Not yet implemented
        /// </summary>
        static public StorageDevice storage;

        /// <summary>
        /// Reference to the GraphicsDeviceManager
        /// </summary>
        static public GraphicsDeviceManager graphics;

        /// <summary>
        /// Reference to the GraphicsDevice
        /// </summary>
        static public GraphicsDevice graphicsDevice;

        /// <summary>
        /// Time elapsed since last frame
        /// </summary>
        static public float elapsed;

        /// <summary>
        /// Reference to the SpriteBatch
        /// </summary>
        static internal SpriteBatch spriteBatch;

        /// <summary>
        /// Reference to Keyboard input
        /// </summary>
        static public FlxKeyboard keys;

        /// <summary>
        /// Reference to Mouse input
        /// </summary>
        static public FlxMouse mouse;

        /// <summary>
        /// Reference to Gamepad input for Player One
        /// </summary>
        static public FlxGamepad pad1;

        /// <summary>
        /// Reference to Gamepad input for Player Two
        /// </summary>
        static public FlxGamepad pad2;

        /// <summary>
        /// Reference to Gamepad input for Player Three
        /// </summary>
        static public FlxGamepad pad3;

        /// <summary>
        /// Reference to Gamepad input for Player Four
        /// </summary>
        static public FlxGamepad pad4;

        /// <summary>
        /// Internal reference to keep track of current state
        /// </summary>
        static internal FlxState state;

        /// <summary>
        /// Unused Console reference
        /// Not yet implemented
        /// </summary>
        static public FlxConsole console;

        /// <summary>
        /// Controls the background color of the Game
        /// </summary>
        static public Color bgColor;

        /// <summary>
        /// Reference to the Game Camera
        /// </summary>
        static public FlxCamera camera;

        /// <summary>
        /// Internal reference to XNA's GameTime - useful for getting time stuff in between frames.
        /// Allows you more control than FlxG.elapsed
        /// </summary>
        static private GameTime gameTime;

        /// <summary>
        /// The default font that you can use for text
        /// </summary>
        static public SpriteFont defaultFont;

        /// <summary>
        /// Internal helper for FlxQuadtree to calculate collisions
        /// </summary>
        static internal FlxRect worldBounds;

        /// <summary>
        /// Internal helper for FlxQuadtree to calculate collisions
        /// </summary>
        static internal uint worldDivisions = 6;

        /// <summary>
        /// Can be used to change volume of all sounds, default is 1.0f which equals 100%
        /// </summary>
        static public float volume = 1.0f;

        /// <summary>
        /// A list of all the sounds being played in the game
        /// </summary>
        static public FlxGroup sounds;

        /// <summary>
        /// Whether all the sounds are mute
        /// </summary>
        static public bool mute;
        
        /// <summary>
        /// Reference to the Width of the Viewport
        /// </summary>
        static public float width;

        /// <summary>
        /// Reference to the Height of the Viewport
        /// </summary>
        static public float height;

        /// <summary>
        /// Internal random number helper
        /// </summary>
        static public float globalSeed = FlxU.random();

        /// <summary>
        /// Can be used to zoom the camera
        /// </summary>
        static public float zoom;

        /// <summary>
        /// Can be used to rotate the camera
        /// </summary>
        static public float rotation;

        /// <summary>
        /// Built-In FlxGroup to handle HUD elements since scrollFactor is not yet working
        /// </summary>
        static public FlxGroup hud;

        /// <summary>
        /// Reference of the Safe Zone, useful for making sure your objects are visible across various televisions
        /// </summary>
        static public FlxRect safeZone;

        /// <summary>
        /// Call this to switch to a new state
        /// @use  FlxG.switchState(new NewState());
        /// </summary>
        /// <param name="State"></param>
        static public void switchState(FlxState State)
        {
            state.destroy();
            state = State;
            state.create();
        }

        /// <summary>
        /// Internal function for keeping input states current
        /// </summary>
        static internal void updateInputs()
        {
            pad1.update();
            pad2.update();
            pad3.update();
            pad4.update();
            keys.update();
            mouse.update();
        }

        /// <summary>
        /// Internal function for updating Camera, State, Console, and Elapsed Time
        /// </summary>
        /// <param name="dt"></param>
        static internal void update(GameTime dt)
        {
            gameTime = dt;
            elapsed = (float)dt.ElapsedGameTime.TotalSeconds;
            state.update();
            updateCameras();
            sounds.update();
            updateInputs();
        }

        static internal void gametime(GameTime dt)
        {
            gameTime = dt;
            elapsed = (float)dt.ElapsedGameTime.TotalSeconds;
        }

        static internal void updateCameras()
        {
            foreach (FlxCamera c in FlxG.cameras)
            {
                if ((c != null) && c.Exists)
                {
                    if (c.Active)
                        c.update();
                }
            }
        }

        static public FlxCamera addCamera(FlxCamera NewCamera)
        {
            FlxG.camera = NewCamera;
            FlxG.cameras.Add(NewCamera);
            Viewport v = new Viewport((int)NewCamera.x, (int)NewCamera.y, (int)NewCamera.width, (int)NewCamera.height);
            //FlxG.log("camera is at x: " + NewCamera.x + " y: " + NewCamera.y + " width: " + NewCamera.width + " height " + NewCamera.height);
            //FlxG.log("camera count: " + FlxG.cameras.Count);
            FlxG.viewports.Add(v);
            return NewCamera;
        }

        /// <summary>
        /// Shake the screen
        /// </summary>
        /// <param name="Intensity"></param>
        /// <param name="Duration"></param>
        /// <param name="OnComplete"></param>
        /// <param name="Force"></param>
        /// <param name="Direction"></param>
        static internal void shake(float Intensity = 0.025f, float Duration = 0.5f, Action OnComplete = null, bool Force = true, uint Direction = 0)
        {
            FlxG.camera.shake(Intensity, Duration, OnComplete, Force, Direction);
        }

        /// <summary>
        /// Internal random float from 0 to 1
        /// </summary>
        /// <returns></returns>
        static internal float random()
        {
            return globalSeed = FlxU.random();
        }


        /// <summary>
        /// Log something to the debugger console in Visual Studio
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        static public bool log(Object data)
        {
            System.Diagnostics.Debug.WriteLine(data.ToString());
            return true;
        }

        /// <summary>
        /// Not yet implemented - will be used to draw bounding boxes around sprites
        /// </summary>
        public static bool visualDebug { get; set; }

        /// <summary>
        /// Returns true if the two FlxObjects overlap.  Optional Callback function of return type bool with two FlxObject parameters will be called if true.
        /// </summary>
        /// <param name="ObjectOrGroup1"></param>
        /// <param name="ObjectOrGroup2"></param>
        /// <param name="NotifyCallback"></param>
        /// <param name="ProcessCallback"></param>
        /// <returns></returns>
        static public Boolean overlap(FlxObject ObjectOrGroup1 = null, FlxObject ObjectOrGroup2 = null, Func<FlxObject, FlxObject, Boolean> NotifyCallback = null, Func<FlxObject, FlxObject, Boolean> ProcessCallback = null)
		{
			if(ObjectOrGroup1 == null)
				ObjectOrGroup1 = FlxG.state;
			if(ObjectOrGroup2 == ObjectOrGroup1)
				ObjectOrGroup2 = null;
			FlxQuadTree.divisions = FlxG.worldDivisions;
			FlxQuadTree quadTree = new FlxQuadTree(FlxG.worldBounds.x,FlxG.worldBounds.y,FlxG.worldBounds.width,FlxG.worldBounds.height);
			quadTree.load(ObjectOrGroup1,ObjectOrGroup2,NotifyCallback,ProcessCallback);
            Boolean result = quadTree.execute();
			quadTree.destroy();
			return result;
		}
        /// <summary>
        /// Returns true if the two FlxObjects collide.  Optional callback function of return type bool with two FlxObject parameters will be called if true.
        /// </summary>
        /// <param name="ObjectOrGroup1"></param>
        /// <param name="ObjectOrGroup2"></param>
        /// <param name="NotifyCallback"></param>
        /// <returns></returns>
        static public Boolean collide(FlxObject ObjectOrGroup1 = null, FlxObject ObjectOrGroup2 = null, Func<FlxObject, FlxObject, Boolean> NotifyCallback = null)
		{
			return overlap(ObjectOrGroup1,ObjectOrGroup2,NotifyCallback,FlxObject.separate);
		}

        /// <summary>
        /// Play a sound, not with FlxSound but with SoundEffect from the XNA framework because the sound is disposed of
        /// properly by the xbox itself.
        /// 
        /// CURRENTLY NOT WORKING
        /// 
        /// The sound starts but cuts off in about half of a second.
        /// </summary>
        /// <param name="EmbeddedSound"></param>
        /// <param name="Volume"></param>
        /// <param name="Looped"></param>
        /// <returns></returns>
        static public FlxSound play(SoundEffect EmbeddedSound, float Volume = 1.0f, bool Looped = false)
        {
            return FlxG.loadSound(EmbeddedSound, Volume, Looped, (Looped)?false:true, true);
        }

        static private FlxSound loadSound(SoundEffect EmbeddedSound, float Volume, bool Looped, bool AutoDestroy, bool AutoPlay)
        {
            if (EmbeddedSound == null)
            {
                FlxG.log("WARNING: FlxG.loadSound() requires an embedded sound to work.");
                return null;
            }
            FlxSound sound = new FlxSound();
            if (EmbeddedSound != null)
                sound.loadEmbedded(EmbeddedSound, Looped, AutoDestroy);
            sound.volume = Volume;
            if (AutoPlay)
                sound.play();
            return sound;
        }

        internal static float snap(float XorY, float Grid)
		{
            float coord = XorY;
            if (coord % Grid > 0)
                coord -= (coord % Grid);
            return coord;
		}

        /// <summary>
        /// Initiate all the things needed by the engine
        /// </summary>
        internal static void init()
        {
            FlxG.defaultWholeScreenViewport = FlxG.viewport;
            FlxG.cameras = new List<FlxCamera>();
            FlxG.viewports = new List<Viewport>();
            //FlxG.width = FlxG.graphics.PreferredBackBufferWidth;
            //FlxG.height = FlxG.graphics.PreferredBackBufferHeight;
            FlxG.bgColor = Color.Black;
            FlxG.mute = false;
            FlxG.sounds = new FlxGroup();
            FlxG.console = new FlxConsole();
            FlxG.worldBounds = new FlxRect();
            FlxG.defaultFont = FlxG.content.Load<SpriteFont>("ConsoleFont");
            //FlxG.zoom = 1f;
            FlxG.rotation = 0f;
            FlxG.pad1 = new FlxGamepad(PlayerIndex.One);
            FlxG.pad2 = new FlxGamepad(PlayerIndex.Two);
            FlxG.pad3 = new FlxGamepad(PlayerIndex.Three);
            FlxG.pad4 = new FlxGamepad(PlayerIndex.Four);
            FlxG.keys = new FlxKeyboard();
            FlxG.mouse = new FlxMouse();
            FlxG.safeZone = new FlxRect(FlxG.graphicsDevice.Viewport.TitleSafeArea.X, FlxG.graphicsDevice.Viewport.TitleSafeArea.Y, FlxG.graphicsDevice.Viewport.TitleSafeArea.Width, FlxG.graphicsDevice.Viewport.TitleSafeArea.Height);
            FlxCamera defaultCam = new FlxCamera(0, 0, FlxG.width, FlxG.height, FlxG.zoom);
            FlxG.addCamera(defaultCam);

            //Thread tCreate = new Thread(FlxG.state.create);
            //tCreate.Start();

            //create state last
            FlxG.state.create();
        }
    }
}
