using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace flxSharp.flxSharp.System
{
    public class FlxGameRunner : Game
    {
        private readonly int _gameSizeX;
        private readonly int _gameSizeY;

        private readonly int _screenSizeX;
        private readonly int _screenSizeY;
        private readonly Rectangle _outputWindowSize;

        private readonly FlxState _initialState;
        private readonly float _zoom;
        private readonly int _gameFramerate;
        private readonly int _flashFramerate;
        private readonly bool _useSystemCursor;
        private FlxGame _flxGame;

        private Texture2D _fxHelperTexture;


        /// <summary>
        /// Setup the XNA graphics system and create a new <code>FlxGame</code> from the given arguments;
        /// </summary>
        /// <param name="gameSizeX">The width of your game in game pixels, not necessarily final display pixels (see Zoom).</param>
        /// <param name="gameSizeY">The height of your game in game pixels, not necessarily final display pixels (see Zoom).</param>
        /// <param name="initialState">The class name of the state you want to create and switch to first (e.g. MenuState).</param>
        /// <param name="zoom">The default level of zoom for the game's cameras (e.g. 2 = all pixels are now drawn at 2x). Default = 1.</param>
        /// <param name="gameFramerate">How frequently the game should update (default is 60 times per second).</param>
        /// <param name="flashFramerate">Sets the actual display framerate for Flash player (default is 30 times per second).</param>
        /// <param name="useSystemCursor">Whether to use the default OS mouse pointer, or to use custom flixel ones.</param>
        public FlxGameRunner(
            int gameSizeX,
            int gameSizeY,
            FlxState initialState,
            float zoom = 1.0f,
            int gameFramerate = 60,
            int flashFramerate = 0,
            bool useSystemCursor = false)
        {
            _gameSizeX = gameSizeX;
            _gameSizeY = gameSizeY;
            _initialState = initialState;
            _zoom = zoom;
            _gameFramerate = gameFramerate;
            _flashFramerate = flashFramerate;
            _useSystemCursor = useSystemCursor;

            _screenSizeX = (int) (_gameSizeX * zoom);
            _screenSizeY = (int) (_gameSizeY * zoom);
            _outputWindowSize = new Rectangle(0, 0, _screenSizeX, _screenSizeY);

            FlxS.GraphicsDeviceManager = new GraphicsDeviceManager(this);

            // we don't need no new-fangled pixel processing
            // in our retro engine! (xnaFlixel ProTip! ;))
            FlxS.GraphicsDeviceManager.PreferMultiSampling = false;

            FlxS.GraphicsDeviceManager.PreferredBackBufferWidth = _screenSizeX;
            FlxS.GraphicsDeviceManager.PreferredBackBufferHeight = _screenSizeY;
            FlxS.GraphicsDeviceManager.ApplyChanges();
            
            Content.RootDirectory = "Content";
            FlxS.ContentManager = Content;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            FlxS.GraphicsDevice = GraphicsDevice;
            FlxS.GraphicsDevice.Viewport = new Viewport(0, 0, _screenSizeX, _screenSizeY);

            FlxS.RenderTarget = new RenderTarget2D(
                FlxS.GraphicsDevice,
                _gameSizeX,
                _gameSizeY,
                false,
                FlxS.GraphicsDevice.DisplayMode.Format,
                DepthFormat.None,
                0,
                RenderTargetUsage.DiscardContents);
            
            _fxHelperTexture = new Texture2D(FlxS.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _fxHelperTexture.SetData(new [] { Color.White });

            FlxS.SpriteBatch = new SpriteBatch(GraphicsDevice);

            _flxGame = new FlxGame(
                _gameSizeX,
                _gameSizeY,
                _initialState,
                _zoom,
                _gameFramerate,
                _flashFramerate,
                _useSystemCursor);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                this.Exit();                
            }

            // TODO: Add your update logic here
            FlxS.GameTime = gameTime;

            _flxGame.Step();

            base.Update(gameTime);
        }

        private void BasicDraw()
        {
            FlxS.GraphicsDevice.Clear(Color.CornflowerBlue);
            FlxS.SpriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend);
            _flxGame.draw();
            FlxS.SpriteBatch.End();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //BasicDraw();
            //base.Draw(gameTime);
            //return;

            FlxS.GraphicsDevice.SetRenderTarget(FlxS.RenderTarget);

            // first clear the screen with the background color of your choice
            FlxS.GraphicsDevice.Clear(FlxG.bgColor); // Protip: Check BackgroundColor if nothing is visible!
            //FlxS.GraphicsDevice.Clear(Color.CornflowerBlue);

            // SpriteSortMode.Texture - Check speed gain

            FlxS.SpriteBatch.Begin(SpriteSortMode.Immediate,
                                   BlendState.AlphaBlend,
                                   SamplerState.PointClamp,
                                   //PointClamp makes sure that the tiles render properly without tearing
                                   null,
                                   null,
                                   null,
                                   Matrix.Identity);
                                   //FlxG.camera.FxMatrix);
                                   // rotate + scale can be easily done in the spritebatch.draw calls
                                   // but maybe the translation can be nicely done here...
            
            _flxGame.draw();
            FlxS.SpriteBatch.End();

            FlxS.GraphicsDevice.SetRenderTarget(null);
            
            /*
            using (Stream sOut = File.Create(Path.GetTempFileName() + ".png"))
            {
                FlxS.RenderTarget.SaveAsPng(sOut, _screenSizeX, _screenSizeY);                
            }
            */

            FlxS.GraphicsDevice.Clear(Color.Black);

            foreach (FlxCamera camera in FlxG.cameras)
            {
                FlxS.SpriteBatch.Begin(
                    SpriteSortMode.Deferred,
                    BlendState.NonPremultiplied,
                    SamplerState.PointClamp,
                    null,
                    null,
                    null, // suitable place for post processing effects
                    camera.FxMatrix);
                    //Matrix.Identity);

                // Draw camera background
                FlxS.SpriteBatch.Draw(
                    _fxHelperTexture,
                    camera.ScreenRect,
                    camera.BgColor);

                // Draw (and scale) backbuffer to screen
                FlxS.SpriteBatch.Draw(
                    FlxS.RenderTarget,
                    camera.ScreenRect,
                    camera.CameraRect,
                    Color.White);

                FlxS.SpriteBatch.End();

            }

            //FlxS.SpriteBatch.Draw(FlxS.RenderTarget, _outputWindowSize, Color.White);

            base.Draw(gameTime);
        }
    }
}
