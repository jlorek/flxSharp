using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using fliXNA_xbox;

namespace flxSharp.flxSharp
{
    public class FlxSprite : FlxObject
    {
        protected Texture2D ImgDefault = FlxG.content.Load<Texture2D>("default");

        public FlxPoint origin;
        public FlxPoint offset;
        public FlxPoint scale;
        public Boolean finished;
        public float frameWidth;
        public float frameHeight;
        public uint frames;
        public Boolean dirty;
        protected List<FlxAnim> _animations;
        protected uint _flipped;
        protected FlxAnim _curAnim;
        protected int _curFrame;
        protected int _curIndex;
        protected float _frameTimer;
        Action<String, uint, uint> _callback;
        protected uint _facing;
        public float alpha;
        protected Color _color;
        protected float _bakedRotation;
        protected Matrix _matrix;
        protected Boolean _animated;
        protected FlxRect sourceRect;
        private Texture2D texture;

        //scrollfactor experimentation
        protected float camX;
        protected float camY;
        protected float oX;
        protected float oY;

        public bool moving;


        public FlxSprite(float X = 0, float Y = 0, Texture2D Graphic = null)
            : base(X, Y)
        {
            scale = new FlxPoint(1.0f, 1.0f);
            offset = new FlxPoint();
            origin = new FlxPoint();
            alpha = 1.0f;
            _color = Color.White * alpha;

            _animations = new List<FlxAnim>();
            _animated = false;

            finished = false;
            _facing = Right;
            _flipped = 0;
            _curAnim = null;
            _curFrame = 0;
            _curIndex = 0;
            _frameTimer = 0;

            _callback = null;
            _matrix = new Matrix();

            if (Graphic == null)
                Graphic = ImgDefault;
            loadGraphic(Graphic);

            Angle = 0f;

            camX = camY = 0;
            oX = X;
            oY = Y;

            moving = false;
        }

        public override void destroy()
        {
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
			
			offset = null;
			origin = null;
			scale = null;
			_curAnim = null;
			_callback = null;
        }

        public Color color
        {
            get { return _color; }
            set { _color = value; }
        }

        public FlxSprite loadGraphic(Texture2D Graphic, Boolean Animated = false, Boolean Reverse = false, float Width = 0, float Height = 0)
        {
            _animated = Animated;
            texture = Graphic;
            if (Width == 0)
            {
                if (Animated)
                    Width = (uint)Graphic.Width;
                else
                    Width = (uint)Graphic.Width;
            }
            frameWidth = base.Width = Width;
            if (Height == 0)
            {
                if (Animated)
                    Height = (uint)base.Width;
                else
                    Height = (uint)Graphic.Height;
            }
            frameHeight = base.Height = Height;
            resetHelpers();
            sourceRect = new FlxRect(0, 0, base.Width, base.Height);
            return this;
        }

        public FlxSprite makeGraphic(float Width, float Height, Color color)
        {
            texture = new Texture2D(FlxG.graphicsDevice, (int)Width, (int)Height);
            Color[] colors = new Color[(int)Width * (int)Height];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = new Color(color.ToVector3());
            }
            texture.SetData(colors);
            color = color * alpha;
            sourceRect = new FlxRect(0, 0, Width, Height);
            resetHelpers();
            frameWidth = base.Width = Width;
            frameHeight = base.Height = Height;
            return this;
        }

        protected void resetHelpers()
        {
            origin.make(frameWidth * 0.5f, frameHeight * 0.5f);
            _curIndex = 0;
        }

        public override void postUpdate()
        {
            base.postUpdate();
            updateAnimation();
        }

        public override void update()
        {
            base.update();
            if (scale.x < 1)
                scale.x = 1;
            if (scale.y < 1)
                scale.y = 1;
            if (flickering)
            {
                alpha = FlxG.random();
            }

            if ((Velocity.x != 0) || (Velocity.y != 0))
                moving = true;
            else
                moving = false;
        }

        public override void reset(float X, float Y)
        {
            base.reset(X, Y);
            alpha = 1;
        }

        public Vector2 getVec2()
        {
            return new Vector2(X - offset.x, Y - offset.y);
        }

