using fliXNA_xbox;
using flxSharp.flxSharp.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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
        static protected bool paused;

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
        /// Read-only: retrieves the Flash stage object (required for event listeners)
        /// Will be null if it's not safe/useful yet.
        /// </summary>
        public static object Stage
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
        protected static float volume;

        /// <summary>
        /// Set <code>volume</code> to a number between 0 and 1 to change the global volume.
        /// </summary>
        public static float Volume
        {
            get { return volume; }
            set
            {
                if (value > 1)
                {
                    value = 1;
                }
                else if (value < 0)
                {
                    value = 0;
                }

                volume = value;

                if (volumeHandler != null)
                {
                    volumeHandler(FlxG.mute ? 0 : volume);
                }
            }
        }

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
        /// Get and set the background color of the game.
        /// Get functionality is equivalent to FlxG.camera.bgColor.
        /// Set functionality sets the background color of all the current cameras.
        /// </summary>
        static public Color bgColor
        {
            get
            {
                if (FlxG.camera == null)
                {
                    return new Color(0f, 0f, 0f, 1f);
                }
                else
                {
                    return FlxG.camera.BgColor;
                }
            }

            set
            {
                foreach (FlxCamera camera in cameras)
                {
                    camera.BgColor = value;
                }
            }
        }

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
        public static List<FlxBasic> plugins { get; set; }

        /// <summary>
        /// Set this hook to get a callback whenever the volume changes.
        /// Function should take the form <code>myVolumeHandler(float Volume)</code>.
        /// </summary>
        public static event Action<float> volumeHandler;

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
        /// flx#
        /// Replacement for Flash getTimer(), which returns the total game time in milliseconds.
        /// </summary>
        public static uint getTimer { get; set; }

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

        /// <summary>
        /// How many times you want your game to update each second.
        /// More updates usually means better collisions and smoother motion.
        /// NOTE: This is NOT the same thing as the Flash Player framerate!
        /// </summary>
        public static float Framerate
        {
            get { return 1000 / _game.StepMS; }
            set
            {
                _game.StepMS = (uint)(1000 / value);

                if (_game.MaxAccumuation < _game.StepMS)
                {
                    _game.MaxAccumuation = _game.StepMS;
                }
            }
        }

        /// <summary>
        /// Read-only: access the current game state from anywhere.
        /// </summary>
        public static FlxState State
        {
            get { return _game.State; }
        }

        /// <summary>
        /// How many times you want your game to update each second.
        /// More updates usually means better collisions and smoother motion.
        /// NOTE: This is NOT the same thing as the Flash Player framerate!
        /// </summary>
        public static float FlashFramerate
        {
            get { throw new NotSupportedException();}
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Generates a random number.  Deterministic, meaning safe
        /// to use if you want to record replays in random environments.
        /// </summary>
        /// <returns>A <code>Number</code> between 0 and 1.</returns>
        internal static float random()
        {
            globalSeed = FlxU.srand(globalSeed);
            return globalSeed;
        }

        /// <summary>
		/// Shuffles the entries in an array into a new random order.
		/// <code>FlxG.shuffle()</code> is deterministic and safe for use with replays/recordings.
		/// HOWEVER, <code>FlxU.shuffle()</code> is NOT deterministic and unsafe for use with replays/recordings.
        /// </summary>
        /// <param name="objects">A Flash <code>Array</code> object containing...stuff.</param>
        /// <param name="howManyTimes">How many swaps to perform during the shuffle operation.  Good rule of thumb is 2-4 times as many objects are in the list.</param>
        /// <returns>The same Flash <code>Array</code> object that you passed in in the first place.</returns>
        public static Array shuffle(Array objects, uint howManyTimes)
        {
            throw new NotImplementedException();

            /*
			var i:uint = 0;
			var index1:uint;
			var index2:uint;
			var object:Object;
			while(i < HowManyTimes)
			{
				index1 = FlxG.random()*Objects.length;
				index2 = FlxG.random()*Objects.length;
				object = Objects[index2];
				Objects[index2] = Objects[index1];
				Objects[index1] = object;
				i++;
			}
			return Objects;
            */
        }

        /// <summary>
        /// Fetch a random entry from the given array.
        /// Will return null if random selection is missing, or array has no entries.
        /// <code>FlxG.getRandom()</code> is deterministic and safe for use with replays/recordings.
        /// HOWEVER, <code>FlxU.getRandom()</code> is NOT deterministic and unsafe for use with replays/recordings.
        /// </summary>
        /// <param name="objects">A Flash array of objects.</param>
        /// <param name="startIndex">Optional offset off the front of the array. Default value is 0, or the beginning of the array.</param>
        /// <param name="length">Optional restriction on the number of values you want to randomly select from.</param>
        /// <returns>The random object that was selected.</returns>
        public static object GetRandom(Array objects, int startIndex = 0, int length = 0)
        {
            /*
			if(Objects != null)
			{
				var l:uint = Length;
				if((l == 0) || (l > Objects.length - StartIndex))
					l = Objects.length - StartIndex;
				if(l > 0)
					return Objects[StartIndex + uint(FlxG.random()*l)];
			}
			return null;
            */

            if (objects != null)
            {
                int l = length;
                if ((length == 0) || (l < objects.Length - startIndex))
                {
                    length = objects.Length - startIndex;
                }

                if (length > 0)
                {
                    return objects.GetValue(
                        startIndex + (int) (FlxG.random() * length));
                }
            }

            return null;
        }

        /// <summary>
        /// Load replay data from a string and play it back.
        /// </summary>
        /// <param name="data">The replay that you want to load.</param>
        /// <param name="state">Optional parameter: if you recorded a state-specific demo or cutscene, pass a new instance of that state here.</param>
        /// <param name="cancelKeys">Optional parameter: an array of string names of keys (see FlxKeyboard) that can be pressed to cancel the playback, e.g. ["ESCAPE","ENTER"].  Also accepts 2 custom key names: "ANY" and "MOUSE" (fairly self-explanatory I hope!).</param>
        /// <param name="timeout">Optional parameter: set a time limit for the replay.  CancelKeys will override this if pressed.</param>
        /// <param name="callback">Optional parameter: if set, called when the replay finishes.  Running to the end, CancelKeys, and Timeout will all trigger Callback(), but only once, and CancelKeys and Timeout will NOT call FlxG.stopReplay() if Callback is set!</param>
        public static void loadReplay(String data, FlxState state = null, Array cancelKeys = null, float timeout = 0, Action callback = null)
        {
            throw new NotImplementedException();

            /*
			_game._replay.load(Data);
			if(State == null)
				FlxG.resetGame();
			else
				FlxG.switchState(State);
			_game._replayCancelKeys = CancelKeys;
			_game._replayTimer = Timeout*1000;
			_game._replayCallback = Callback;
			_game._replayRequested = true;
            */
        }

        /// <summary>
        /// Resets the game or state and replay requested flag.
        /// </summary>
        /// <param name="standardMode">If true, reload entire game, else just reload current game state.</param>
        public static void loadReplay(bool standardMode = true)
        {
            throw new NotImplementedException();

            /*
			if(StandardMode)
				FlxG.resetGame();
			else
				FlxG.resetState();
			if(_game._replay.frameCount > 0)
				_game._replayRequested = true;
            */
        }

        /// <summary>
        /// Resets the game or state and requests a new recording.
        /// </summary>
        /// <param name="standardMode">If true, reset the entire game, else just reset the current state.</param>
        public static void recordReplay(bool standardMode = true)
        {
            throw new NotImplementedException();

            /*
			if(StandardMode)
				FlxG.resetGame();
			else
				FlxG.resetState();
			_game._recordingRequested = true;
            */
        }

        /// <summary>
        /// Stop recording the current replay and return the replay data.
        /// </summary>
        /// <returns>The replay data in simple ASCII format (see <code>FlxReplay.save()</code>).</returns>
        public static string stopRecording()
        {
            throw new NotImplementedException();

            /*
			_game._recording = false;
			if(_game._debugger != null)
				_game._debugger.vcr.stopped();
			return _game._replay.save();
            */
        }

        /// <summary>
        /// Request a reset of the current game state.
        /// </summary>
        public static void resetState()
        {
            throw new NotImplementedException();

            /*
            _game._requestedState = new (FlxU.getClass(FlxU.getClassName(_game._state,false)))();
            */
        }

        /// <summary>
        /// Like hitting the reset button on a game console, this will re-launch the game as if it just started.
        /// </summary>
        public static void resetGame()
        {
            _game.RequestedReset = true;
        }

        /// <summary>
        /// Reset the input helper objects (useful when changing screens or states).
        /// </summary>
        public static void resetInput()
        {
            //throw new NotImplementedException("GamepadReset");

            keys.reset();
            mouse.reset();
        }

        /// <summary>
        /// Set up and play a looping background soundtrack.
        /// </summary>
        /// <param name="soundEffect">The sound file you want to loop in the background.</param>
        /// <param name="volume">How loud the sound should be, from 0 to 1.</param>
        static public void playMusic(SoundEffect soundEffect, float volume = 1.0f)
        {
            // flx# - Mediaplayer && Song instance here!

            if (FlxG.music == null)
            {
                FlxG.music = new FlxSound();
            }
            else if (FlxG.music.Active)
            {
                FlxG.music.stop();
            }

            music.loadEmbedded(soundEffect, true);
            music.Volume = volume;
            music.survive = true;
            music.play();
        }

        /// <summary>
        /// Creates a new sound object.
        /// </summary>
        /// <param name="soundEffect">The embedded sound resource you want to play. To stream, use the optional URL parameter instead.</param>
        /// <param name="volume">How loud to play it (0 to 1).</param>
        /// <param name="looped">Whether to loop this sound.</param>
        /// <param name="autoDestroy">Whether to destroy this sound when it finishes playing. Leave this value set to "false" if you want to re-use this <code>FlxSound</code> instance.</param>
        /// <param name="autoPlay">Whether to play the sound.</param>
        /// <param name="url">Load a sound from an external web resource instead. Only used if EmbeddedSound = null.</param>
        /// <returns>A <code>FlxSound</code> object.</returns>
        public static FlxSound loadSound(SoundEffect soundEffect, float volume = 1.0f, bool looped = false, bool autoDestroy = false, bool autoPlay = false, string url = null)
        {
            if (!string.IsNullOrEmpty(url))
            {
                throw new NotSupportedException();
            }

            var sound = new FlxSound();
            sound.loadEmbedded(soundEffect, looped, autoDestroy);
            sound.Volume = volume;

            if (autoPlay)
            {
                sound.play();
            }

            return sound;
        }

        /// <summary>
        /// Creates a new sound object from an embedded <code>Class</code> object.
        /// NOTE: Just calls FlxG.loadSound() with AutoPlay == true.
        /// </summary>
        /// <param name="soundEffect">The sound you want to play.</param>
        /// <param name="volume">How loud to play it (0 to 1).</param>
        /// <param name="looped">Whether to loop this sound.</param>
        /// <param name="autoDestroy">Whether to destroy this sound when it finishes playing. Leave this value set to "false" if you want to re-use this <code>FlxSound</code> instance.</param>
        /// <returns>A <code>FlxSound</code> object.</returns>
        public static FlxSound play(SoundEffect soundEffect, float volume = 1.0f, bool looped = false, bool autoDestroy = false)
        {
            return FlxG.loadSound(soundEffect, volume, looped, autoDestroy, true);
        }

        /// <summary>
        /// Creates a new sound object from a URL.
        /// NOTE: Just calls FlxG.loadSound() with AutoPlay == true.
        /// </summary>
        /// <param name="url">The URL of the sound you want to play.</param>
        /// <param name="Volume">How loud to play it (0 to 1).</param>
        /// <param name="looped">Whether or not to loop this sound.</param>
        /// <param name="autoDestroy">Whether to destroy this sound when it finishes playing. Leave this value set to "false" if you want to re-use this <code>FlxSound</code> instance.</param>
        /// <returns></returns>
		public static FlxSound stream(string url, float Volume = 1.0f, bool looped = false, bool autoDestroy = true)
		{
		    throw new NotSupportedException();

            /*
			return FlxG.loadSound(null,Volume,Looped,AutoDestroy,true,URL);
            */
        }

        /// <summary>
        /// Called by FlxGame on state changes to stop and destroy sounds.
        /// </summary>
        /// <param name="forceDestroy">Kill sounds even if they're flagged <code>survive</code>.</param>
        internal static void destroySounds(bool forceDestroy = false)
        {
            if ((music != null) && (forceDestroy || !music.survive))
            {
                music.destroy();
                music = null;
            }

            foreach (FlxSound sound in sounds)
            {
                if (forceDestroy || !sound.survive)
                {
                    sound.destroy();
                }
            }
        }

        /// <summary>
        /// Called by the game loop to make sure the sounds get updated each frame.
        /// </summary>
        internal static void updateSounds()
        {
            if ((music != null) && music.Active)
            {
                music.update();
            }

            if ((sounds != null) && sounds.Active)
            {
                sounds.update();
            }
        }

        /// <summary>
        /// Pause all sounds currently playing.
        /// </summary>
        public static void pauseSounds()
        {
            if ((music != null) && music.Exists && music.Active)
            {
                music.pause();
            }

            foreach (FlxSound sound in sounds)
            {
                if ((sound != null) && sound.Exists && sound.Active)
                {
                    sound.pause();
                }
            }
        }

        /// <summary>
        /// Resume playing existing sounds.
        /// </summary>
        public static void resumeSounds()
        {
            if ((music != null) && music.Exists)
            {
                music.play();
            }

            foreach (FlxSound sound in sounds)
            {
                if ((sound != null) && sound.Exists)
                {
                    sound.resume();
                }
            }
        }

        /// <summary>
        /// Check the local bitmap cache to see if a bitmap with this key has been loaded already.
        /// </summary>
        /// <param name="key">The string key identifying the bitmap.</param>
        /// <returns></returns>
        public static bool checkBitmapCache(string key)
        {
            throw new NotSupportedException();

            /*
            return (_cache[Key] != undefined) && (_cache[Key] != null);
            */
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
            var texture = new Texture2D(FlxS.GraphicsDevice, (int)width, (int)height);

            // flx# - user (u)int colors for faster access?
            var colorData = new Color[width * height];
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
        /// Dumps the cache's image references.
        /// </summary>
        public static void clearBitmapCache()
        {
            throw new NotSupportedException();

            /*
            _cache = new Object();
            */
        }

        /// <summary>
        /// Switch from the current game state to the one specified here.
        /// </summary>
        /// <param name="state">The new state.</param>
        static public void switchState(FlxState state)
        {
            _game.RequestedState = state;

            //state.destroy();
            //state = State;
            //state.create();
        }

        /// <summary>
        /// Change the way the debugger's windows are laid out.
        /// </summary>
        /// <param name="layout">See the presets above (e.g. <code>DEBUGGER_MICRO</code>, etc).</param>
        public static void setDebuggerLayout(uint layout)
        {
            throw new NotImplementedException();

            /*
			if(_game._debugger != null)
				_game._debugger.setLayout(Layout);
            */
        }

        /// <summary>
        /// Just resets the debugger windows to whatever the last selected layout was (<code>DEBUGGER_STANDARD</code> by default).
        /// </summary>
        public static void resetDebuggerLayout()
        {
            throw new NotImplementedException();

            /*
			if(_game._debugger != null)
				_game._debugger.resetLayout();
            */
        }

        /// <summary>
        /// Add a new camera object to the game.
        /// Handy for PiP, split-screen, etc.
        /// </summary>
        /// <param name="newCamera">The camera you want to add.</param>
        /// <returns>This <code>FlxCamera</code> instance.</returns>
        static public FlxCamera addCamera(FlxCamera newCamera)
        {
            FlxG.cameras.Add(newCamera);

            var newViewport = new Viewport((int)newCamera.X, (int)newCamera.Y, (int)newCamera.Width, (int)newCamera.Height);
            FlxS.Viewports.Add(newViewport);
            
            //FlxG.log("camera is at x: " + NewCamera.x + " y: " + NewCamera.y + " width: " + NewCamera.width + " height " + NewCamera.height);
            //FlxG.log("camera count: " + FlxG.cameras.Count);

            return newCamera;

            /*
			FlxG._game.addChildAt(NewCamera._flashSprite,FlxG._game.getChildIndex(FlxG._game._mouse));
			FlxG.cameras.push(NewCamera);
			return NewCamera;
            */
        }

        /// <summary>
        /// Remove a camera from the game.
        /// </summary>
        /// <param name="camera">The camera you want to remove.</param>
        /// <param name="destroy">Whether to call destroy() on the camera, default value is true.</param>
        public static void removeCamera(FlxCamera camera, bool destroy = true)
        {
            throw new NotImplementedException();

            /*
			try
			{
				FlxG._game.removeChild(Camera._flashSprite);
			}
			catch(E:Error)
			{
				FlxG.log("Error removing camera, not part of game.");
			}
			if(Destroy)
				Camera.destroy();
            */
        }

        /// <summary>
        /// Dumps all the current cameras and resets to just one camera.
        /// Handy for doing split-screen especially.
        /// </summary>
        /// <param name="newCamera">Optional; specify a specific camera object to be the new main camera.</param>
        public static void resetCameras(FlxCamera newCamera = null)
        {
            foreach (FlxCamera flxCam in cameras)
            {
                //FlxG._game.removeChild(cam._flashSprite); // ?
                flxCam.destroy();
            }

            cameras.Clear();

            if (newCamera == null)
            {
                newCamera = new FlxCamera(0, 0, FlxG.width, FlxG.height);
            }

            FlxG.addCamera(newCamera);
            FlxG.camera = newCamera;

            /*
			var cam:FlxCamera;
			var i:uint = 0;
			var l:uint = cameras.length;
			while(i < l)
			{
				cam = FlxG.cameras[i++] as FlxCamera;
				FlxG._game.removeChild(cam._flashSprite);
				cam.destroy();
			}
			FlxG.cameras.length = 0;
			
			if(NewCamera == null)
				NewCamera = new FlxCamera(0,0,FlxG.width,FlxG.height)
			FlxG.camera = FlxG.addCamera(NewCamera);
            */
        }

        /// <summary>
        /// All screens are filled with this color and gradually return to normal.
        /// </summary>
        /// <param name="color">The color you want to use.</param>
        /// <param name="duration">How long it takes for the flash to fade.</param>
        /// <param name="onComplete">A function you want to run when the flash finishes.</param>
        /// <param name="force">Force the effect to reset.</param>
        public static void flash(Color color, float duration = 1.0f, Action onComplete = null, bool force = false)
        {
            foreach (FlxCamera camera in cameras)
            {
                camera.Flash(color, duration, onComplete, force);
            }
        }

        /// <summary>
        /// The screen is gradually filled with this color.
        /// </summary>
        /// <param name="color">The color you want to use.</param>
        /// <param name="duration">How long it takes for the fade to fade.</param>
        /// <param name="onComplete">A function you want to run when the flash finishes.</param>
        /// <param name="force">Force the effect to reset.</param>
        public static void fade(Color color, float duration = 1.0f, Action onComplete = null, bool force = false)
        {
            foreach (FlxCamera camera in cameras)
            {
                camera.Fade(color, duration, onComplete, force);
            }
        }

        /// <summary>
        /// A simple screen-shake effect.
        /// </summary>
        /// <param name="intensity"></param>
        /// <param name="duration"></param>
        /// <param name="onComplete"></param>
        /// <param name="force"></param>
        /// <param name="direction"></param>
        static internal void shake(float intensity = 0.05f, float duration = 0.5f, Action onComplete = null, bool force = true, uint direction = 0)
        {
            foreach (FlxCamera camera in cameras)
            {
                FlxG.camera.shake(intensity, duration, onComplete, force, direction);
            }
        }

        /// <summary>
        /// Call this function to see if one <code>FlxObject</code> overlaps another.
        /// Can be called with one object and one group, or two groups, or two objects,
        /// whatever floats your boat! For maximum performance try bundling a lot of objects
        /// together using a <code>FlxGroup</code> (or even bundling groups together!).
        /// 
        /// <p>NOTE: does NOT take objects' scrollfactor into account, all overlaps are checked in world space.</p>
        /// </summary>
        /// <param name="ObjectOrGroup1">The first object or group you want to check.</param>
        /// <param name="ObjectOrGroup2">The second object or group you want to check. If it is the same as the first, flixel knows to just do a comparison within that group.</param>
        /// <param name="NotifyCallback">A function with two <code>FlxObject</code> parameters - e.g. <code>myOverlapFunction(Object1:FlxObject,Object2:FlxObject)</code> - that is called if those two objects overlap.</param>
        /// <param name="ProcessCallback">A function with two <code>FlxObject</code> parameters - e.g. <code>myOverlapFunction(Object1:FlxObject,Object2:FlxObject)</code> - that is called if those two objects overlap. If a ProcessCallback is provided, then NotifyCallback will only be called if ProcessCallback returns true for those objects!</param>
        /// <returns></returns>
        static public Boolean overlap(FlxObject ObjectOrGroup1 = null, FlxObject ObjectOrGroup2 = null, Func<FlxObject, FlxObject, Boolean> NotifyCallback = null, Func<FlxObject, FlxObject, Boolean> ProcessCallback = null)
        {
            if (ObjectOrGroup1 == null)
            {
                ObjectOrGroup1 = FlxG.State;                
            }

            if (ObjectOrGroup2 == ObjectOrGroup1)
            {
                ObjectOrGroup2 = null;                
            }

            FlxQuadTree.divisions = FlxG.worldDivisions;
            FlxQuadTree quadTree = new FlxQuadTree(FlxG.worldBounds.X, FlxG.worldBounds.Y, FlxG.worldBounds.Width, FlxG.worldBounds.Height);
            quadTree.load(ObjectOrGroup1, ObjectOrGroup2, NotifyCallback, ProcessCallback);
            Boolean result = quadTree.execute();
            quadTree.destroy();
            return result;
        }

        /// <summary>
        /// Call this function to see if one <code>FlxObject</code> collides with another.
        /// Can be called with one object and one group, or two groups, or two objects,
        /// whatever floats your boat! For maximum performance try bundling a lot of objects
        /// together using a <code>FlxGroup</code> (or even bundling groups together!).
        /// 
        /// <p>This function just calls FlxG.overlap and presets the ProcessCallback parameter to FlxObject.separate.
        /// To create your own collision logic, write your own ProcessCallback and use FlxG.overlap to set it up.</p>
        /// 
        /// <p>NOTE: does NOT take objects' scrollfactor into account, all overlaps are checked in world space.</p>
        /// </summary>
        /// <param name="objectOrGroup1">The first object or group you want to check.</param>
        /// <param name="objectOrGroup2">The second object or group you want to check. If it is the same as the first, flixel knows to just do a comparison within that group.</param>
        /// <param name="notifyCallback">A function with two <code>FlxObject</code> parameters - e.g. <code>myOverlapFunction(Object1:FlxObject,Object2:FlxObject)</code> - that is called if those two objects overlap.</param>
        /// <returns>Whether any objects were successfully collided/separated.</returns>
        static public Boolean collide(FlxObject objectOrGroup1 = null, FlxObject objectOrGroup2 = null, Func<FlxObject, FlxObject, Boolean> notifyCallback = null)
        {
            return overlap(objectOrGroup1, objectOrGroup2, notifyCallback, FlxObject.separate);
        }

        /// <summary>
        /// Adds a new plugin to the global plugin array.
        /// </summary>
        /// <param name="plugin">Any object that extends FlxBasic. Useful for managers and other things. See org.flixel.plugin for some examples!</param>
        /// <returns>The same <code>FlxBasic</code>-based plugin you passed in.</returns>
        public static FlxBasic addPlugin(FlxBasic plugin)
        {
            // Don't add repeats
            if (!plugins.Contains(plugin))
            {
                plugins.Add(plugin);
            }

            return plugin;
        }

        /// <summary>
        /// Removes an instance of a plugin from the global plugin array.
        /// </summary>
        /// <param name="plugin">The plugin instance you want to remove.</param>
        /// <returns>The same <code>FlxBasic</code>-based plugin you passed in.</returns>
        public static FlxBasic removePlugin(FlxBasic plugin)
        {
            plugins.RemoveAll(existingPlugin => existingPlugin == plugin);

            return plugin;
        }

        /// <summary>
        /// Removes an instance of a plugin from the global plugin array.
        /// </summary>
        /// <param name="type">The class name of the plugin type you want removed from the array.</param>
        /// <returns>Whether or not at least one instance of this plugin type was removed.</returns>
        public static bool removePluginType(Type type)
        {
            int removalCount = plugins.RemoveAll(existingPlugin => existingPlugin.GetType() == type);

            return (removalCount > 0);
        }

        /// <summary>
        /// Called by <code>FlxGame</code> to set up <code>FlxG</code> during <code>FlxGame</code>'s constructor.
        /// </summary>
        internal static void init(FlxGame game, int width, int height, float zoom)
        {
            FlxG._game = game;
            FlxG.width = width;
            FlxG.height = height;

            FlxG.mute = false;
            FlxG.volume = 1.0f;
            FlxG.sounds = new FlxGroup();
            FlxG.volumeHandler = delegate { };

            //FlxG.clearBitmapCache();
			
            //if(flashGfxSprite == null)
            //{
            //    flashGfxSprite = new Sprite();
            //    flashGfx = flashGfxSprite.graphics;
            //} 

            FlxCamera.DefaultZoom = zoom;
            //FlxG._cameraRect = new Rectangle();
            FlxG.cameras = new List<FlxCamera>();
            //FlxG.UseBufferLocking = false;

            FlxG.plugins = new List<FlxBasic>();
			//addPlugin(new DebugPathDisplay());
			//addPlugin(new TimerManager());

            FlxG.mouse = new FlxMouse();
            FlxG.keys = new FlxKeyboard();
            //FlxG.mobile = false;

            FlxG.levels = null;
            FlxG.scores = null;
            FlxG.visualDebug = false;

            // flx# stuff

            FlxG.cameras = new List<FlxCamera>();

            //FlxG.width = FlxG.graphics.PreferredBackBufferWidth;
            //FlxG.height = FlxG.graphics.PreferredBackBufferHeight;
            FlxG.bgColor = Color.Black;
            FlxG.mute = false;
            FlxG.sounds = new FlxGroup();
            FlxG.worldBounds = new FlxRect();
            FlxG.defaultFont = FlxS.ContentManager.Load<SpriteFont>("ConsoleFont");
            //FlxG.zoom = 1f;
            FlxG.pad1 = new FlxGamepad(PlayerIndex.One);
            FlxG.pad2 = new FlxGamepad(PlayerIndex.Two);
            FlxG.pad3 = new FlxGamepad(PlayerIndex.Three);
            FlxG.pad4 = new FlxGamepad(PlayerIndex.Four);
            FlxG.keys = new FlxKeyboard();
            FlxG.mouse = new FlxMouse();
            FlxCamera defaultCam = new FlxCamera(0, 0, FlxG.width, FlxG.height, FlxG.zoom);
            FlxG.addCamera(defaultCam);

            //Thread tCreate = new Thread(FlxG.state.create);
            //tCreate.Start();

            //create state last
            //FlxG.State.create();
        }

        /// <summary>
        /// Called whenever the game is reset, doesn't have to do quite as much work as the basic initialization stuff.
        /// </summary>
        static internal void reset()
        {
            //FlxG.clearBitmapCache();
            FlxG.resetInput();
            FlxG.destroySounds(true);
            FlxG.levels = null;
            FlxG.scores = null;
            FlxG.level = 0;
            FlxG.score = 0;
            FlxG.paused = false;
            FlxG.timeScale = 1.0f;
            FlxG.elapsed = 0;
            FlxG.globalSeed = (float) FlxU.Random.NextDouble();
            FlxG.worldBounds = new FlxRect(-10, -10, FlxG.width + 20, FlxG.height + 20);
            FlxG.worldDivisions = 6;

            /*
			var debugPathDisplay:DebugPathDisplay = FlxG.getPlugin(DebugPathDisplay) as DebugPathDisplay;
			if(debugPathDisplay != null)
				debugPathDisplay.clear();
            */
        }

        /// <summary>
        /// Called by the game object to update the keyboard and mouse input tracking objects.
        /// </summary>
        internal static void updateInput()
        {
            FlxG.keys.update();
            FlxG.mouse.update();

            // flx#
            pad1.update();
            pad2.update();
            pad3.update();
            pad4.update();
        }

        /// <summary>
        /// Called by the game object to lock all the camera buffers and clear them for the next draw pass.
        /// </summary>
        internal static void lockCameras()
        {
            //throw new NotImplementedException();

            /*
			var cam:FlxCamera;
			var cams:Array = FlxG.cameras;
			var i:uint = 0;
			var l:uint = cams.length;
			while(i < l)
			{
				cam = cams[i++] as FlxCamera;
				if((cam == null) || !cam.exists || !cam.visible)
					continue;
				if(useBufferLocking)
					cam.buffer.lock();
				cam.fill(cam.bgColor);
				cam.screen.dirty = true;
			}
            */
        }

        /// <summary>
        /// Called by the game object to draw the special FX and unlock all the camera buffers.
        /// </summary>
        internal static void unlockCameras()
        {
            //throw new NotImplementedException();

            foreach (FlxCamera camera in FlxG.cameras)
            {
                if (!camera.Exists || !camera.Visible)
                {
                    continue;
                }

                camera.drawFX();
            }

            /*
			var cam:FlxCamera;
			var cams:Array = FlxG.cameras;
			var i:uint = 0;
			var l:uint = cams.length;
			while(i < l)
			{
				cam = cams[i++] as FlxCamera;
				if((cam == null) || !cam.exists || !cam.visible)
					continue;
				cam.drawFX();
				if(useBufferLocking)
					cam.buffer.unlock();
			}
            */
        }

        /// <summary>
        /// Called by the game object to update the cameras and their tracking/special effects logic.
        /// </summary>
        internal static void updateCameras()
        {
            foreach (FlxCamera cam in FlxG.cameras)
            {
                if (cam.Exists)
                {
                    if (cam.Active)
                    {
                        cam.update();                        
                    }

                    cam.FlashSprite.X = cam.X + cam.FlashOffsetX;
                    cam.FlashSprite.Y = cam.Y + cam.FlashOffsetY;
                    cam.FlashSprite.Visible = cam.Visible;
                }
            }
        }

        /// <summary>
        /// Used by the game object to call <code>update()</code> on all the plugins.
        /// </summary>
        internal static void updatePlugins()
        {
            foreach (FlxBasic plugin in plugins)
            {
                if (plugin.Exists && plugin.Active)
                {
                    plugin.update();
                }
            }
        }

        /// <summary>
        /// Used by the game object to call <code>draw()</code> on all the plugins.
        /// </summary>
        internal static void drawPlugins()
        {
            foreach (FlxBasic plugin in plugins)
            {
                if (plugin.Exists && plugin.Visible)
                {
                    plugin.draw();
                }
            }
        }

        #region flx# Stuff

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
        //static internal FlxState state;

        /// <summary>
        /// The default font that you can use for text
        /// </summary>
        static public SpriteFont defaultFont;

        /// <summary>
        /// Can be used to zoom the camera
        /// </summary>
        static public float zoom;

        /// <summary>
        /// Internal function for updating Camera, State, Console, and Elapsed Time
        /// </summary>
        /// <param name="dt"></param>
        static internal void update(GameTime dt)
        {
            elapsed = (float)dt.ElapsedGameTime.TotalSeconds;

            //state.update();
            updateCameras();
            sounds.update();
            updateInput();
        }

        #endregion // flx# Stuff

        internal static float snap(float XorY, float Grid)
		{
            float coord = XorY;
            if (coord % Grid > 0)
                coord -= (coord % Grid);
            return coord;
		}
    }
}
