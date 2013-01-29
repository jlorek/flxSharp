using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using System;
using System.Collections.Generic;
using fliXNA_xbox;
using flxSharp.flxSharp.System;

namespace flxSharp.flxSharp
{
    /// <summary>
    /// This is a global helper class full of useful functions for audio,
    /// input, basic info, and the camera system among other things.
    /// Utilities for maths and color and things can be found in <code>FlxU</code>.
    /// <code>FlxG</code> is specifically for Flixel-specific properties.
    /// </summary>
    public class FlxG
    {
        /// <summary>
        /// If you build and maintain your own version of flixel,
        /// you can give it your own name here.
        /// </summary>
        public static readonly string LibraryName = "flxSharp";

        /// <summary>
        /// Assign a major version to your library.
        /// Appears before the decimal in the console.
        /// </summary>
        public static readonly uint LibraryMajorVersion = 2;

        /// <summary>
        /// Assign a minor version to your library.
        /// Appears after the decimal in the console.
        /// </summary>
        public static readonly uint LibraryMinorVersion = 55;

        /// <summary>
        /// Debugger overlay layout preset: Wide but low windows at the bottom of the screen.
        /// </summary>
        public static readonly uint DebuggerStandard = 0;

        /// <summary>
        /// Debugger overlay layout preset: Tiny windows in the screen corners.
        /// </summary>
        public static readonly uint DebuggerMicro = 1;

        /// <summary>
        /// Debugger overlay layout preset: Large windows taking up bottom half of screen.
        /// </summary>
        public static readonly uint DebuggerBig = 2;

        /// <summary>
        /// Debugger overlay layout preset: Wide but low windows at the top of the screen.
        /// </summary>
        public static readonly uint DebuggerTop = 3;

        /// <summary>
        /// Debugger overlay layout preset: Large windows taking up left third of screen.
        /// </summary>
        public static readonly uint DebuggerLeft = 4;

        /// <summary>
        /// Debugger overlay layout preset: Large windows taking up right third of screen.
        /// </summary>
        public static readonly uint DebuggerRight = 5;

        /// <summary>
        /// Some handy color presets.  Less glaring than pure RGB full values.
        /// Primarily used in the visual debugger mode for bounding box displays.
        /// Red is used to indicate an active, movable, solid object.
        /// </summary>
        public static readonly uint Red = 0xffff0012;

        /// <summary>
        /// Green is used to indicate solid but immovable objects.
        /// </summary>
        public static readonly uint Green = 0xff00f225;

        /// <summary>
        /// Blue is used to indicate non-solid objects.
        /// </summary>
        public static readonly uint Blue = 0xff0090e9;

        /// <summary>
        /// Pink is used to indicate objects that are only partially solid, like one-way platforms.
        /// </summary>
        public static readonly uint Pink = 0xfff01eff;

        /// <summary>
        /// White... for white stuff.
        /// </summary>
        public static readonly uint White = 0xffffffff;

        /// <summary>
        /// And black too.
        /// </summary>
        public static readonly uint Black = 0xff000000;

        /// <summary>
        /// Internal tracker for game object.
        /// </summary>
        static protected internal FlxGame _game;

        /// <summary>
        /// Handy shared variable for implementing your own pause behavior.
        /// </summary>
        static protected bool _pause;

        /// <summary>
        /// Whether you are running in Debug or Release mode.
        /// Set automatically by <code>FlxPreloader</code> during startup.
        /// </summary>
        static public bool debug;

        /// <summary>
        /// Represents the amount of time in seconds that passed since last frame.
        /// </summary>
        public static float elapsed;

        /// <summary>
        /// How fast or slow time should pass in the game; default is 1.0.
        /// </summary>
        static public float timeScale;

        /// <summary>
        /// The width of the screen in game pixels.
        /// </summary>
        public static int width;

        /// <summary>
        /// The height of the screen in game pixels.
        /// </summary>
        public static int height;

        /// <summary>
        /// The dimensions of the game world, used by the quad tree for collisions and overlap checks.
        /// </summary>
        static internal FlxRect worldBounds;

        /// <summary>
        /// How many times the quad tree should divide the world on each axis.
        /// Generally, sparse collisions can have fewer divisons,
        /// while denser collision activity usually profits from more.
        /// Default value is 6.
        /// </summary>
        static internal uint worldDivisions;

        /// <summary>
        /// Whether to show visual debug displays or not.
        /// Default = false.
        /// </summary>
        public static bool visualDebug;

