using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace flxSharp.flxSharp.System
{
    /// <summary>
    /// FlxGame is the heart of all flixel games, and contains a bunch of basic game loops and things.
    /// It is a long and sloppy file that you shouldn't have to worry about too much!
    /// It is basically only used to create your game object in the first place,
    /// after that FlxG and FlxState have all the useful stuff you actually need.
    /// </summary>
    public class FlxGame
    {
        /// <summary>
        /// Sets 0, -, and + to control the global volume sound volume.
        /// @default true
        /// </summary>
        public bool UseSoundHotKeys { get; set; }

        /// <summary>
        /// Tells flixel to use the default system mouse cursor instead of custom Flixel mouse cursors.
        /// @default false
        /// </summary>
        public bool UseSystemCursor { get; set; }

        /// <summary>
        /// Initialize and allow the flixel debugger overlay even in release mode.
        /// Also useful if you don't use FlxPreloader!
        /// @default false
        /// </summary>
        public bool ForceDebugger { get; set; }

        /// <summary>
        /// Current game state.
        /// </summary>
        internal FlxState State;

        /// <summary>
        /// Mouse cursor.
        /// </summary>
        internal Texture2D Mouse;

        /// <summary>
        /// Class type of the initial/first game state for the game, usually MenuState or something like that.
        /// </summary>
        protected FlxState InitialState;

        /// <summary>
        /// Whether the game object's basic initialization has finished yet.
        /// </summary>
        protected bool Created { get; set; }

        /// <summary>
        /// Total number of milliseconds elapsed since game start.
        /// </summary>
        protected ulong Total { get; set; }

        /// <summary>
        /// Total number of milliseconds elapsed since last update loop.
        /// Counts down as we step through the game loop.
        /// </summary>
        protected uint Accumulator { get; set; }

        /// <summary>
        /// Whether the Flash player lost focus.
        /// </summary>
        protected bool LostFocus { get; set; }

        /// <summary>
        /// Milliseconds of time per step of the game loop. FlashEvent.g. 60 fps = 16ms.
        /// </summary>
        internal uint StepMS { get; set; }

        /// <summary>
        /// Framerate of the Flash player (NOT the game loop). Default = 30.
        /// </summary>
        internal uint FlashFramerate
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Max allowable accumulation (see _accumulator).
        /// Should always (and automatically) be set to roughly 2x the flash player framerate.
        /// </summary>
        public uint MaxAccumuation { get; set; }

        /// <summary>
        /// If a state change was requested, the new state object is stored here until we switch to it.
        /// </summary>
        public FlxState RequestedState { get; set; }

        /// <summary>
        /// A flag for keeping track of whether a game reset was requested or not.
        /// </summary>
        public bool RequestedReset { get; set; }

        /// <summary>
        /// The "focus lost" screen (see <code>createFocusScreen()</code>).
        /// </summary>
        protected Texture2D Focus;

        /// <summary>
        /// The sound tray display container (see <code>createSoundTray()</code>).
        /// </summary>
        protected Texture2D SoundTray;

        /// <summary>
        /// Helps us auto-hide the sound tray after a volume change.
        /// </summary>
        protected float SoundTrayTimer;

        /// <summary>
        /// Helps display the volume bars on the sound tray.
        /// </summary>
        protected Array SoundTrayBars;

        /**
		 * The debugger overlay object.
		 */
		//internal var _debugger:FlxDebugger;

		/**
		 * A handy boolean that keeps track of whether the debugger exists and is currently visible.
		 */
		//internal var _debuggerUp:Boolean;
		
		/**
		 * Container for a game replay object.
		 */
		//internal var _replay:FlxReplay;

		/**
		 * Flag for whether a playback of a recording was requested.
		 */
		//internal var _replayRequested:Boolean;

		/**
		 * Flag for whether a new recording was requested.
		 */
		//internal var _recordingRequested:Boolean;

		/**
		 * Flag for whether a replay is currently playing.
		 */
		//internal var _replaying:Boolean;

		/**
		 * Flag for whether a new recording is being made.
		 */
		//internal var _recording:Boolean;

		/**
		 * Array that keeps track of keypresses that can cancel a replay.
		 * Handy for skipping cutscenes or getting out of attract modes!
		 */
		//internal var _replayCancelKeys:Array;

		/**
		 * Helps time out a replay if necessary.
		 */
		//internal var _replayTimer:int;

		/**
		 * This function, if set, is triggered when the callback stops playing.
		 */
		//internal var _replayCallback:Function;

        /// <summary>
        /// Required XNA stuff
        /// </summary>
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        /// <summary>
        /// FlxGame constructor takes in the initial state and resolution of the screen.
        /// </summary>
        /// <param name="State">The state you want to load</param>
        /// <param name="Width">The width of the screen</param>
        /// <param name="Height">The height of the screen</param>
        /// <param name="ContentRootDirectory">The directory of your content.  It is set automatically by default but you can change it if you want to.</param>
        public FlxGame(FlxState State, int Width = 1280, int Height = 720, float Zoom = 1.0f, string ContentRootDirectory = "Content") : base()
        {
            throw new NotSupportedException();

            //FlxG.state = State;
            
            /*
            FlxG.zoom = Zoom;
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = Width / (int)Zoom;
            graphics.PreferredBackBufferHeight = Height / (int)Zoom;
            graphics.ApplyChanges();
            Content.RootDirectory = ContentRootDirectory;
            */
        }

        /// <summary>
        /// Instantiate a new game object.
        /// </summary>
        /// <param name="gameSizeX">The width of your game in game pixels, not necessarily final display pixels (see Zoom).</param>
        /// <param name="gameSizeY">The height of your game in game pixels, not necessarily final display pixels (see Zoom).</param>
        /// <param name="initialState">The class name of the state you want to create and switch to first (e.g. MenuState).</param>
        /// <param name="zoom">The default level of zoom for the game's cameras (e.g. 2 = all pixels are now drawn at 2x). Default = 1.</param>
        /// <param name="gameFramerate">How frequently the game should update (default is 60 times per second).</param>
        /// <param name="flashFramerate">Sets the actual display framerate for Flash player (default is 30 times per second).</param>
        /// <param name="useSystemCursor">Whether to use the default OS mouse pointer, or to use custom flixel ones.</param>
        public FlxGame(int gameSizeX,
                       int gameSizeY,
                       FlxState initialState,
                       float zoom = 1.0f,
                       int gameFramerate = 60,
                       int flashFramerate = 0,
                       bool useSystemCursor = false) : base()
        {
            if (flashFramerate != 0)
            {
                throw new NotSupportedException();
            }

            // super high priority init stuff (focus, mouse, etc)
            LostFocus = false;
            /*
			_focus = new Sprite();
			_focus.visible = false;
			_soundTray = new Sprite();
			_mouse = new Sprite() 
            */

            // flx# - the tight stuff is done in Initialize

            // basic display and update setup stuff
            FlxG.init(this, gameSizeX, gameSizeY, zoom);
            FlxG.Framerate = gameFramerate;
            //FlxG.FlashFramerate = flashFramerate;
            Accumulator = StepMS;
            Total = 0;
            State = null;
            UseSoundHotKeys = true;
            UseSystemCursor = useSystemCursor;

            if (!useSystemCursor)
            {
                //flash.ui.mouse.hide();
            }

            ForceDebugger = false;
            //_debuggerUp = false;

            // replay data
            /*
            _replay = new FlxReplay();
            _replayRequested = false;
            _recordingRequested = false;
            _replaying = false;
            _recording = false;
            */

            // then get ready to create the game object for real
            InitialState = initialState;
            RequestedState = null;
            RequestedReset = true;
            Created = false;
            //addEventListener(Event.ENTER_FRAME, create);
        }

        /// <summary>
        /// Makes the little volume tray slide out.
        /// </summary>
        /// <param name="silent">Whether or not it should beep.</param>
        internal void showSoundTray(bool silent = false)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Internal event handler for input and focus.
        /// </summary>
        /// <param name="keyboardEvent">Flash keyboard event.</param>
        protected void onKeyUp(EventArgs keyboardEvent)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Internal event handler for input and focus.
        /// </summary>
        /// <param name="keyboardEvent">Flash keyboard event.</param>
        protected void onKeyDown(EventArgs keyboardEvent)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Internal event handler for input and focus.
        /// </summary>
        /// <param name="mouseEvent">Flash mouse event.</param>
        protected void onMouseDown(EventArgs mouseEvent)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Internal event handler for input and focus.
        /// </summary>
        /// <param name="mouseEvent">Flash mouse event.</param>
        protected void onMouseUp(EventArgs mouseEvent)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Internal event handler for input and focus.
        /// </summary>
        /// <param name="mouseEvent">Flash mouse event.</param>
        protected void onMouseWheel(EventArgs mouseEvent)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Internal event handler for input and focus.
        /// </summary>
        /// <param name="eventArgs">Flash event.</param>
        protected void onFocus(EventArgs eventArgs = null)
        {
            throw new NotImplementedException();

            /*
            if(!_debuggerUp && !useSystemCursor)
                flash.ui.Mouse.hide();
            FlxG.resetInput();
            _lostFocus = _focus.visible = false;
            stage.frameRate = _flashFramerate;
            FlxG.resumeSounds();
            */
        }

        /// <summary>
        /// Internal event handler for input and focus.
        /// </summary>
        /// <param name="eventArgs">Flash event.</param>
        protected void onFocusLost(EventArgs eventArgs = null)
        {
            throw new NotImplementedException();

            /*
			if((x != 0) || (y != 0))
			{
				x = 0;
				y = 0;
			}
			flash.ui.Mouse.show();
			_lostFocus = _focus.visible = true;
			stage.frameRate = 10;
			FlxG.pauseSounds();
            */
        }

        /// <summary>
        /// Handles the onEnterFrame call and figures out how many updates and draw calls to do.
        /// </summary>
        /// <param name="eventArgs">Flash event.</param>
        protected void onEnterFrame(EventArgs eventArgs = null)
        {
            /*
			var mark:uint = getTimer();
			var elapsedMS:uint = mark-_total;
			_total = mark;
			updateSoundTray(elapsedMS);
			if(!_lostFocus)
			{
				if((_debugger != null) && _debugger.vcr.paused)
				{
					if(_debugger.vcr.stepRequested)
					{
						_debugger.vcr.stepRequested = false;
						step();
					}
				}
				else
				{
					_accumulator += elapsedMS;
					if(_accumulator > _maxAccumulation)
						_accumulator = _maxAccumulation;
					while(_accumulator >= _step)
					{
						step();
						_accumulator = _accumulator - _step; 
					}
				}
				
				FlxBasic._VISIBLECOUNT = 0;
				draw();
				
				if(_debuggerUp)
				{
					_debugger.perf.flash(elapsedMS);
					_debugger.perf.visibleObjects(FlxBasic._VISIBLECOUNT);
					_debugger.perf.update();
					_debugger.watch.update();
				}
			}
            */
        }

        /// <summary>
        /// If there is a state change requested during the update loop,
        /// this function handles actual destroying the old state and related processes,
        /// and calls creates on the new state and plugs it into the game object.
        /// </summary>
        protected void switchState()
        {
            FlxG.resetCameras();
            FlxG.resetInput();
            FlxG.destroySounds();
            //FlxG.clearBitmapCache();

			// Clear the debugger overlay's Watch window
            //if(_debugger != null)
            //    _debugger.watch.removeAll();
			
			// Clear any timers left in the timer manager
            //var timerManager:TimerManager = FlxTimer.manager;
            //if(timerManager != null)
            //    timerManager.clear();

            // Destroy the old state (if there is an old state)
            if (State != null)
            {
                State.destroy();
            }

            // Finally assign and create the new state
            State = RequestedState;
            State.create();
        }

        /// <summary>
        /// This is the main game update logic section.
        /// The onEnterFrame() handler is in charge of calling this
        /// the appropriate number of times each frame.
        /// This block handles state changes, replays, all that good stuff.
        /// </summary>
        protected internal void Step()
        {
            if (RequestedReset)
            {
                RequestedReset = false;
                // flx# - create new instance from initialState, otherwise this wont work...
                RequestedState = InitialState;
                //_replayTimer = 0;
                //_replayCancelKeys = null;
                FlxG.reset();
            }

            // handle replay-related requests
            /*
            if (_recordingRequested)
            {
                _recordingRequested = false;
                _replay.create(FlxG.globalSeed);
                _recording = true;
                if (_debugger != null)
                {
                    _debugger.vcr.recording();
                    FlxG.log("FLIXEL: starting new flixel gameplay record.");
                }
            }
            else if (_replayRequested)
            {
                _replayRequested = false;
                _replay.rewind();
                FlxG.globalSeed = _replay.seed;
                if (_debugger != null)
                    _debugger.vcr.playing();
                _replaying = true;
            }
            */

            // handle state switching requests
            if (State != RequestedState)
            {
                switchState();
            }

            // finally actually step through the game physics
            FlxBasic.activeCount = 0;
            /*
            if(_replaying)
			{
				_replay.playNextFrame();
				if(_replayTimer > 0)
				{
					_replayTimer -= _step;
					if(_replayTimer <= 0)
					{
						if(_replayCallback != null)
						{
							_replayCallback();
							_replayCallback = null;
						}
						else
							FlxG.stopReplay();
					}
				}
				if(_replaying && _replay.finished)
				{
					FlxG.stopReplay();
					if(_replayCallback != null)
					{
						_replayCallback();
						_replayCallback = null;
					}
				}
				if(_debugger != null)
					_debugger.vcr.updateRuntime(_step);
			}
			else
            */
            {
                FlxG.updateInput();
            }

            /*
			if(_recording)
			{
				_replay.recordFrame();
				if(_debugger != null)
					_debugger.vcr.updateRuntime(_step);
			}
            */

            update();
            FlxG.mouse.wheel = 0;
            //if (_debuggerUp)
            //    _debugger.perf.activeObjects(FlxBasic._ACTIVECOUNT);
        }

        /// <summary>
        /// This function just updates the soundtray object.
        /// </summary>
        /// <param name="ms"></param>
        protected void updateSoundTray(float ms)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This function is called by step() and updates the actual game state.
        /// May be called multiple times per "frame" or draw call.
        /// </summary>
        protected internal void update()
        {
            //var mark:uint = getTimer();
			//FlxG.elapsed = FlxG.timeScale * (_step / 1000);
            FlxG.getTimer = (uint) FlxS.GameTime.TotalGameTime.TotalMilliseconds;
            FlxG.elapsed = /* FlxG.timeScale * */ (float) FlxS.GameTime.ElapsedGameTime.TotalSeconds;

			FlxG.updateSounds();
			FlxG.updatePlugins();
			State.update();
			FlxG.updateCameras();
			
			//if(_debuggerUp)
			//	_debugger.perf.flixelUpdate(getTimer()-mark);
        }

        /// <summary>
        /// Goes through the game state and draws all the game objects and special effects.
        /// </summary>
        protected internal void draw()
        {
			//var mark:uint = getTimer();
			
            FlxG.lockCameras(); // Nothing happens here...
			State.draw();
			FlxG.drawPlugins();
			FlxG.unlockCameras(); // Draw camera FX

			//if(_debuggerUp)
			//	_debugger.perf.flixelDraw(getTimer()-mark);
        }

        /// <summary>
        /// Used to instantiate the guts of the flixel game object once we have a valid reference to the root.
        /// </summary>
        /// <param name="flashEvent">Just a Flash system event, not too important for our purposes.</param>
        protected void create(EventArgs flashEvent)
        {
            /*
            if(root == null)
				return;
			removeEventListener(Event.ENTER_FRAME, create);
			_total = getTimer();
			
			//Set up the view window and double buffering
			stage.scaleMode = StageScaleMode.NO_SCALE;
            stage.align = StageAlign.TOP_LEFT;
            stage.frameRate = _flashFramerate;
			
			//Add basic input event listeners and mouse container
			stage.addEventListener(MouseEvent.MOUSE_DOWN, onMouseDown);
			stage.addEventListener(MouseEvent.MOUSE_UP, onMouseUp);
			stage.addEventListener(MouseEvent.MOUSE_WHEEL, onMouseWheel);
			stage.addEventListener(KeyboardEvent.KEY_DOWN, onKeyDown);
			stage.addEventListener(KeyboardEvent.KEY_UP, onKeyUp);
			addChild(_mouse);
			
			//Let mobile devs opt out of unnecessary overlays.
			if(!FlxG.mobile)
			{
				//Debugger overlay
				if(FlxG.debug || forceDebugger)
				{
					_debugger = new FlxDebugger(FlxG.width*FlxCamera.defaultZoom,FlxG.height*FlxCamera.defaultZoom);
					addChild(_debugger);
				}
				
				//Volume display tab
				createSoundTray();
				
				//Focus gained/lost monitoring
				stage.addEventListener(Event.DEACTIVATE, onFocusLost);
				stage.addEventListener(Event.ACTIVATE, onFocus);
				createFocusScreen();
			}
			
			//Finally, set up an event for the actual game loop stuff.
			addEventListener(Event.ENTER_FRAME, onEnterFrame);
            */

            createSoundTray();
            createFocusScreen();
        }

        /// <summary>
        /// Sets up the "sound tray", the little volume meter that pops down sometimes.
        /// </summary>
        private void createSoundTray()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets up the darkened overlay with the big white "play" button that appears when a flixel game loses focus.
        /// </summary>
        private void createFocusScreen()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Required XNA initialization along with some FlxG initializations.
        /// </summary>
        /*
        protected override void Initialize()
        {
            base.Initialize();
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Basic FlxG inits that we need before we can call FlxG.init()
            FlxG.graphicsDevice = GraphicsDevice;
            FlxG.graphics = graphics;
            FlxG.content = Content;
            FlxG.spriteBatch = spriteBatch;
            FlxG.viewport = FlxG.graphicsDevice.Viewport;
            FlxG.width = graphics.PreferredBackBufferWidth;
            FlxG.height = graphics.PreferredBackBufferHeight;

            //Thread tInit = new Thread(FlxG.init);
            //tInit.Start();

            FlxG.init(this, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, 1);
        }
        */

        /// <summary>
        /// Update FlxG which updates important mechanics such as cameras and time elapsed.
        /// </summary>
        /*
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            //FlxG.gametime(gameTime);
            FlxG.update(gameTime);
        }
        */

        /// <summary>
        /// Draw everything - mainly the state and all of its objects.
        /// </summary>
        /*
        protected override void Draw(GameTime gameTime)
        {
            //Thread d = new Thread(FlxG.state.draw);
            //d.Start();
            //FlxG.state.draw();
        }
        */
    }
}
