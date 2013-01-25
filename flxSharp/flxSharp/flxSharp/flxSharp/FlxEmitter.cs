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
    public class FlxEmitter : FlxGroup
	{
		/**
		 * The X position of the top left corner of the emitter in world space.
		 */
		//public var x:Number;
		/**
		 * The Y position of the top left corner of emitter in world space.
		 */
		//public var y:Number;
		/**
		 * The width of the emitter.  Particles can be randomly generated from anywhere within this box.
		 */
		//public var width:Number;
		/**
		 * The height of the emitter.  Particles can be randomly generated from anywhere within this box.
		 */
		//public var height:Number;
		/**
		 * The minimum possible velocity of a particle.
		 * The default value is (-100,-100).
		 */
		public FlxPoint minParticleSpeed;
		/**
		 * The maximum possible velocity of a particle.
		 * The default value is (100,100).
		 */
		public FlxPoint maxParticleSpeed;
		/**
		 * The X and Y drag component of particles launched from the emitter.
		 */
		public FlxPoint particleDrag;
		/**
		 * The minimum possible angular velocity of a particle.  The default value is -360.
		 * NOTE: rotating particles are more expensive to draw than non-rotating ones!
		 */
		public float minRotation;
		/**
		 * The maximum possible angular velocity of a particle.  The default value is 360.
		 * NOTE: rotating particles are more expensive to draw than non-rotating ones!
		 */
		public float maxRotation;
		/**
		 * Sets the <code>acceleration.y</code> member of each particle to this value on launch.
		 */
		public float gravity;
		/**
		 * Determines whether the emitter is currently emitting particles.
		 * It is totally safe to directly toggle this.
		 */
		public bool on;
		/**
		 * How often a particle is emitted (if emitter is started with Explode == false).
		 */
		public float frequency;
		/**
		 * How long each particle lives once it is emitted.
		 * Set lifespan to 'zero' for particles to live forever.
		 */
		public float lifespan;
		/**
		 * How much each particle should bounce.  1 = full bounce, 0 = no bounce.
		 */
		public float bounce;
		/**
		 * Set your own particle class type here.
		 * Default is <code>FlxParticle</code>.
		 */
		public Object particleClass;
		/**
		 * Internal helper for deciding how many particles to launch.
		 */
		protected uint _quantity;
		/**
		 * Internal helper for the style of particle emission (all at once, or one at a time).
		 */
		protected bool _explode;
		/**
		 * Internal helper for deciding when to launch particles or kill them.
		 */
		protected float _timer;
		/**
		 * Internal counter for figuring out how many particles to launch.
		 */
		protected uint _counter;
		/**
		 * Internal point object, handy for reusing for memory mgmt purposes.
		 */
		//protected FlxPoint _point;

        protected FlxObject target;
		/**
		 * Creates a new <code>FlxEmitter</code> object at a specific position.
		 * Does NOT automatically generate or attach particles!
		 * 
		 * @param	X		The X position of the emitter.
		 * @param	Y		The Y position of the emitter.
		 * @param	Size	Optional, specifies a maximum capacity for this emitter.
		 */
		public FlxEmitter(float X=0, float Y=0, uint Size=0, FlxObject Target=null) : base(Size)
		{
			//super(Size);
			x = X;
			y = Y;
			width = 0;
			height = 0;
			minParticleSpeed = new FlxPoint(-100,-100);
			maxParticleSpeed = new FlxPoint(100,100);
			minRotation = -360;
			maxRotation = 360;
			gravity = 0;
			particleClass = null;
			particleDrag = new FlxPoint();
			frequency = 0.1f;
			lifespan = 3;
			bounce = 0;
			_quantity = 0;
			_counter = 0;
			_explode = true;
			on = false;
			_point = new FlxPoint();
            target = Target;
		}

		/**
		 * Clean up memory.
		 */
		override public void destroy()
		{
			minParticleSpeed = null;
			maxParticleSpeed = null;
			particleDrag = null;
			particleClass = null;
			_point = null;
			base.destroy();
		}

        /// <summary>
        /// Create the particles to be used
        /// </summary>
        /// <param name="Graphics">Texture for the particles - can be one square or a spritesheet.  Set Multiple to true if it is a spritesheet and it will automatically create the particles as long as each frame on the spritesheet is square</param>
        /// <param name="Multiple">Whether or not the Texture contains multiple sprites for particles</param>
        /// <param name="Quantity">The number of particles to generate</param>
        /// <param name="Rotation">The amount of rotation in degrees per frame, so keep this number low</param>
        /// <param name="Collide">The collidability of the particle, 1 = Full and 0 = None</param>
        /// <returns>FlxEmitter</returns>
		public FlxEmitter makeParticles(Texture2D Graphic, bool Multiple=false, uint Quantity = 50, float Rotation = 1f, float Collide = 0.8f)
		{
			maxSize = Quantity;
			FlxParticle particle;
			uint i = 0;
			while(i < Quantity)
			{
                particle = new FlxParticle();
                if (Multiple)
                    particle.loadParticleGraphic(Graphic, Rotation);
                else
                    particle.loadGraphic(Graphic);
				if(Collide > 0)
				{
                    particle.width *= Collide;
                    particle.height *= Collide;
                    particle.centerOffsets();
				}
				else
                    particle.allowCollisions = FlxObject.NONE;
                particle.exists = false;
                add(particle);
				i++;
			}
			return this;
		}

		/**
		 * Called automatically by the game loop, decides when to launch particles and when to "die".
		 */
		override public void update()
		{

            if (target != null)
            {
                FlxPoint t = target.getMidpoint();
                x = t.x;
                y = t.y;
            }

			if(on)
			{
				if(_explode)
				{
					on = false;
					uint i = 0;
					uint l = _quantity;
					if((l <= 0) || (l > length))
						l = (uint)length;
					while(i < l)
					{
						emitParticle();
						i++;
					}
					_quantity = 0;
				}
				else
				{
					_timer += FlxG.elapsed;
					while((frequency > 0) && (_timer > frequency) && on)
					{
						_timer -= frequency;
						emitParticle();
						if((_quantity > 0) && (++_counter >= _quantity))
						{
							on = false;
							_quantity = 0;
						}
					}
				}
			}
			base.update();
		}

		/**
		 * Call this function to turn off all the particles and the emitter.
		 */
		override public void kill()
		{
			on = false;
			base.kill();
		}

		/**
		 * Call this function to start emitting particles.
		 * 
		 * @param	Explode		Whether the particles should all burst out at once.
		 * @param	Lifespan	How long each particle lives once emitted. 0 = forever.
		 * @param	Frequency	Ignored if Explode is set to true. Frequency is how often to emit a particle. 0 = never emit, 0.1 = 1 particle every 0.1 seconds, 5 = 1 particle every 5 seconds.
		 * @param	Quantity	How many particles to launch. 0 = "all of the particles".
		 */
		public void start(bool Explode=true, float Lifespan=0, float Frequency=0.1f, uint Quantity=0)
		{
			revive();
			visible = true;
			on = true;

			_explode = Explode;
			lifespan = Lifespan;
			frequency = Frequency;
			_quantity += Quantity;

			_counter = 0;
			_timer = 0;
		}

		/**
		 * This function can be used both internally and externally to emit the next particle.
		 */
		public void emitParticle()
		{
			FlxParticle particle = recycle(typeof(FlxBasic)) as FlxParticle;
            particle.minSpeed = minParticleSpeed;
            particle.maxSpeed = maxParticleSpeed;
			particle.lifespan = lifespan;
			particle.elasticity = bounce;
            
            if (target != null)
            {
                FlxPoint t = target.getMidpoint();
                particle.reset(t.x, t.y);
            }
            else
            {
                particle.reset(x - (Convert.ToInt32(particle.width) >> 1) + FlxG.random() * width, y - (Convert.ToInt32(particle.height) >> 1) + FlxG.random() * height);                
            }

			particle.visible = true;
			particle.drag.x = particleDrag.x;
			particle.drag.y = particleDrag.y;
			particle.onEmit();
		}

		/**
		 * A more compact way of setting the width and height of the emitter.
		 * 
		 * @param	Width	The desired width of the emitter (particles are spawned randomly within these dimensions).
		 * @param	Height	The desired height of the emitter.
		 */
		public void setSize(uint Width, uint Height)
		{
			width = Width;
			height = Height;
		}

		/**
		 * A more compact way of setting the X velocity range of the emitter.
		 * 
		 * @param	Min		The minimum value for this range.
		 * @param	Max		The maximum value for this range.
		 */
		public void setXSpeed(float Min=0, float Max=0)
		{
			minParticleSpeed.x = Min;
			maxParticleSpeed.x = Max;
		}

		/**
		 * A more compact way of setting the Y velocity range of the emitter.
		 * 
		 * @param	Min		The minimum value for this range.
		 * @param	Max		The maximum value for this range.
		 */
		public void setYSpeed(float Min=0, float Max=0)
		{
			minParticleSpeed.y = Min;
			maxParticleSpeed.y = Max;
		}

		/**
		 * A more compact way of setting the angular velocity constraints of the emitter.
		 * 
		 * @param	Min		The minimum value for this range.
		 * @param	Max		The maximum value for this range.
		 */
		public void setRotation(float Min=0, float Max=0)
		{
			minRotation = Min;
			maxRotation = Max;
		}

		/**
		 * Change the emitter's midpoint to match the midpoint of a <code>FlxObject</code>.
		 * 
		 * @param	Object		The <code>FlxObject</code> that you want to sync up with.
		 */
		public void at(FlxObject Object)
		{
			Object.getMidpoint(_point);
			x = _point.x - (Convert.ToInt32(width)>>1);
			y = _point.y - (Convert.ToInt32(height)>>1);
		}
	}
}
