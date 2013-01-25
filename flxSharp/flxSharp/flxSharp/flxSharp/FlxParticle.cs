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

namespace fliXNA_xbox
{
    /**
	 * This is a simple particle class that extends the default behavior
	 * of <code>FlxSprite</code> to have slightly more specialized behavior
	 * common to many game scenarios.  You can override and extend this class
	 * just like you would <code>FlxSprite</code>. While <code>FlxEmitter</code>
	 * used to work with just any old sprite, it now requires a
	 * <code>FlxParticle</code> based class.
	 * 
	 * @author Adam Atomic
	 */
	public class FlxParticle : FlxSprite
	{

        private Texture2D texture;
        public FlxPoint minSpeed;
        public FlxPoint maxSpeed;
        private float rotateBy;
        private FlxRect particleRect;

		/**
		 * How long this particle lives before it disappears.
		 * NOTE: this is a maximum, not a minimum; the object
		 * could get recycled before its lifespan is up.
		 */
		public float lifespan;

		/**
		 * Determines how quickly the particles come to rest on the ground.
		 * Only used if the particle has gravity-like acceleration applied.
		 * @default 500
		 */
		public float friction;

		/**
		 * Instantiate a new particle.  Like <code>FlxSprite</code>, all meaningful creation
		 * happens during <code>loadGraphic()</code> or <code>makeGraphic()</code> or whatever.
		 */
		public FlxParticle() : base()
		{
			lifespan = 0;
			friction = 500;
            rotateBy = 0;
            minSpeed = new FlxPoint();
            maxSpeed = new FlxPoint();
            particleRect = null;
		}

		/**
		 * The particle's main update logic.  Basically it checks to see if it should
		 * be dead yet, and then has some special bounce behavior if there is some gravity on it.
		 */
		override public void update()
		{
			//lifespan behavior
			if(lifespan <= 0)
				return;
			lifespan -= FlxG.elapsed;
			if(lifespan <= 0)
				kill();

			//simpler bounce/spin behavior for now
			if(Convert.ToBoolean(touching))
			{
				if(angularVelocity != 0)
					angularVelocity = -angularVelocity;
			}
			if(acceleration.y > 0) //special behavior for particles with gravity
			{
				if(Convert.ToBoolean(touching) & Convert.ToBoolean(FLOOR))
				{
					drag.x = friction;

					if(!(Convert.ToBoolean(wasTouching) & Convert.ToBoolean(FLOOR)))
					{
						if(velocity.y < -elasticity*10)
						{
							if(angularVelocity != 0)
								angularVelocity *= -elasticity;
						}
						else
						{
							velocity.y = 0;
							angularVelocity = 0;
						}
					}
				}
				else
					drag.x = 0;
			}
            angle += rotateBy;
		}

		/**
		 * Triggered whenever this object is launched by a <code>FlxEmitter</code>.
		 * You can override this to add custom behavior like a sound or AI or something.
		 */
		public void onEmit()
		{
		}

        public override void reset(float X, float Y)
        {
            base.reset(X, Y);
            Random r = new Random();
            velocity.x = r.Next((int)minSpeed.x, (int)maxSpeed.x);
            velocity.y = r.Next((int)minSpeed.y, (int)maxSpeed.y);
        }

        /// <summary>
        /// Internal function used by FlxEmitter to create the particles
        /// </summary>
        /// <param name="Graphic"></param>
        /// <param name="Rotation"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <returns></returns>
        public FlxSprite loadParticleGraphic(Texture2D Graphic, float Rotation, float Width = 0, float Height = 0)
        {
            rotateBy = Rotation * 0.0174532925f;
            texture = Graphic;
            int numFrames = (int)FlxU.floor(texture.Width / texture.Height);
            
            if (Width == 0 || Height == 0)
            {
                Width = texture.Height;
                Height = Width;
            }
            frameWidth = width = Width;
            frameHeight = height = Height;
            List<FlxRect> _draws = new List<FlxRect>();
            for (int i = 0; i < numFrames; i++)
            {
                FlxRect textureArea = new FlxRect(i * height, 0, width, height);
                _draws.Add(textureArea);
            }
            int di;// = (int)Math.Round(FlxU.randomBetween(0, numFrames));
            Random r = new Random();
            di = r.Next(numFrames);
            particleRect = _draws[di];
            //FlxRect t = _draws[di];
            sourceRect = new FlxRect(particleRect.x, particleRect.y, particleRect.width, particleRect.height);
            return this;
        }

        public override void draw()
        {
            if (visible)
            {
                if (texture != null)
                {
                    Rectangle rect = new Rectangle((int)sourceRect.x, (int)sourceRect.y, (int)sourceRect.width, (int)sourceRect.height);
                    FlxG.spriteBatch.Draw(texture, getVec2(), rect, _color * alpha, angle, new Vector2(width/2, height/2), scale.getVec2(),  SpriteEffects.None,  0f);
                }
            }
        }
	}
}
