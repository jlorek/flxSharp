using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace flxSharp.flxSharp
{
    /// <summary>
    /// The camera class is used to display the game's visuals in the Flash player.
    /// By default one camera is created automatically, that is the same size as the Flash player.
    /// You can add more cameras or even replace the main camera using utilities in <code>FlxG</code>.
    /// </summary>
    public class FlxCamera : FlxBasic
    {
        /// <summary>
        /// Camera "follow" style preset: camera has no deadzone, just tracks the focus object directly.
        /// </summary>
        public const uint StyleLockon = 0;

        /// <summary>
        /// Camera "follow" style preset: camera deadzone is narrow but tall.
        /// </summary>
        public const uint StylePlatformer = 1;

        /// <summary>
        /// Camera "follow" style preset: camera deadzone is a medium-size square around the focus object.
        /// </summary>
        public const uint StyleTopdown = 2;

        /// <summary>
        /// Camera "follow" style preset: camera deadzone is a small square around the focus object.
        /// </summary>
        public const uint StyleTopdownTight = 3;

        /// <summary>
        /// fliXNA style
        /// </summary>
        public const uint StyleLoose = 4;

        /// <summary>
        /// Camera "shake" effect preset: shake camera on both the X and Y axes.
        /// </summary>
        public const uint ShakeBothAxes = 0;
        
        /// <summary>
        /// Camera "shake" effect preset: shake camera on the X axis only.
        /// </summary>
        public const uint ShakeHorizontalOnly = 1;

        /// <summary>
        /// Camera "shake" effect preset: shake camera on the Y axis only.
        /// </summary>
        public const uint ShakeVerticalOnly = 2;

        /// <summary>
        /// While you can alter the zoom of each camera after the fact,
        /// this variable determines what value the camera will start at when created.
        /// </summary>
        static public float DefaultZoom = 1.0f;

        /// <summary>
        /// The X position of this camera's display.  Zoom does NOT affect this number.
        /// Measured in pixels from the left side of the flash window.
        /// </summary>
        public float X;

        /// <summary>
        /// The Y position of this camera's display.  Zoom does NOT affect this number.
        /// Measured in pixels from the top of the flash window.
        /// </summary>
        public float Y;

        /// <summary>
        /// How wide the camera display is, in game pixels.
        /// </summary>
        public float Width;

        /// <summary>
        /// How tall the camera display is, in game pixels.
        /// </summary>
        public float Height;

        /// <summary>
        /// Tells the camera to follow this <code>FlxObject</code> object around.
        /// </summary>
        public FlxObject Target;

        /// <summary>
        /// You can assign a "dead zone" to the camera in order to better control its movement.
        /// The camera will always keep the focus object inside the dead zone,
        /// unless it is bumping up against the bounds rectangle's edges.
        /// The deadzone's coordinates are measured from the camera's upper left corner in game pixels.
        /// For rapid prototyping, you can use the preset deadzones (e.g. <code>STYLE_PLATFORMER</code>) with <code>follow()</code>.
        /// </summary>
        public FlxRect Deadzone;

        /// <summary>
        /// The edges of the camera's range, i.e. where to stop scrolling.
        /// Measured in game pixels and world coordinates.
        /// </summary>
        public FlxRect Bounds;

        /// <summary>
        /// Stores the basic parallax scrolling values.
        /// </summary>
        public FlxPoint Scroll;

        /// <summary>
        /// The actual bitmap data of the camera display itself.
        /// </summary>
        public RenderTarget2D Buffer;

        /// <summary>
        /// The natural background color of the camera. Defaults to FlxG.bgColor.
        /// NOTE: can be transparent for crazy FX!
        /// </summary>
        public Color BgColor;

        /// <summary>
        /// Sometimes it's easier to just work with a <code>FlxSprite</code> than it is to work
        /// directly with the <code>BitmapData</code> buffer.  This sprite reference will
        /// allow you to do exactly that.
        /// </summary>
        public FlxSprite screen;

        /// <summary>
        /// Indicates how far the camera is zoomed in.
        /// </summary>
        protected float _zoom;

        /// <summary>
        /// Internal, to help avoid costly allocations.
        /// </summary>
        protected FlxPoint _point;

        /// <summary>
        /// Internal, help with color transforming the flash bitmap.
        /// </summary>
        protected uint _color;

        /**
		 * Internal, used to render buffer to screen space.
		 */
		//protected var _flashBitmap:Bitmap;
		
        /**
		 * Internal, used to render buffer to screen space.
		 */
		//internal var _flashSprite:Sprite;
		
        /**
		 * Internal, used to render buffer to screen space.
		 */
		//internal var _flashOffsetX:Number;
		
        /**
		 * Internal, used to render buffer to screen space.
		 */
		//internal var _flashOffsetY:Number;
		
        /**
		 * Internal, used to render buffer to screen space.
		 */
		//protected var _flashRect:Rectangle;
		
        /**
		 * Internal, used to render buffer to screen space.
		 */
		//protected var _flashPoint:Point;

        ///<summary>
        /// Internal, used to control the "flash" special effect.
        ///</summary>
        protected uint fxFlashColor;
        
        ///<summary>
        /// Internal, used to control the "flash" special effect.
        ///</summary>
        protected float fxFlashDuration;

        ///<summary>
        /// Internal, used to control the "flash" special effect.
        ///</summary>
        protected Action fxFlashComplete;

        ///<summary>
        /// Internal, used to control the "flash" special effect.
        ///</summary>
        protected float fxFlashAlpha;

        ///<summary>
        /// Internal, used to control the "fade" special effect.
        ///</summary>
        protected uint fxFadeColor;
        
        ///<summary>
        /// Internal, used to control the "fade" special effect.
        ///</summary>
        protected float fxFadeDuration;
        
        ///<summary>
        /// Internal, used to control the "fade" special effect.
        ///</summary>
        protected Action fxFadeComplete;

        ///<summary>
        /// Internal, used to control the "fade" special effect.
        ///</summary>
        protected float fxFadeAlpha;
        
        ///<summary>
        /// Internal, used to control the "shake" special effect.
        ///</summary>
        protected float fxShakeIntensity;
        
        ///<summary>
        /// Internal, used to control the "shake" special effect.
        ///</summary>
        protected float fxShakeDuration;
        
        ///<summary>
        /// Internal, used to control the "shake" special effect.
        ///</summary>
        protected Action fxShakeComplete;
        
        ///<summary>
        /// Internal, used to control the "shake" special effect.
        ///</summary>
        protected FlxPoint fxShakeOffset;
        
        ///<summary>
        /// Internal, used to control the "shake" special effect.
        ///</summary>
        protected uint fxShakeDirection;

        /**
		 * Internal helper variable for doing better wipes/fills between renders.
		 */
		//protected var _fill:BitmapData;

        // flx# stuff
        public Matrix transform;
        protected float angle;
        public FlxObject FlashSprite;
        public float FlashOffsetX;
        public float FlashOffsetY;

        /// <summary>
        /// Instantiates a new camera at the specified location, with the specified size and zoom level.
        /// </summary>
        /// <param name="x">X location of the camera's display in pixels. Uses native, 1:1 resolution, ignores zoom.</param>
        /// <param name="y">Y location of the camera's display in pixels. Uses native, 1:1 resolution, ignores zoom.</param>
        /// <param name="width">The width of the camera display in pixels.</param>
        /// <param name="height">The height of the camera display in pixels.</param>
        /// <param name="zoom">The initial zoom level of the camera. A zoom level of 2 will make all pixels display at 2x resolution.</param>
        public FlxCamera(float x, float y, int width, int height, float zoom = 0)
            : base()
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Target = null;
            Deadzone = null;
            Scroll = new FlxPoint();
            _point = new FlxPoint();
            Bounds = null;
            screen = new FlxSprite();
            screen.makeGraphic((uint) width, (uint) height, new Color(0, 0, 0, 0));
            screen.setOriginToCorner();
            //Buffer = screen.Pixels;
            BgColor = FlxG.bgColor;
            _color = 0xffffffff;

            /*
			_flashBitmap = new Bitmap(buffer);
			_flashBitmap.x = -width*0.5;
			_flashBitmap.y = -height*0.5;
			_flashSprite = new Sprite();
            */

            // flx# - ?
			// zoom = Zoom; //sets the scale of flash sprite, which in turn loads flashoffset values
			
            /*
            _flashOffsetX = width*0.5*zoom;
			_flashOffsetY = height*0.5*zoom;
			_flashSprite.x = x + _flashOffsetX;
			_flashSprite.y = y + _flashOffsetY;
			_flashSprite.addChild(_flashBitmap);
			_flashRect = new Rectangle(0,0,width,height);
			_flashPoint = new Point();
            */

            fxFlashColor = 0;
            fxFlashDuration = 0.0f;
            fxFlashComplete = null;
            fxFlashAlpha = 0.0f;

            fxFadeColor = 0;
            fxFadeDuration = 0.0f;
            fxFadeComplete = null;
            fxFadeAlpha = 0.0f;

            fxShakeIntensity = 0.0f;
            fxShakeDuration = 0.0f;
            fxShakeComplete = null;
            fxShakeOffset = new FlxPoint();
            fxShakeDirection = 0;

            //_fill = new BitmapData(width,height,true,0);
            
            // flx#
            DefaultZoom = 1.0f;
            rotating = 0.0f;
            zooming = zoom;
            FlashSprite = new FlxObject();
            //BgColor = Color.Black;
        }

        /// <summary>
        /// Clean up memory.
        /// </summary>
        public override void destroy()
        {
            screen.destroy();
            screen = null;
            Target = null;
            Scroll = null;
            Deadzone = null;
            Bounds = null;
            Buffer = null;
            //_flashBitmap = null;
            //_flashRect = null;
            //_flashPoint = null;
            fxFlashComplete = null;
            fxFadeComplete = null;
            fxShakeComplete = null;
            fxShakeOffset = null;
            //_fill = null;

            base.destroy();
        }

        /// <summary>
        /// Updates the camera scroll as well as special effects like screen-shake or fades.
        /// </summary>
        public override void update()
        {
            //Either follow the object closely, 
            //or doublecheck our deadzone and update accordingly.
            if (Target != null)
            {
                if (Deadzone == null)
                {
                    focusOn(Target.getMidpoint(_point));
                }
                else
                {
                    float edge;
                    //float targetX = FlxU.ceil(Target.X + ((Target.X > 0) ? 0.0000001f : -0.0000001f));
                    //float targetY = FlxU.ceil(Target.Y + ((Target.Y > 0) ? 0.0000001f : -0.0000001f));
                    float targetX = Target.X + ((Target.X > 0) ? 0.0000001f : -0.0000001f);
                    float targetY = Target.Y + ((Target.Y > 0) ? 0.0000001f : -0.0000001f);

                    edge = targetX - Deadzone.X;
                    if (Scroll.X > edge)
                    {
                        Scroll.X = edge;                        
                    }

                    edge = targetX + Target.Width - Deadzone.X - Deadzone.Width;
                    if (Scroll.X < edge)
                    {
                        Scroll.X = edge;                        
                    }

                    edge = targetY - Deadzone.Y;
                    if (Scroll.Y > edge)
                    {
                        Scroll.Y = edge;                        
                    }

                    edge = targetY + Target.Height - Deadzone.Y - Deadzone.Height;
                    if (Scroll.Y < edge)
                    {
                        Scroll.Y = edge;                        
                    }
                }
            }

            // Make sure we didn't go outside the camera's bounds
            if (Bounds != null)
            {
                //FlxG.log("bounds is not null");
                if (Scroll.X < Bounds.Left)
                {
                    Scroll.X = Bounds.Left;                    
                }

                if (Scroll.X > Bounds.Right - Width)
                {
                    Scroll.X = Bounds.Right - Width;                    
                }

                if (Scroll.Y < Bounds.Top)
                {
                    Scroll.Y = Bounds.Top;                    
                }

                if (Scroll.Y > Bounds.Bottom - Height)
                {
                    Scroll.Y = Bounds.Bottom - Height;                    
                }
            }

            // Update the "flash" special effect
            if (fxFlashAlpha > 0.0)
            {
                fxFlashAlpha -= FlxG.elapsed / fxFlashDuration;
                if ((fxFlashAlpha <= 0) && (fxFlashComplete != null))
                {
                    fxFlashComplete();                    
                }
            }

            //Update the "fade" special effect
            if ((fxFadeAlpha > 0.0) && (fxFadeAlpha < 1.0))
            {
                fxFadeAlpha += FlxG.elapsed / fxFadeDuration;
                if (fxFadeAlpha >= 1.0)
                {
                    fxFadeAlpha = 1.0f;
                    if (fxFadeComplete != null)
                    {
                        fxFadeComplete();                        
                    }
                }
            }

            //Update the "shake" special effect
            if (fxShakeDuration > 0)
            {
                fxShakeDuration -= FlxG.elapsed;
                if (fxShakeDuration <= 0)
                {
                    fxShakeOffset.make();
                    if (fxShakeComplete != null)
                    {
                        fxShakeComplete();                        
                    }
                }
                else
                {
                    if ((fxShakeDirection == ShakeBothAxes) || (fxShakeDirection == ShakeHorizontalOnly))
                    {
                        fxShakeOffset.X = (FlxG.random() * fxShakeIntensity * Width * 2 - fxShakeIntensity * Width) * _zoom;                        
                    }

                    if ((fxShakeDirection == ShakeBothAxes) || (fxShakeDirection == ShakeVerticalOnly))
                    {
                        fxShakeOffset.Y = (FlxG.random() * fxShakeIntensity * Height * 2 - fxShakeIntensity * Height) * _zoom;                        
                    }
                }
            }

            // flx#
            //Scroll.X -= fxShakeOffset.X;
            //Scroll.Y -= fxShakeOffset.Y;

            //if (zooming < 1)
            //    zooming = 1;
        }

        /// <summary>
        /// Tells this camera object what <code>FlxObject</code> to track.
        /// </summary>
        /// <param name="target">The object you want the camera to track. Set to null to not follow anything.</param>
        /// <param name="style">Leverage one of the existing "deadzone" presets.  If you use a custom deadzone, ignore this parameter and manually specify the deadzone after calling <code>follow()</code>.</param>
        public void follow(FlxObject target, uint style = StyleLockon)
        {
            Target = target;
            float helper;
            
            switch (style)
            {
                case StylePlatformer:
                    float w = Width / 8;
                    float h = Height / 3;
                    Deadzone = new FlxRect((Width - w) / 2, (Height - h) / 2 - h * 0.25f, w, h);
                    break;

                case StyleTopdown:
                    helper = FlxU.max(Width, Height) / 4;
                    Deadzone = new FlxRect((Width - helper) / 2, (Height - helper) / 2, helper, helper);
                    break;

                case StyleTopdownTight:
                    helper = FlxU.max(Width, Height) / 8;
                    Deadzone = new FlxRect((Width - helper) / 2, (Height - helper) / 2, helper, helper);
                    break;

                case StyleLockon:
                    break;

                case StyleLoose:
                    Deadzone = new FlxRect(0, 0, Width, Height);
                    break;

                default:
                    Deadzone = null;
                    break;
            }

        }

        /// <summary>
        /// Move the camera focus to this location instantly.
        /// </summary>
        /// <param name="point">Where you want the camera to focus.</param>
        public void focusOn(FlxPoint point)
        {
            point.X += (point.X > 0) ? 0.0000001f : -0.0000001f;
            point.Y += (point.Y > 0) ? 0.0000001f : -0.0000001f;
            Scroll.make(point.X - Width * 0.5f, point.Y - Height * 0.5f);
        }

        /// <summary>
        /// Specify the boundaries of the level or where the camera is allowed to move.
        /// </summary>
        /// <param name="x">The smallest X value of your level (usually 0).</param>
        /// <param name="y">The smallest Y value of your level (usually 0).</param>
        /// <param name="width">The largest X value of your level (usually the level width).</param>
        /// <param name="height">The largest Y value of your level (usually the level height).</param>
        /// <param name="updateWorld">Whether the global quad-tree's dimensions should be updated to match (default: false).</param>
        public void setBounds(float x = 0, float y = 0, float width = 0, float height = 0, Boolean updateWorld = false)
        {
            if (Bounds == null)
            {
                Bounds = new FlxRect();                
            }

            Bounds.make(x, y, width, height);

            if (updateWorld)
            {
                FlxG.worldBounds.copyFrom(Bounds);                
            }

            update();
        }

        /// <summary>
        /// Shake the screen
        /// </summary>
        /// <param name="Intensity"></param>
        /// <param name="Duration"></param>
        /// <param name="OnComplete"></param>
        /// <param name="Force"></param>
        /// <param name="Direction"></param>
        public void shake(float Intensity = 0.05f, float Duration = 0.5f, Action OnComplete=null, bool Force=true, uint Direction=ShakeBothAxes)
        {
            if (!Force && ((fxShakeOffset.X != 0) || (fxShakeOffset.Y != 0)))
                return;
            fxShakeIntensity = Intensity;
            fxShakeDuration = Duration;
            fxShakeComplete = OnComplete;
            fxShakeDirection = Direction;
            fxShakeOffset.make();
        }

        /// <summary>
        /// Internal function for stopping any effects, can be called manually
        /// </summary>
        public void stopFX()
        {
            fxShakeDuration = 0;
        }







        /// <summary>
        /// Get/Set for the zoom of the camera
        /// </summary>
        public float zooming
        {
            get { return _zoom; }
            set { _zoom = value; if (_zoom < 0.1f) _zoom = DefaultZoom; } // Negative zoom will flip image
        }

        /// <summary>
        /// Get/Set for the rotation of the camera
        /// </summary>
        public float rotating
        {
            get { return angle; }
            set { angle = value; }
        }

        /// <summary>
        /// Get/Set for the position of the camera
        /// </summary>
        public FlxPoint positioning
        {
            get { return Scroll; }
            set { Scroll = value; }
        }

        /// <summary>
        /// Internal function for camera stuff
        /// </summary>
        /// <returns></returns>
        internal Matrix TransformMatrix
        {
            get
            {
                if(Target!=null)
                    return Matrix.CreateTranslation(-Target.X, -Target.Y, 0) * Matrix.CreateRotationZ(rotating) * Matrix.CreateScale(new Vector3(zooming, zooming, 1)) * Matrix.CreateTranslation(-Scroll.X, -Scroll.Y, 0) * Matrix.CreateTranslation(Target.X, Target.Y, 0);
                else
                    return Matrix.CreateRotationZ(rotating) * Matrix.CreateScale(new Vector3(zooming, zooming, 1)) * Matrix.CreateTranslation(-Scroll.X, -Scroll.Y, 0);
            }
        }

        public void Flash(Color color, float duration, Action onComplete, bool force)
        {
            throw new NotImplementedException();
        }

        public void Fade(Color color, float duration, Action onComplete, bool force)
        {
            throw new NotImplementedException();
        }
    }
}