        /// <summary>
        /// Setting this to true will disable/skip stuff that isn't necessary for mobile platforms like Android. [BETA]
        /// </summary>
        public static bool mobile
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// The global random number generator seed (for deterministic behavior in recordings and saves).
        /// </summary>
        public static float globalSeed;

        /// <summary>
        /// <code>FlxG.levels</code> and <code>FlxG.scores</code> are generic
        /// global variables that can be used for various cross-state stuff.
        /// </summary>
        static public Array levels;
        static public int level;
        static public Array scores;
        static public int score;

        /// <summary>
        /// <code>FlxG.saves</code> is a generic bucket for storing
        /// FlxSaves so you can access them whenever you want.
        /// </summary>
        static public Array saves;
        static public int save;

        /// <summary>
        /// A reference to a <code>FlxMouse</code> object. Important for input!
        /// </summary>
        static public FlxMouse mouse;

        /// <summary>
        /// A reference to a <code>FlxKeyboard</code> object. Important for input!
        /// </summary>
        static public FlxKeyboard keys;

        /// <summary>
        /// A handy container for a background music object.
        /// </summary>
        public static FlxSound music;

        /// <summary>
        /// A list of all the sounds being played in the game.
        /// </summary>
        public static FlxGroup sounds;

        /// <summary>
        /// Whether or not the game sounds are muted.
        /// </summary>
        public static bool mute;

        /// <summary>
        /// Internal volume level, used for global sound control.
        /// </summary>
        public static float volume;

        /// <summary>
        /// An array of <code>FlxCamera</code> objects that are used to draw stuff.
        /// By default flixel creates one camera the size of the screen.
        /// </summary>
        public static List<FlxCamera> cameras;

        /// <summary>
        /// By default this just refers to the first entry in the cameras array
        /// declared above, but you can do what you like with it.
        /// </summary>
        public static FlxCamera camera;

        /// <summary>
        /// Allows you to possibly slightly optimize the rendering process IF
        /// you are not doing any pre-processing in your game state's <code>draw()</code> call.
        /// @default false
        /// </summary>
        public static bool UseBufferLocking
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Internal helper variable for clearing the cameras each frame.
        /// </summary>
        protected static Rectangle _cameraRect
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// An array container for plugins.
        /// By default flixel uses a couple of plugins:
        /// DebugPathDisplay, and TimerManager.
        /// </summary>
        public static List<FlxBasic> plugins { get; protected set; }

        /// <summary>
        /// Set this hook to get a callback whenever the volume changes.
        /// Function should take the form <code>myVolumeHandler(float Volume)</code>.
        /// </summary>
        public static event Action volumeHandler;

        /**
		 * Useful helper objects for doing Flash-specific rendering.
		 * Primarily used for "debug visuals" like drawing bounding boxes directly to the screen buffer.
		 */
		//static public var flashGfxSprite:Sprite;
		//static public var flashGfx:Graphics;

        /**
		 * Internal storage system to prevent graphics from being used repeatedly in memory.
		 */
		//static protected var _cache:Object;

        /// <summary>
        /// For debug purpose.
        /// </summary>
        /// <returns>The library name including major and minor version.</returns>
        public static string getLibraryName()
        {
            return FlxG.LibraryName + " v" + FlxG.LibraryMajorVersion + "." + FlxG.LibraryMinorVersion;
        }

        /// <summary>
        /// Log data to the debugger.
        /// </summary>
        /// <param name="data">Anything you want to log to the console.</param>
        public static void log(Object data)
        {
            Debug.WriteLine(data.ToString());

            /*
            if((_game != null) && (_game._debugger != null))
				_game._debugger.log.add((Data == null)?"ERROR: null object":Data.toString());
            */
        }

        /// <summary>
        /// Add a variable to the watch list in the debugger.
        /// This lets you see the value of the variable all the time.
        /// </summary>
        /// <param name="anyObject">A reference to any object in your game, e.g. Player or Robot or this.</param>
        /// <param name="variableName">The name of the variable you want to watch, in quotes, as a string: e.g. "speed" or "health".</param>
        /// <param name="displayName">Optional, display your own string instead of the class name + variable name: e.g. "enemy count".</param>
        public static void watch(object anyObject, string variableName, string displayName = null)
        {
            throw new NotImplementedException("UseMSVSWatchForNow;)");

            /*
			if((_game != null) && (_game._debugger != null))
				_game._debugger.watch.add(AnyObject,VariableName,DisplayName);
            */
        }

