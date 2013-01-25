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
using flxSharp.flxSharp;

namespace fliXNA_xbox
{
    public class FlxCamera : FlxBasic
    {
        public const uint STYLE_LOCKON = 0;
        public const uint STYLE_PLATFORMER = 1;
        public const uint STYLE_TOPDOWN = 2;
        public const uint STYLE_TOPDOWN_TIGHT = 3;
        public const uint STYLE_LOOSE = 4;
        public const uint SHAKE_BOTH_AXES = 0;
        public const uint SHAKE_HORIZONTAL_ONLY = 1;
        public const uint SHAKE_VERTICAL_ONLY = 2;

        static public Effect effect;

        static public float defaultZoom = 1.0f;
        public float x;
        public float y;
        public float width;
        public float height;
        public FlxObject target;
        public FlxRect deadzone;
        public FlxRect bounds;

        protected float _zoom;
        protected FlxPoint _point;

        public Matrix transform;
        public FlxPoint scroll;
        protected float angle;

        protected float _fxShakeIntensity;
        protected float _fxShakeDuration;
        protected Action _fxShakeComplete;
        protected FlxPoint _fxShakeOffset;
        protected uint _fxShakeDirection;

        public FlxCamera(float X, float Y, float Width, float Height, float Zoom = 1.0f)
            : base()
        {
            x = X;
            y = Y;
            width = Width;
            height = Height;
            defaultZoom = 1.0f;
            rotating = 0.0f;
            scroll = new FlxPoint();
            _point = new FlxPoint();
            target = null;
            deadzone = null;
            bounds = null;
            zooming = Zoom;

            _fxShakeIntensity = 0;
            _fxShakeDuration = 0;
            _fxShakeComplete = null;
            _fxShakeOffset = new FlxPoint();
            _fxShakeDirection = 0;
        }

        // update camera scroll in here
        // make sure it stays within bounds
        public override void update()
        {
            //zooming = FlxG.zoom;
            //rotating = FlxG.rotation;
            //follow closely or check deadzones
            if (target != null)
            {
                if (deadzone == null)
                {
                    focusOn(target.getMidpoint());   //add getMidpoint() for FlxObjects
                }
                else
                {
                    //FlxG.log("deadzone is not null");
                    float edge;
                    float targetX = FlxU.ceil(target.X + ((target.X > 0) ? 0.0000001f : -0.0000001f));
                    float targetY = FlxU.ceil(target.Y + ((target.Y > 0) ? 0.0000001f : -0.0000001f));

                    edge = targetX - deadzone.x;
                    if (scroll.x > edge)
                        scroll.x = edge;
                    edge = targetX + target.Width - deadzone.x - deadzone.width;
                    if (scroll.x < edge)
                        scroll.x = edge;

                    edge = targetY - deadzone.y;
                    if (scroll.y > edge)
                        scroll.y = edge;
                    edge = targetY + target.Height - deadzone.y - deadzone.height;
                    if (scroll.y < edge)
                        scroll.y = edge;

                    //SHAKE
                }
            }

            //make sure we didnt go outside camera's bounds
            if (bounds != null)
            {
                //FlxG.log("bounds is not null");
                if (scroll.x < bounds.left)
                    scroll.x = bounds.left;
                if (scroll.x > bounds.right - width)
                    scroll.x = bounds.right - width;
                if (scroll.y < bounds.top)
                    scroll.y = bounds.top;
                if (scroll.y > bounds.bottom - height)
                    scroll.y = bounds.bottom - height;
            }

            //update effects

            //shake
            if (_fxShakeDuration > 0)
            {
                _fxShakeDuration -= FlxG.elapsed;
                if (_fxShakeDuration <= 0)
                {
                    _fxShakeOffset.make();
                    if (_fxShakeComplete != null)
                        _fxShakeComplete();
                }
                else
                {
                    if ((_fxShakeDirection == SHAKE_BOTH_AXES) || (_fxShakeDirection == SHAKE_HORIZONTAL_ONLY))
                        _fxShakeOffset.x = (FlxG.random() * _fxShakeIntensity * width * 2 - _fxShakeIntensity * width) * _zoom;
                    if ((_fxShakeDirection == SHAKE_BOTH_AXES) || (_fxShakeDirection == SHAKE_VERTICAL_ONLY))
                        _fxShakeOffset.y = (FlxG.random() * _fxShakeIntensity * height * 2 - _fxShakeIntensity * height) * _zoom;
                }
                
            }

            
            scroll.x -= _fxShakeOffset.x;
            scroll.y -= _fxShakeOffset.y;

            if (zooming < 1)
                zooming = 1;
        }