        public override void draw()
        {
            if (dirty)
                calcFrame();

            if (Cameras == null)
                Cameras = FlxG.cameras;
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
                _tagPoint.x = X - (int)(camera.scroll.x * ScrollFactor.x) - offset.x;
                _tagPoint.y = Y - (int)(camera.scroll.y * ScrollFactor.y) - offset.y;
                _tagPoint.x += (_tagPoint.x > 0) ? 0.0000001f : -0.0000001f;
                _tagPoint.y += (_tagPoint.y > 0) ? 0.0000001f : -0.0000001f;
                if (Visible)
                {
                    base.draw();
                    if (texture != null)
                    {
                        //if the sprite is animated then the sourceRect needs to be changed to be the correct frame
                        if (_animated)
                            sourceRect = new FlxRect(frameWidth * _curIndex, 0, frameWidth, frameHeight);
                        Rectangle rect = new Rectangle((int)sourceRect.x, (int)sourceRect.y, (int)sourceRect.width, (int)sourceRect.height);
                        FlxG.spriteBatch.Draw(texture, getVec2(), rect, _color * alpha, Angle, new Vector2(), scale.getVec2(), SpriteEffects.None, 0f);
                    }
                }
            }
        }

        protected void updateAnimation()
		{
            if((_curAnim != null) && (_curAnim.delay > 0) && (_curAnim.looped || !finished))
			{
				_frameTimer += FlxG.elapsed;
				while(_frameTimer > _curAnim.delay)
				{
					_frameTimer = _frameTimer - _curAnim.delay;
					if(_curFrame == _curAnim.frames.Length-1)
					{
						if(_curAnim.looped)
							_curFrame = 0;
						finished = true;
					}
					else
						_curFrame++;
                    _curIndex = _curAnim.frames[_curFrame];
					dirty = true;
				}
			}

			if(dirty)
				calcFrame();
		}

        public void addAnimation(String Name, int[] Frames, float FrameRate = 1, Boolean Looped = true)
        {
            _animations.Add(new FlxAnim(Name, Frames, FrameRate, Looped));
        }

        public void addAnimationCallback(Action<String, uint, uint> AnimationCallback)
		{
			_callback = AnimationCallback;
		}

        public void play(String AnimName, Boolean Force=false)
		{
            if (!Force && (_curAnim != null) && (AnimName == _curAnim.name) && (_curAnim.looped || !finished)) return;
			_curFrame = 0;
			_curIndex = 0;
			_frameTimer = 0;
			uint i = 0;
			uint l = (uint)_animations.Count;
			while(i < l)
			{
				if(_animations[(int)i].name == AnimName)
				{
					_curAnim = _animations[(int)i];
					if(_curAnim.delay <= 0)
						finished = true;
					else
						finished = false;
                    _curIndex = _curAnim.frames[_curFrame];
					dirty = true;
					return;
				}
				i++;
			}
			FlxG.log("WARNING: No animation called \""+AnimName+"\"");
		}


        protected virtual void calcFrame()
		{
			float indexX = _curIndex*frameWidth;
            float indexY = 0;

			//Handle sprite sheets
			uint widthHelper = Convert.ToBoolean(_flipped)?_flipped:(uint)texture.Width;
			if(indexX >= widthHelper)
			{
				indexY = (indexX/(int)widthHelper)*frameHeight;
				indexX %= (int)widthHelper;
			}

			//handle reversed sprites
            //if ( Convert.ToBoolean(_flipped) && Convert.ToBoolean(_facing == LEFT))
            //    indexX = (_flipped<<1)-indexX-frameWidth;

			//Update display bitmap
            //_flashRect.x = indexX;
            //_flashRect.y = indexY;
            //framePixels.copyPixels(_pixels,_flashRect,_flashPointZero);
            //_flashRect.x = _flashRect.y = 0;
            //if(_colorTransform != null)
            //    framePixels.colorTransform(_flashRect,_colorTransform);
            if (_callback != null)
                _callback(((_curAnim != null) ? (_curAnim.name) : null), (uint)_curFrame, (uint)_curIndex);
			dirty = false;
		}

        public uint facing
        {
            get { return _facing; }
            set { 
                if(_facing != value)
                    dirty = true;
                _facing = value;
            }
        }

        override public bool onScreen(FlxCamera camera = null)
        {
            if (camera == null)
                camera = FlxG.camera;
            getScreenXY(_tagPoint, camera);
            _tagPoint.x = _tagPoint.x - offset.x;
            _tagPoint.y = _tagPoint.y - offset.y;

            float halfWidth = frameWidth / 2;
            float halfHeight = frameHeight / 2;
            float absScaleX = (scale.x > 0) ? scale.x : -scale.x;
            float absScaleY = (scale.y > 0) ? scale.y : -scale.y;
            float radius = (float)Math.Sqrt(halfWidth * halfWidth + halfHeight * halfHeight) * ((absScaleX >= absScaleY) ? absScaleX : absScaleY);
            _tagPoint.x += halfWidth;
            _tagPoint.y += halfHeight;
            return ((_tagPoint.x + radius > 0) && (_tagPoint.x - radius < camera.width) && (_tagPoint.y + radius > 0) && (_tagPoint.y - radius < camera.height));
        }

        public int frame
        {
            get { return _curIndex; }
            set
            {
                _curAnim = null;
                _curIndex = value;
                dirty = true;
            }
        }

        public FlxPoint scaling
        {
            get { return scale; }
            set
            {
                scale.x += value.x;
                scale.y += value.y;
                Width *= scale.x;
                Height *= scale.y;
                centerOffsets();
            }
        }

        public void centerOffsets(bool AdjustPosition = false)
        {
            offset.x = (frameWidth - Width) * 0.5f;
            offset.y = (frameHeight - Height) * 0.5f;
            if (AdjustPosition)
            {
                X += offset.x;
                Y += offset.y;
            }
        }

        /// <summary>
        /// Get/Set for the rotation of the camera
        /// </summary>
        public float rotation
        {
            get { return Angle; }
            set { Angle += value; }
        }
        
    }
}
