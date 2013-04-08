using flxSharp.flxSharp.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace flxSharp.flxSharp
{
    public class FlxSprite : FlxObject
    {
        /// <summary>
        /// Default image.
        /// </summary>
        protected static readonly Texture2D ImgDefault = FlxS.ContentManager.Load<Texture2D>("default");

        /// <summary>
        /// WARNING: The origin of the sprite will default to its center.
        /// If you change this, the visuals and the collisions will likely be
        /// pretty out-of-sync if you do any rotation.
        /// </summary>
        public FlxPoint Origin;

        /// <summary>
        /// If you changed the size of your sprite object after loading or making the graphic,
        /// you might need to offset the graphic away from the bound box to center it the way you want.
        /// </summary>
        public FlxPoint Offset;

        /// <summary>
        /// Change the size of your sprite's graphic.
        /// NOTE: Scale doesn't currently affect collisions automatically,
        /// you will need to adjust the width, height and offset manually.
        /// WARNING: scaling sprites decreases rendering performance for this sprite by a factor of 10x!
        /// </summary>
        public FlxPoint Scale;

        /// <summary>
        /// Blending modes, just like Photoshop or whatever.
        /// E.g. "multiply", "screen", etc.
        /// </summary>
        public string Blend { get; set; }

        /// <summary>
        /// Controls whether the object is smoothed when rotated, affects performance.
        /// </summary>
        public bool AntiAliasing { get; set; }

        /// <summary>
        /// Whether the current animation has finished its first (or only) loop.
        /// </summary>
        public Boolean Finished;

        /// <summary>
        /// The width of the actual graphic or image being displayed (not necessarily the game object/bounding box).
        /// NOTE: Edit at your own risk!!  This is intended to be read-only.
        /// </summary>
        public float FrameWidth;
        
        /// <summary>
        /// The height of the actual graphic or image being displayed (not necessarily the game object/bounding box).
        /// NOTE: Edit at your own risk!!  This is intended to be read-only.
        /// </summary>
        public float FrameHeight;
        
        /// <summary>
        /// The total number of frames in this image.  WARNING: assumes each row in the sprite sheet is full!
        /// </summary>
        public uint Frames;

        /// <summary>
        /// The actual Flash <code>BitmapData</code> object representing the current display state of the sprite.
        /// </summary>
        public Texture2D FramePixels;

        /// <summary>
        /// Set this flag to true to force the sprite to update during the draw() call.
        /// NOTE: Rarely if ever necessary, most sprite operations will flip this flag automatically.
        /// </summary>
        public Boolean Dirty { get; set; }
        
        /// <summary>
        /// Internal, stores all the animations that were added to this sprite.
        /// </summary>
        protected List<FlxAnim> _animations;
        
        /// <summary>
        /// Internal, keeps track of whether the sprite was loaded with support for automatic reverse/mirroring.
        /// </summary>
        protected uint _flipped;
        
        /// <summary>
        /// Internal, keeps track of the current animation being played.
        /// </summary>
        protected FlxAnim _curAnim;
        
        /// <summary>
        /// Internal, keeps track of the current frame of animation.
        /// This is NOT an index into the tile sheet, but the frame number in the animation object.
        /// </summary>
        protected int _curFrame;
        
        /// <summary>
        /// Internal, keeps track of the current index into the tile sheet based on animation or rotation.
        /// </summary>
        protected int _curIndex;
        
        /// <summary>
        /// Internal, used to time each frame of animation.
        /// </summary>
        protected float _frameTimer;
        
        /// <summary>
        /// Internal tracker for the animation callback. Default is null.
        /// If assigned, will be called each time the current frame changes.
        /// A function that has 3 parameters: a string name, a uint frame number, and a uint frame index.
        /// </summary>
        protected Action<String, uint, uint> _callback;
        
        /// <summary>
        /// Internal tracker for what direction the sprite is currently facing, used with Flash getter/setter.
        /// </summary>
        protected uint _facing;

        /// <summary>
        /// Set <code>facing</code> using <code>FlxSprite.LEFT</code>,<code>RIGHT</code>,
        /// <code>UP</code>, and <code>DOWN</code> to take advantage of
        /// flipped sprites and/or just track player orientation more easily.
        /// </summary>
        public uint Facing
        {
            get { return _facing; }
            set
            {
                if (value != _facing)
                {
                    Dirty = true;
                }

                _facing = value;
            }
        }

        /// <summary>
        /// Internal tracker for opacity, used with Flash getter/setter.
        /// </summary>
        protected float _alpha;

        /// <summary>
        /// Set <code>alpha</code> to a number between 0 and 1 to change the opacity of the sprite.
        /// </summary>
        public float Alpha
        {
            get { return _alpha; }
            set
            {
                if (value == _alpha) return;
                if (value > 1) value = 1;
                if (value < 0) value = 0;

                // flx# - alpha is applied during draw() by multiplying with color
                _alpha = value;

                /*
			    if((_alpha != 1) || (_color != 0x00ffffff))
				    _colorTransform = new ColorTransform((_color>>16)*0.00392,(_color>>8&0xff)*0.00392,(_color&0xff)*0.00392,_alpha);
			    else
				    _colorTransform = null; 
                */

                Dirty = true;
            }
        }

        /// <summary>
        /// Internal tracker for color tint, used with Flash getter/setter.
        /// </summary>
        protected Color _color;

        /// <summary>
        /// Set <code>color</code> to a number in this format: 0xRRGGBB.
        /// <code>color</code> IGNORES ALPHA.  To change the opacity use <code>alpha</code>.
        /// Tints the whole sprite to be this color (similar to OpenGL vertex colors).
        /// </summary>
        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value;

                /*
                if (value == _color) return;

                value.A = 0;
                _color = value;
                */

                /*
		        if((_alpha != 1) || (_color != 0x00ffffff))
			        _colorTransform = new ColorTransform((_color>>16)*0.00392,(_color>>8&0xff)*0.00392,(_color&0xff)*0.00392,_alpha);
		        else
			        _colorTransform = null; 
                */

                Dirty = true;
            }
        }

        /// <summary>
        /// Internal, stores the entire source graphic (not the current displayed animation frame), used with Flash getter/setter.
        /// </summary>
        protected Texture2D _pixels;

        /// <summary>
        /// Set <code>pixels</code> to any <code>BitmapData</code> object.
        /// Automatically adjust graphic size and render helpers.
        /// </summary>
        public Texture2D Pixels
        {
            get { return _pixels; }
            set
            {
                _pixels = value;

                Width = _pixels.Width;
                FrameWidth = _pixels.Width;

                Height = _pixels.Height;
                FrameHeight = _pixels.Height;

                resetHelpers();
            }
        }

        /// <summary>
        /// Tell the sprite to change to a specific frame of animation.
        /// </summary>
        public int Frame
        {
            get { return _curIndex; }
            set
            {
                _curAnim = null;
                _curIndex = value;
                Dirty = true;
            }
        }

        /// <summary>
        /// Internal tracker for how many frames of "baked" rotation there are (if any).
        /// </summary>
        protected float _bakedRotation;

        /// <summary>
        /// Internal, reused frequently during drawing and animating.
        /// </summary>
        protected FlxPoint _flashPoint;

        /// <summary>
        /// Internal, reused frequently during drawing and animating.
        /// </summary>
        protected FlxRect _flashRect;

        /// <summary>
        /// Internal, reused frequently during drawing and animating.
        /// </summary>
        protected FlxRect _flashRect2;

        /// <summary>
        /// Internal, reused frequently during drawing and animating. Always contains (0,0).
        /// </summary>
        protected FlxPoint _flashPointZero;

        ///**
        // * Internal, helps with animation, caching and drawing.
        // */
        //protected var _colorTransform:ColorTransform;

        /// <summary>
        /// Internal, helps with animation, caching and drawing.
        /// </summary>
        protected Matrix _matrix;
        
        // flx# stuff below

        protected Rectangle drawSourceRect;

        protected Boolean _animated;
        protected FlxRect sourceRect;
        private Texture2D texture;

        //scrollfactor experimentation
        protected float camX;
        protected float camY;
        protected float oX;
        protected float oY;
        public bool moving;


        /// <summary>
        /// Creates a white 8x8 square <code>FlxSprite</code> at the specified position.
        /// Optionally can load a simple, one-frame graphic instead.
        /// </summary>
        /// <param name="x">The initial X position of the sprite.</param>
        /// <param name="y">The initial Y position of the sprite.</param>
        /// <param name="graphic">The graphic you want to display (OPTIONAL - for simple stuff only, do NOT use for animated images!).</param>
        public FlxSprite(float x = 0, float y = 0, Texture2D graphic = null)
            : base(x, y)
        {
            Health = 1;

            _flashPoint = new FlxPoint();
            _flashRect = new FlxRect();
            _flashRect2 = new FlxRect();
            _flashPointZero = new FlxPoint(0, 0);
            Offset = new FlxPoint();
            Origin = new FlxPoint();

            Scale = new FlxPoint(1, 1);
            Alpha = 1;
            Color = Color.White;
            Blend = null;
            AntiAliasing = false;
            Cameras = null;

            Finished = false;
            _facing = Right;
            _animations = new List<FlxAnim>();
            _flipped = 0;
            _curAnim = null;
            _curFrame = 0;
            _curIndex = 0;
            _frameTimer = 0;

            _matrix = new Matrix();
            _callback = null;

            if (graphic == null)
            {
                graphic = ImgDefault;
            }

            loadGraphic(graphic);

            // flx# stuff
            Angle = 0f;
            camX = camY = 0;
            oX = x;
            oY = y;
            moving = false;

            /*
            Scale = new FlxPoint(1.0f, 1.0f);
            Offset = new FlxPoint();
            Origin = new FlxPoint();
            alpha = 1.0f;
            _color = Color.White * alpha;

            _animations = new List<FlxAnim>();
            _animated = false;

            Finished = false;
            _facing = Right;
            _flipped = 0;
            _curAnim = null;
            _curFrame = 0;
            _curIndex = 0;
            _frameTimer = 0;

            _callback = null;
            _matrix = new Matrix();

            if (graphic == null)
                graphic = ImgDefault;
            loadGraphic(graphic);

            Angle = 0f;

            camX = camY = 0;
            oX = x;
            oY = y;

            moving = false;
            */
        }

        public override void destroy()
        {
            // flx# - can animations in _animations be null?
            /*
            if(_animations != null)
			{
				FlxAnim a;
				int i = 0;
				int l = _animations.Count;
				while(i < l)
				{
					a = _animations[i++];
					if(a != null)
						a.destroy();
				}
				_animations = null;
			}
			*/

            if (_animations != null)
            {
                foreach (FlxAnim animation in _animations)
                {
                    animation.destroy();
                }
            }

            _flashPoint = null;
            _flashRect = null;
            _flashRect2 = null;
            _flashPointZero = null;

			Offset = null;
			Origin = null;
			Scale = null;

			_curAnim = null;
            //_matrix = null; // flx# - matrix is a struct
			_callback = null;
            //_framePixels = null; // flx# - unused
        }

        /// <summary>
        /// Load an image from an embedded graphic file.
        /// </summary>
        /// <param name="graphic">The image you want to use.</param>
        /// <param name="animated">Whether the Graphic parameter is a single sprite or a row of sprites.</param>
        /// <param name="reverse">Whether you need this class to generate horizontally flipped versions of the animation frames.</param>
        /// <param name="width">Optional, specify the width of your sprite (helps FlxSprite figure out what to do with non-square sprites or sprite sheets).</param>
        /// <param name="height">Optional, specify the height of your sprite (helps FlxSprite figure out what to do with non-square sprites or sprite sheets).</param>
        /// <param name="unique">Optional, whether the graphic should be a unique instance in the graphics cache.  Default is false.</param>
        /// <returns></returns>
        public FlxSprite loadGraphic(Texture2D graphic, Boolean animated = false, Boolean reverse = false, float width = 0, float height = 0, bool unique = false)
        {
            if (unique || reverse)
            {
                throw new NotSupportedException();
            }

            _bakedRotation = 0;

            // flx# - if reversed, addBitmap doubles the texture width and draws
            // a flipped image beside the original one. thus all these * 2 / 2
            // calculations below
            //_pixels = FlxG.addBitmap(graphic, reverse, unique);

            _pixels = graphic;  
            
            if (reverse)
            {
                _flipped = (uint)_pixels.Width >> 1;
            }
            else
            {
                _flipped = 0;
            }

            if (width == 0)
            {
                if (animated)
                {
                    width = _pixels.Width;
                }
                else if (_flipped > 0)
                {
                    width = _pixels.Width * 0.5f;
                }
                else
                {
                    width = _pixels.Width;
                }
            }

            Width = width;
            FrameWidth = width;

            if (height == 0)
            {
                if (animated)
                {
                    height = width;
                }
                else
                {
                    height = _pixels.Height;
                }
            }

            Height = height;
            FrameHeight = height;

            resetHelpers();

            // flx# stuff
            drawSourceRect = new Rectangle(0, 0, (int)Width, (int)Height);

            _animated = animated;
            texture = graphic;
            sourceRect = new FlxRect(0, 0, width, height);

            return this;
        }

        /// <summary>
        /// Create a pre-rotated sprite sheet from a simple sprite.
        /// This can make a huge difference in graphical performance!
        /// </summary>
        /// <param name="graphic">The image you want to rotate and stamp.</param>
        /// <param name="rotations">The number of rotation frames the final sprite should have.  For small sprites this can be quite a large number (360 even) without any problems.</param>
        /// <param name="frame">If the Graphic has a single row of square animation frames on it, you can specify which of the frames you want to use here.  Default is -1, or "use whole graphic."</param>
        /// <param name="antiAliasing">Whether to use high quality rotations when creating the graphic.  Default is false.</param>
        /// <param name="autoBuffer">Whether to automatically increase the image size to accomodate rotated corners.  Default is false.  Will create frames that are 150% larger on each axis than the original frame or graphic.</param>
        /// <returns></returns>
        public FlxSprite loadRotatedGraphic(Texture2D graphic, uint rotations = 16, int frame = 1, bool antiAliasing = false, bool autoBuffer = false)
        {
            throw new NotSupportedException("RotationIsPieceOfCakeForMightyXNA");

            /*
			//Create the brush and canvas
			var rows:uint = Math.sqrt(Rotations);
			var brush:BitmapData = FlxG.addBitmap(Graphic);
			if(Frame >= 0)
			{
				//Using just a segment of the graphic - find the right bit here
				var full:BitmapData = brush;
				brush = new BitmapData(full.height,full.height);
				var rx:uint = Frame*brush.width;
				var ry:uint = 0;
				var fw:uint = full.width;
				if(rx >= fw)
				{
					ry = uint(rx/fw)*brush.height;
					rx %= fw;
				}
				_flashRect.x = rx;
				_flashRect.y = ry;
				_flashRect.width = brush.width;
				_flashRect.height = brush.height;
				brush.copyPixels(full,_flashRect,_flashPointZero);
			}
			
			var max:uint = brush.width;
			if(brush.height > max)
				max = brush.height;
			if(AutoBuffer)
				max *= 1.5;
			var columns:uint = FlxU.ceil(Rotations/rows);
			width = max*columns;
			height = max*rows;
			var key:String = String(Graphic) + ":" + Frame + ":" + width + "x" + height;
			var skipGen:Boolean = FlxG.checkBitmapCache(key);
			_pixels = FlxG.createBitmap(width, height, 0, true, key);
			width = frameWidth = _pixels.width;
			height = frameHeight = _pixels.height;
			_bakedRotation = 360/Rotations;
			
			//Generate a new sheet if necessary, then fix up the width and height
			if(!skipGen)
			{
				var row:uint = 0;
				var column:uint;
				var bakedAngle:Number = 0;
				var halfBrushWidth:uint = brush.width*0.5;
				var halfBrushHeight:uint = brush.height*0.5;
				var midpointX:uint = max*0.5;
				var midpointY:uint = max*0.5;
				while(row < rows)
				{
					column = 0;
					while(column < columns)
					{
						_matrix.identity();
						_matrix.translate(-halfBrushWidth,-halfBrushHeight);
						_matrix.rotate(bakedAngle*0.017453293);
						_matrix.translate(max*column+midpointX, midpointY);
						bakedAngle += _bakedRotation;
						_pixels.draw(brush,_matrix,null,null,null,AntiAliasing);
						column++;
					}
					midpointY += max;
					row++;
				}
			}
			frameWidth = frameHeight = width = height = max;
			resetHelpers();
			if(AutoBuffer)
			{
				width = brush.width;
				height = brush.height;
				centerOffsets();
			}
			return this;
            */
        }

        /// <summary>
        /// This function creates a flat colored square image dynamically.
        /// </summary>
        /// <param name="width">The width of the sprite you want to generate.</param>
        /// <param name="height">The height of the sprite you want to generate.</param>
        /// <param name="color">Specifies the color of the generated block.</param>
        /// <param name="unique">Whether the graphic should be a unique instance in the graphics cache. Default is false.</param>
        /// <param name="key">Optional parameter - specify a string key to identify this graphic in the cache. Trumps Unique flag.</param>
        /// <returns></returns>
        public FlxSprite makeGraphic(uint width, uint height, Color color, bool unique = false, string key = null)
        {
            if (unique || !string.IsNullOrEmpty(key))
            {
                throw new NotSupportedException();    
            }

            _bakedRotation = 0;
            _pixels = FlxG.createBitmap(width, height, color, unique, key);

            Width = width;
            FrameWidth = width;
            Height = height;
            FrameHeight = height;

            resetHelpers();

            // flx# stuff
            drawSourceRect = new Rectangle(0, 0, (int)width, (int)height);

            texture = _pixels;
            sourceRect = new FlxRect(0, 0, width, height);

            return this;
        }

        /// <summary>
        /// Resets some important variables for sprite optimization and rendering.
        /// </summary>
        protected void resetHelpers()
        {
            _flashRect.X = 0;
            _flashRect.Y = 0;
            _flashRect.Width = FrameWidth;
            _flashRect.Height = FrameHeight;

            _flashRect2.X = 0;
            _flashRect2.Y = 0;
            _flashRect2.Width = _pixels.Width;
            _flashRect2.Height = _pixels.Height;

            /*
            if ((framePixels == null) || (framePixels.width != width) || (framePixels.height != height))
                framePixels = new BitmapData(width, height);
            */

            Origin.make(FrameWidth * 0.5f, FrameHeight * 0.5f);

            //framePixels.copyPixels(_pixels, _flashRect, _flashPointZero);
            Frames = (uint)((_flashRect2.Width / _flashRect.Width) * (_flashRect2.Height / _flashRect.Height));
            //if (_colorTransform != null) framePixels.colorTransform(_flashRect, _colorTransform);

            _curIndex = 0;
        }

        /// <summary>
        /// Automatically called after update() by the game loop,
        /// this function just calls updateAnimation().
        /// </summary>
        public override void postUpdate()
        {
            base.postUpdate();
            updateAnimation();
        }

        /// <summary>
        /// Called by game loop, updates then blits or renders current frame of animation to the screen
        /// </summary>
        public override void draw()
        {
            if (_flickerTimer != 0)
            {
                _flicker = !_flicker;
                if (_flicker)
                {
                    return;
                }
            }

            // rarely
            if (Dirty)
            {
                calcFrame();                
            }

            if (Cameras == null)
            {
                Cameras = FlxG.cameras;                
            }

            foreach (FlxCamera camera in Cameras)
            {
                if (!onScreen(camera))
                {
                    continue;
                }

                _tagPoint.X = X - (int)(camera.Scroll.X * ScrollFactor.X) - Offset.X;
                _tagPoint.Y = Y - (int)(camera.Scroll.Y * ScrollFactor.Y) - Offset.Y;
                _tagPoint.X += (_tagPoint.X > 0) ? 0.0000001f : -0.0000001f;
                _tagPoint.Y += (_tagPoint.Y > 0) ? 0.0000001f : -0.0000001f;
                
                if (Visible)
                {
                    if (texture == null || _pixels == null)
                    {
                        throw new Exception("Cannot draw anything without a texture!");
                    }

                    /*
				    {	//Advanced render
					    _matrix.identity();
					    _matrix.translate(-origin.x,-origin.y);
					    _matrix.scale(scale.x,scale.y);
					    if((angle != 0) && (_bakedRotation <= 0))
						    _matrix.rotate(angle * 0.017453293);
					    _matrix.translate(_point.x+origin.x,_point.y+origin.y);
					    camera.buffer.draw(framePixels,_matrix,null,blend,null,antialiasing);
				    }
                     */
                    Vector2 halfSize = new Vector2(sourceRect.Width / 2, sourceRect.Height / 2);
                    Vector2 vecOrigin = new Vector2(Origin.X, Origin.Y);


                    //Rectangle sourceRectangle = new Rectangle(0, 0, (int)FrameWidth, (int)FrameHeight);
                    Rectangle sourceRectangle = drawSourceRect;

                    //Vector2 position = new Vector2(X + FrameWidth / 2, Y + FrameWidth / 2);
                    //Vector2 position = new Vector2(X, Y);

                    // add origin to reverse the translation brought up by the origin in the origin... you know ;)
                    Vector2 position = new Vector2(_tagPoint.X, _tagPoint.Y) + vecOrigin;

                    //Vector2 origin = new Vector2(FrameWidth / 2, FrameHeight / 2);
                    Vector2 origin = new Vector2(Offset.X, Offset.Y) + vecOrigin;
                    //Debug.WriteLine(origin);
                    
                    Vector2 scale = new Vector2(Scale.X, Scale.Y);
                    // flx# - use sprite effects to flip horizontal/vertical

                    //Color drawColor = new Color(1f, 1f, 1f, 0f);
                    //Color drawColor = Color.White * _alpha;
                    Color drawColor = Color * _alpha;

                    FlxS.SpriteBatch.Draw(
                        _pixels,
                        position,
                        sourceRectangle,
                        drawColor,
                        Angle,
                        origin,
                        scale,
                        SpriteEffects.None,
                        0);

                    /*
                    FlxS.SpriteBatch.Draw(
                        _pixels,
                        position,
                        sourceRectangle,
                        Color.White);
                    */

                    /*
                    base.draw();
                    if (texture != null)
                    {
                        //if the sprite is animated then the sourceRect needs to be changed to be the correct frame
                        if (_animated)
                            sourceRect = new FlxRect(FrameWidth * _curIndex, 0, FrameWidth, FrameHeight);
                        Rectangle rect = new Rectangle((int)sourceRect.x, (int)sourceRect.y, (int)sourceRect.width, (int)sourceRect.height);
                        FlxG.spriteBatch.Draw(texture, getVec2(), rect, Color * alpha, Angle, new Vector2(), Scale.getVec2(), SpriteEffects.None, 0f);
                    }
                    */

                    visibleCount++;
                    if (FlxG.visualDebug && !IgnoreDrawDebug)
                    {
                        drawDebug(camera);
                    }
                }

            }

            /*
            FlxCamera camera = FlxG.camera;
            int i = 0;
            int l = Cameras.Count;
            while (i < l)
            {
                camera = Cameras[i++];
                //camera = FlxG.camera;
                if (!onScreen(camera))
                {
                    continue;
                }
                _tagPoint.x = X - (int)(camera.scroll.x * ScrollFactor.x) - Offset.x;
                _tagPoint.y = Y - (int)(camera.scroll.y * ScrollFactor.y) - Offset.y;
                _tagPoint.x += (_tagPoint.x > 0) ? 0.0000001f : -0.0000001f;
                _tagPoint.y += (_tagPoint.y > 0) ? 0.0000001f : -0.0000001f;
                if (Visible)
                {
                    base.draw();
                    if (texture != null)
                    {
                        //if the sprite is animated then the sourceRect needs to be changed to be the correct frame
                        if (_animated)
                            sourceRect = new FlxRect(FrameWidth * _curIndex, 0, FrameWidth, FrameHeight);
                        Rectangle rect = new Rectangle((int)sourceRect.x, (int)sourceRect.y, (int)sourceRect.width, (int)sourceRect.height);
                        FlxG.spriteBatch.Draw(texture, getVec2(), rect, Color * alpha, Angle, new Vector2(), Scale.getVec2(), SpriteEffects.None, 0f);
                    }
                }
            }
            */
        }

        /// <summary>
        /// This function draws or stamps one <code>FlxSprite</code> onto another.
        /// This function is NOT intended to replace <code>draw()</code>!
        /// </summary>
        /// <param name="brush">The image you want to use as a brush or stamp or pen or whatever.</param>
        /// <param name="x">The X coordinate of the brush's top left corner on this sprite.</param>
        /// <param name="y">They Y coordinate of the brush's top left corner on this sprite.</param>
        public void stamp(FlxSprite brush, int x = 0, int y = 0)
        {
            throw new NotImplementedException();

            /*
			Brush.drawFrame();
			var bitmapData:BitmapData = Brush.framePixels;
			
			//Simple draw
			if(((Brush.angle == 0) || (Brush._bakedRotation > 0)) && (Brush.scale.x == 1) && (Brush.scale.y == 1) && (Brush.blend == null))
			{
				_flashPoint.x = X;
				_flashPoint.y = Y;
				_flashRect2.width = bitmapData.width;
				_flashRect2.height = bitmapData.height;
				_pixels.copyPixels(bitmapData,_flashRect2,_flashPoint,null,null,true);
				_flashRect2.width = _pixels.width;
				_flashRect2.height = _pixels.height;
				calcFrame();
				return;
			}
			
			//Advanced draw
			_matrix.identity();
			_matrix.translate(-Brush.origin.x,-Brush.origin.y);
			_matrix.scale(Brush.scale.x,Brush.scale.y);
			if(Brush.angle != 0)
				_matrix.rotate(Brush.angle * 0.017453293);
			_matrix.translate(X+Brush.origin.x,Y+Brush.origin.y);
			_pixels.draw(bitmapData,_matrix,null,Brush.blend,null,Brush.antialiasing);
			calcFrame();
            */
        }

        /// <summary>
        /// This function draws a line on this sprite from position X1,Y1
        /// to position X2,Y2 with the specified color.
        /// </summary>
        /// <param name="startX">X coordinate of the line's start point.</param>
        /// <param name="startY">Y coordinate of the line's start point.</param>
        /// <param name="endX">X coordinate of the line's end point.</param>
        /// <param name="endY">Y coordinate of the line's end point.</param>
        /// <param name="color">The line's color.</param>
        /// <param name="thickness">How thick the line is in pixels (default value is 1).</param>
        public void drawLine(float startX, float startY, float endX, float endY, Color color, uint thickness = 1)
		{
            if (thickness > 1)
            {
                throw new NotImplementedException();
            }

            var texBuffer = new Color[_pixels.Width * _pixels.Height];
            _pixels.GetData<Color>(texBuffer);

            int posX = (int) startX;
            int posY = (int) startY;

            int dx = (int) Math.Abs(endX - startX);
            int sx = (startX < endX) ? 1 : -1;
            int dy = - (int) Math.Abs(endY - startY);
            int sy = (startY < endY) ? 1 : -1;
            int err = dx + dy;

            while(true)
            {
                texBuffer[posY * _pixels.Width + posX] = color;

                if ((posX == endX) && (posY == endY))
                {
                    break;
                }

                float e = 2 * err;
                
                if (e > dy)
                {
                    err += dy;
                    posX += sx;
                }

                if (e < dx)
                {
                    err += dx;
                    posY += sy;
                }
            }

            _pixels.SetData<Color>(texBuffer);

            // flx# - seems useless
            Dirty = true;
		}

        /// <summary>
        /// Fills this sprite's graphic with a specific color.
        /// </summary>
        /// <param name="color">The color with which to fill the graphic, format 0xAARRGGBB.</param>
        public void fill(Color color)
        {
            var colorData = new Color[_pixels.Width * _pixels.Height];
            for (int i = 0; i < colorData.Length; ++i)
            {
                colorData[i] = color;
            }
            _pixels.SetData<Color>(colorData);
        }

        /// <summary>
        /// Internal function for updating the sprite's animation.
        /// Useful for cases when you need to update this but are buried down in too many supers.
        /// This function is called automatically by <code>FlxSprite.postUpdate()</code>.
        /// </summary>
        protected void updateAnimation()
        {
            /*
            if(_bakedRotation > 0)
			{
				var oldIndex:uint = _curIndex;
				var angleHelper:int = angle%360;
				if(angleHelper < 0)
					angleHelper += 360;
				_curIndex = angleHelper/_bakedRotation + 0.5;
				if(oldIndex != _curIndex)
					dirty = true;
			}
            */

            if ((_curAnim != null) && (_curAnim.Delay > 0) && (_curAnim.Looped || !Finished))
            {
                _frameTimer = _frameTimer + FlxG.elapsed;
                while (_frameTimer > _curAnim.Delay)
                {
                    _frameTimer = _frameTimer - _curAnim.Delay;
                    if (_curFrame == _curAnim.Frames.Length - 1)
                    {
                        if (_curAnim.Looped)
                        {
                            _curFrame = 0;
                        }
                        Finished = true;
                    }
                    else
                    {
                        _curFrame++;                        
                    }

                    _curIndex = _curAnim.Frames[_curFrame];
                    Dirty = true;
                }
            }

            if (Dirty)
            {
                calcFrame();                
            }
        }

        /// <summary>
        /// Request (or force) that the sprite update the frame before rendering.
        /// Useful if you are doing procedural generation or other weirdness!
        /// </summary>
        /// <param name="force">Force the frame to redraw, even if its not flagged as necessary.</param>
        public void drawFrame(bool force = false)
        {
            if (force || Dirty)
            {
                calcFrame();
            }
        }

        /// <summary>
        /// Adds a new animation to the sprite.
        /// </summary>
        /// <param name="name">What this animation should be called (e.g. "run").</param>
        /// <param name="frames">An array of numbers indicating what frames to play in what order (e.g. 1, 2, 3).</param>
        /// <param name="frameRate">The speed in frames per second that the animation should play at (e.g. 40 fps).</param>
        /// <param name="looped">Whether or not the animation is looped or just plays once.</param>
        public void addAnimation(String name, int[] frames, float frameRate = 0, Boolean looped = true)
        {
            _animations.Add(new FlxAnim(name, frames, frameRate, looped));
        }

        /// <summary>
        /// Pass in a function to be called whenever this sprite's animation changes.
        /// </summary>
        /// <param name="animationCallback">A function that has 3 parameters: a string name, a uint frame number, and a uint frame index.</param>
        public void addAnimationCallback(Action<String, uint, uint> animationCallback)
        {
            // flx# - could be changed to a multicast event
            _callback = animationCallback;
        }

        /// <summary>
        /// Plays an existing animation (e.g. "run").
        /// If you call an animation that is already playing it will be ignored.
        /// </summary>
        /// <param name="animName">The string name of the animation you want to play.</param>
        /// <param name="force">Whether to force the animation to restart.</param>
        public void play(String animName, Boolean force = false)
        {
            if (!force && (_curAnim != null) && (animName == _curAnim.Name) && (_curAnim.Looped || !Finished)) return;
            _curFrame = 0;
            _curIndex = 0;
            _frameTimer = 0;

            foreach (FlxAnim animation in _animations)
            {
                if (animation.Name.Equals(animName))
                {
                    _curAnim = animation;

                    if (_curAnim.Delay <= 0)
                    {
                        Finished = true;
                    }
                    else
                    {
                        Finished = false;                        
                    }
                    
                    _curIndex = _curAnim.Frames[_curFrame];
                    Dirty = true;
                    return;
                }
            }

            FlxG.log("WARNING: No animation called \"" + animName + "\"");
        }

        /// <summary>
        /// Tell the sprite to change to a random frame of animation
        /// Useful for instantiating particles or other weird things.
        /// </summary>
        public void randomFrame()
        {
            _curAnim = null;
            _curIndex = (int) (FlxG.random() * (_pixels.Width / FrameWidth));
            Dirty = true;
        }

        /// <summary>
        /// Helper function that just sets origin to (0,0).
        /// </summary>
        public void setOriginToCorner()
        {
            Origin.X = 0;
            Origin.Y = 0;
        }

        /// <summary>
        /// Helper function that adjusts the offset automatically to center the bounding box within the graphic.
        /// </summary>
        /// <param name="adjustPosition">Adjusts the actual X and Y position just once to match the offset change. Default is false.</param>
        public void centerOffsets(bool adjustPosition = false)
        {
            Offset.X = (FrameWidth - Width) * 0.5f;
            Offset.Y = (FrameHeight - Height) * 0.5f;

            if (adjustPosition)
            {
                X = X + Offset.X;
                Y = Y + Offset.Y;
            }
        }

        /// <summary>
        /// Undocumented in Flixel.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="newColor"></param>
        /// <param name="fetchPositions"></param>
        /// <returns></returns>
        public List<FlxPoint> replaceColor(Color color, Color newColor, bool fetchPositions = false)
        {
            List<FlxPoint> positions = (fetchPositions) ? new List<FlxPoint>() : null;

            var colorBuffer = new Color[_pixels.Width * _pixels.Height];
            _pixels.GetData<Color>(colorBuffer);

            for (int y = 0; y < _pixels.Height; ++y)
            {
                for (int x = 0; x < _pixels.Width; ++x)
                {
                    int index = (y * _pixels.Width) + x;
                    Color c = colorBuffer[index];

                    // flx# - maybe can be replaced with c.Equals(color)
                    if (c.A == color.A &&
                        c.R == color.R &&
                        c.G == color.G &&
                        c.B == color.B)
                    {
                        colorBuffer[index] = newColor;

                        if (fetchPositions)
                        {
                            positions.Add(new FlxPoint(x, y));
                        }

                        Dirty = true;
                    }
                }
            }

            _pixels.SetData<Color>(colorBuffer);

            return positions;
        }

        /// <summary>
        /// Check and see if this object is currently on screen.
        /// Differs from <code>FlxObject</code>'s implementation
        /// in that it takes the actual graphic into account,
        /// not just the hitbox or bounding box or whatever.
        /// </summary>
        /// <param name="camera">Specify which game camera you want.  If null getScreenXY() will just grab the first global camera.</param>
        /// <returns>Whether the object is on screen or not.</returns>
        override public bool onScreen(FlxCamera camera = null)
        {
            if (camera == null)
            {
                camera = FlxG.camera;                
            }

            getScreenXY(_tagPoint, camera);
            _tagPoint.X = _tagPoint.X - Offset.X;
            _tagPoint.Y = _tagPoint.Y - Offset.Y;

            /*
            if (((angle == 0) || (_bakedRotation > 0)) && (scale.x == 1) && (scale.y == 1))
                return ((_point.x + frameWidth > 0) && (_point.x < Camera.width) && (_point.y + frameHeight > 0) && (_point.y < Camera.height));
            */

            float halfWidth = FrameWidth / 2;
            float halfHeight = FrameHeight / 2;
            float absScaleX = (Scale.X > 0) ? Scale.X : -Scale.X;
            float absScaleY = (Scale.Y > 0) ? Scale.Y : -Scale.Y;
            float radius = (float)Math.Sqrt((halfWidth * halfWidth) + (halfHeight * halfHeight)) * ((absScaleX >= absScaleY) ? absScaleX : absScaleY);
            _tagPoint.X = _tagPoint.X + halfWidth;
            _tagPoint.Y = _tagPoint.Y + halfHeight;
            return ((_tagPoint.X + radius > 0) && (_tagPoint.X - radius < camera.Width) && (_tagPoint.Y + radius > 0) && (_tagPoint.Y - radius < camera.Height));
        }

        /// <summary>
        /// Checks to see if a point in 2D world space overlaps this <code>FlxSprite</code> object's current displayed pixels.
        /// This check is ALWAYS made in screen space, and always takes scroll factors into account.
        /// </summary>
        /// <param name="point">The point in world space you want to check.</param>
        /// <param name="mask">Used in the pixel hit test to determine what counts as solid.</param>
        /// <param name="camera">Specify which game camera you want.  If null getScreenXY() will just grab the first global camera.</param>
        /// <returns></returns>
        public bool pixelOverlapPoint(FlxPoint point, uint mask = 0xFF, FlxCamera camera = null)
        {
            throw new NotImplementedException();

            /*
			if(Camera == null)
				Camera = FlxG.camera;
			getScreenXY(_point,Camera);
			_point.x = _point.x - offset.x;
			_point.y = _point.y - offset.y;
			_flashPoint.x = (Point.x - Camera.scroll.x) - _point.x;
			_flashPoint.y = (Point.y - Camera.scroll.y) - _point.y;
			return framePixels.hitTest(_flashPointZero,Mask,_flashPoint);
            */
        }

        /// <summary>
        /// Internal function to update the current animation frame.
        /// </summary>
        protected virtual void calcFrame()
        {
            int posX = _curIndex * (int)FrameWidth;
            int posY = 0;

            if (posX >= _pixels.Width)
            {
                posY = (posX / _pixels.Width) * (int)FrameHeight;
                posX = posX % _pixels.Width;
            }

            /*
            //Handle sprite sheets
            uint widthHelper = Convert.ToBoolean(_flipped) ? _flipped : (uint)texture.Width;
            if (indexX >= widthHelper)
            {
                indexY = (indexX / (int)widthHelper) * FrameHeight;
                indexX %= (int)widthHelper;
            }
            */

            drawSourceRect.X = (int)posX;
            drawSourceRect.Y = (int)posY;

            /*
            // handle reversed sprites
            if ( Convert.ToBoolean(_flipped) && Convert.ToBoolean(_facing == LEFT))
                indexX = (_flipped<<1)-indexX-frameWidth;

            // Update display bitmap
            _flashRect.x = indexX;
            _flashRect.y = indexY;
            framePixels.copyPixels(_pixels,_flashRect,_flashPointZero);
            _flashRect.x = _flashRect.y = 0;
            if(_colorTransform != null)
                framePixels.colorTransform(_flashRect,_colorTransform);
            */

            if (_callback != null)
            {
                _callback(((_curAnim != null) ? (_curAnim.Name) : null), (uint)_curFrame, (uint)_curIndex);                
            }

            Dirty = false;
        }

        /// <summary>
        /// flx# stuff
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /*
        public override void reset(float X, float Y)
        {
            base.reset(X, Y);
            Alpha = 1;
        }
        */

        /// <summary>
        /// flx# only
        /// </summary>
        /*
        public override void update()
        {
            base.update();
            if (Scale.x < 1)
                Scale.x = 1;
            if (Scale.y < 1)
                Scale.y = 1;
            if (flickering)
            {
                Alpha = FlxG.random();
            }

            if ((Velocity.x != 0) || (Velocity.y != 0))
                moving = true;
            else
                moving = false;
        }
        */

        public Vector2 getVec2()
        {
            return new Vector2(X - Offset.X, Y - Offset.Y);
        }
    }
}