        /// <summary>
        /// Shake the screen
        /// </summary>
        /// <param name="Intensity"></param>
        /// <param name="Duration"></param>
        /// <param name="OnComplete"></param>
        /// <param name="Force"></param>
        /// <param name="Direction"></param>
        public void shake(float Intensity = 0.05f, float Duration = 0.5f, Action OnComplete=null, bool Force=true, uint Direction=SHAKE_BOTH_AXES)
        {
            if (!Force && ((_fxShakeOffset.x != 0) || (_fxShakeOffset.y != 0)))
                return;
            _fxShakeIntensity = Intensity;
            _fxShakeDuration = Duration;
            _fxShakeComplete = OnComplete;
            _fxShakeDirection = Direction;
            _fxShakeOffset.make();
        }

        /// <summary>
        /// Internal function for stopping any effects, can be called manually
        /// </summary>
        public void stopFX()
        {
            _fxShakeDuration = 0;
        }

        /// <summary>
        /// Make the camera follow an object with a specified follow-style
        /// </summary>
        /// <param name="Target"></param>
        /// <param name="Style"></param>
        public void follow(FlxObject Target, uint Style=STYLE_LOCKON)
        {
            target = Target;
            float helper, w, h;
            switch (Style)
            {
                case STYLE_PLATFORMER:
                    w = width / 8;
                    h = height / 3;
                    deadzone = new FlxRect((width - w) / 2, (height - h) / 2 - h * 0.25f, w, h);
                    break;
                case STYLE_TOPDOWN:
                    helper = FlxU.max(width, height) / 3;
                    deadzone = new FlxRect((width - helper) / 3, (height - helper) / 3, helper, helper);
                    break;
                case STYLE_TOPDOWN_TIGHT:
                    helper = FlxU.max(width, height) / 12;
                    deadzone = new FlxRect((width - helper) / 2, (height - helper) / 2, helper, helper);
                    break;
                case STYLE_LOCKON:
                    break;
                case STYLE_LOOSE:
                    deadzone = new FlxRect(0, 0, width, height);
                    break;
                default:
                    deadzone = null;
                    break;
            }

        }

        /// <summary>
        /// Shift the camera's focus onto a specific point in the game-world.
        /// </summary>
        /// <param name="Point"></param>
        public void focusOn(FlxPoint Point)
        {
            Point.x += (Point.x > 0) ? (int)0.0000001 : (int)-0.0000001;
            Point.y += (Point.y > 0) ? (int)0.0000001 : (int)-0.0000001;
            scroll = new FlxPoint(Point.x - width * (int)0.5, Point.y - height * (int)0.5);
        }

        /// <summary>
        /// Set the boundaries of the camera.  Useful for preventing the camera from scrolling past the edge of a map.
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <param name="UpdateWorld"></param>
        public void setBounds(float X = 0, float Y = 0, float Width = 0, float Height = 0, Boolean UpdateWorld = false)
        {
            if (bounds == null)
                bounds = new FlxRect();
            bounds.make(X, Y, Width, Height);
            if (UpdateWorld)
                FlxG.worldBounds.copyFrom(bounds);
            update();
        }

        /// <summary>
        /// Get/Set for the zoom of the camera
        /// </summary>
        public float zooming
        {
            get { return _zoom; }
            set { _zoom = value; if (_zoom < 0.1f) _zoom = defaultZoom; } // Negative zoom will flip image
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
            get { return scroll; }
            set { scroll = value; }
        }

        /// <summary>
        /// Internal function for camera stuff
        /// </summary>
        /// <returns></returns>
        internal Matrix TransformMatrix
        {
            get
            {
                if(target!=null)
                    return Matrix.CreateTranslation(-target.X, -target.Y, 0) * Matrix.CreateRotationZ(rotating) * Matrix.CreateScale(new Vector3(zooming, zooming, 1)) * Matrix.CreateTranslation(-scroll.x, -scroll.y, 0) * Matrix.CreateTranslation(target.X, target.Y, 0);
                else
                    return Matrix.CreateRotationZ(rotating) * Matrix.CreateScale(new Vector3(zooming, zooming, 1)) * Matrix.CreateTranslation(-scroll.x, -scroll.y, 0);
            }
        }
    }
}