        /// <summary>
        /// Remove a variable from the watch list in the debugger.
        /// Don't pass a Variable Name to remove all watched variables for the specified object.
        /// </summary>
        /// <param name="anyObject">A reference to any object in your game, e.g. Player or Robot or this.</param>
        /// <param name="variableName">The name of the variable you want to watch, in quotes, as a string: e.g. "speed" or "health".</param>
        /// <param name="displayName">Optional, display your own string instead of the class name + variable name: e.g. "enemy count".</param>
        public static void unwatch(object anyObject, string variableName)
        {
            throw new NotImplementedException("UseMSVSWatchForNow;)");

            /*
			if((_game != null) && (_game._debugger != null))
				_game._debugger.watch.add(AnyObject,VariableName,DisplayName);
            */
        }

        #region flx# Stuff
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
        /// Reference to the SpriteBatch
        /// </summary>
        static internal SpriteBatch spriteBatch;

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
        /// Internal reference to XNA's GameTime - useful for getting time stuff in between frames.
        /// Allows you more control than FlxG.elapsed
        /// </summary>
        static private GameTime gameTime;

        /// <summary>
        /// The default font that you can use for text
        /// </summary>
        static public SpriteFont defaultFont;

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
        #endregion // flx# Stuff

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
            return globalSeed = (float) FlxU.Random.NextDouble();
        }






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
        /// Check the local bitmap cache to see if a bitmap with this key has been loaded already.
        /// </summary>
        /// <param name="key">The string key identifying the bitmap.</param>
        /// <returns></returns>
        public static bool checkBitmapCache(string key)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Generates a new <code>BitmapData</code> object (a colored square) and caches it.
        /// </summary>
        /// <param name="width">How wide the square should be.</param>
        /// <param name="height">How high the square should be.</param>
        /// <param name="color">What color the square should be (0xAARRGGBB)</param>
        /// <param name="unique">Ensures that the bitmap data uses a new slot in the cache.</param>
        /// <param name="key">Force the cache to use a specific Key to index the bitmap.</param>
        /// <returns></returns>
        public static Texture2D createBitmap(uint width, uint height, Color color, bool unique = false, string key = null)
        {
            if (unique || !string.IsNullOrEmpty(key))
            {
                throw new NotSupportedException();
            }

            // we could also use the famous XNA1x1pxWhiteTexture here
            var texture = new Texture2D(FlxG.graphicsDevice, (int)width, (int)height);

            // flx# - why read the color data when everything is overridden anyway?
            var colorData = new Color[width * height];
            //texture.GetData<Color>(colorData);
            for (int i = 0; i < colorData.Length; ++i)
            {
                colorData[i] = color;
            }
            texture.SetData<Color>(colorData);

            return texture;
        }

        /// <summary>
        /// Loads a bitmap from a file, caches it, and generates a horizontally flipped version if necessary.
        /// </summary>
        /// <param name="graphic">The image file that you want to load.</param>
        /// <param name="reverse">Whether to generate a flipped version.</param>
        /// <param name="unique">Ensures that the bitmap data uses a new slot in the cache.</param>
        /// <param name="key">Force the cache to use a specific Key to index the bitmap.</param>
        /// <returns>The <code>BitmapData</code> we just created.</returns>
        static public Texture2D addBitmap(Texture2D graphic, bool reverse = false, bool unique = false, string key = null)
        {
            throw new NotSupportedException();

            /*
			var needReverse:Boolean = false;
			if(Key == null)
			{
				Key = String(Graphic)+(Reverse?"_REVERSE_":"");
				if(Unique && checkBitmapCache(Key))
				{
					var inc:uint = 0;
					var ukey:String;
					do
					{
						ukey = Key + inc++;
					} while(checkBitmapCache(ukey));
					Key = ukey;
				}
			}
			
			//If there is no data for this key, generate the requested graphic
			if(!checkBitmapCache(Key))
			{
				_cache[Key] = (new Graphic).bitmapData;
				if(Reverse)
					needReverse = true;
			}
			var pixels:BitmapData = _cache[Key];
			if(!needReverse && Reverse && (pixels.width == (new Graphic).bitmapData.width))
				needReverse = true;
			if(needReverse)
			{
				var newPixels:BitmapData = new BitmapData(pixels.width<<1,pixels.height,true,0x00000000);
				newPixels.draw(pixels);
				var mtx:Matrix = new Matrix();
				mtx.scale(-1,1);
				mtx.translate(newPixels.width,0);
				newPixels.draw(pixels,mtx);
				pixels = newPixels;
				_cache[Key] = pixels;
			}
			return pixels;
            */
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
