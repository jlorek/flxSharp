using Microsoft.Xna.Framework.Graphics;
using System;
using fliXNA_xbox;

namespace flxSharp.flxSharp
{
    /// <summary>
    /// <code>FlxEmitter</code> is a lightweight particle emitter.
    /// It can be used for one-time explosions or for
    /// continuous fx like rain and fire.  <code>FlxEmitter</code>
    /// is not optimized or anything; all it does is launch
    /// <code>FlxParticle</code> objects out at set intervals
    /// by setting their positions and velocities accordingly.
    /// It is easy to use and relatively efficient,
    /// relying on <code>FlxGroup</code>'s RECYCLE POWERS.
    /// </summary>
    public class FlxEmitter : FlxGroup
	{
        /// <summary>
        /// The X position of the top left corner of the emitter in world space.
        /// Already inherited by FlxObject.
        /// </summary>
		//public new float x { get; set; }
		
        /// <summary>
        /// The Y position of the top left corner of emitter in world space.
        /// Already inherited by FlxObject.
        /// </summary>
        //public new float y { get; set; }

        /// <summary>
        /// The width of the emitter.  Particles can be randomly generated from anywhere within this box.
        /// Already inherited by FlxObject.
        /// </summary>
		//public new float width { get; set; }

        /// <summary>
        /// The height of the emitter.  Particles can be randomly generated from anywhere within this box.
        /// Already inherited by FlxObject.
        /// </summary>
        //public new float height { get; set; }

        /// <summary>
        /// The minimum possible velocity of a particle.
        /// The default value is (-100,-100).
        /// </summary>
        public FlxPoint minParticleSpeed;

        /// <summary>
        /// The maximum possible velocity of a particle.
        /// The default value is (100,100).
        /// </summary>
        public FlxPoint maxParticleSpeed;

        /// <summary>
        /// The X and Y drag component of particles launched from the emitter.
        /// </summary>
		public FlxPoint particleDrag;

        /// <summary>
        /// The minimum possible angular velocity of a particle.  The default value is -360.
        /// NOTE: rotating particles are more expensive to draw than non-rotating ones!
        /// </summary>
        public float minRotation;

        /// <summary>
        /// The maximum possible angular velocity of a particle.  The default value is 360.
        /// NOTE: rotating particles are more expensive to draw than non-rotating ones!
        /// </summary>
        public float maxRotation;

        /// <summary>
        /// Sets the <code>acceleration.y</code> member of each particle to this value on launch.
        /// </summary>
		public float gravity;

        /// <summary>
        /// Determines whether the emitter is currently emitting particles.
        /// It is totally safe to directly toggle this.
        /// </summary>
		public bool on;

        /// <summary>
        /// How often a particle is emitted (if emitter is started with Explode == false).
        /// </summary>
		public float frequency;

        /// <summary>
        /// How long each particle lives once it is emitted.
        /// Set lifespan to 'zero' for particles to live forever.
        /// </summary>
		public float lifespan;

        /// <summary>
        /// How much each particle should bounce.  1 = full bounce, 0 = no bounce.
        /// </summary>
		public float bounce;

        /// <summary>
        /// Set your own particle class type here.
        /// Default is <code>FlxParticle</code>.
        /// </summary>
		public Object particleClass;

        /// <summary>
        /// Internal helper for deciding how many particles to launch.
        /// </summary>
		protected uint _quantity;

        /// <summary>
        /// Internal helper for the style of particle emission (all at once, or one at a time).
        /// </summary>
		protected bool _explode;

        /// <summary>
        /// Internal helper for deciding when to launch particles or kill them.
        /// </summary>
		protected float _timer;

        /// <summary>
        /// Internal counter for figuring out how many particles to launch.
        /// </summary>
		protected uint _counter;

        /// <summary>
        /// Internal counter for figuring out how many particles to launch.
        /// </summary>
		protected FlxPoint _point;

		/// <summary>
        /// Creates a new <code>FlxEmitter</code> object at a specific position.
        /// Does NOT automatically generate or attach particles!
		/// </summary>
        /// <param name="X">The X position of the emitter.</param>
        /// <param name="Y">The Y position of the emitter.</param>
        /// <param name="size">Optional, specifies a maximum capacity for this emitter.</param>
		public FlxEmitter(float X=0, float Y=0, uint size=0) : base(size)
		{
			((FlxObject) this).X = X;
			((FlxObject) this).Y = Y;

			Width = 0;
			Height = 0;
			
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
		}

        /// <summary>
        /// Clean up memory.
        /// </summary>
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
        /// This function generates a new array of particle sprites to attach to the emitter.
        /// </summary>
        /// <param name="graphics">Texture for the particles - can be one square or a spritesheet.  Set Multiple to true if it is a spritesheet and it will automatically create the particles as long as each frame on the spritesheet is square</param>
        /// <param name="Multiple">Whether or not the Texture contains multiple sprites for particles</param>
        /// <param name="Quantity">The number of particles to generate</param>
        /// <param name="Rotation">The amount of rotation in degrees per frame, so keep this number low</param>
        /// <param name="Collide">The collidability of the particle, 1 = Full and 0 = None</param>
        /// <returns>This FlxEmitter instance (nice for chaining stuff together, if you're into that).</returns>
		//public FlxEmitter makeParticles(Texture2D graphics, bool Multiple=false, uint Quantity = 50, float Rotation = 1f, float Collide = 0.8f)
		public FlxEmitter makeParticles(Texture2D graphics, uint Quantity = 50, uint BakedRotations = 0, bool Multiple = false, float Collide = 0.8f)
        {
			maxSize = Quantity;

            if (Multiple || BakedRotations > 0)
            {
                throw new NotSupportedException();
            }

            for (int i = 0; i < Quantity; ++i)
            {
                var particle = new FlxParticle();
                particle.loadGraphic(graphics);

                if (Collide > 0)
                {
                    particle.Width = particle.Width * Collide;
                    particle.Height = particle.Height * Collide;
                    particle.centerOffsets();
                }
                else
                {
                    particle.AllowCollisions = FlxObject.None;
                }

                particle.Exists = false;
                this.add(particle);
            }

            /*
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
            */

			return this;
		}

        /// <summary>
        /// Called automatically by the game loop, decides when to launch particles and when to "die".
        /// </summary>
		override public void update()
		{
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
					_timer = _timer + FlxG.elapsed;
					while((frequency > 0) && (_timer > frequency) && on)
					{
						_timer = _timer - frequency;
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

        /// <summary>
        /// Call this function to turn off all the particles and the emitter.
        /// </summary>
		override public void kill()
		{
			on = false;
			base.kill();
		}

        /// <summary>
        /// Call this function to start emitting particles.
        /// </summary>
        /// <param name="Explode">Whether the particles should all burst out at once.</param>
        /// <param name="Lifespan">How long each particle lives once emitted. 0 = forever.</param>
        /// <param name="Frequency">Ignored if Explode is set to true. Frequency is how often to emit a particle. 0 = never emit, 0.1 = 1 particle every 0.1 seconds, 5 = 1 particle every 5 seconds.</param>
        /// <param name="Quantity">How many particles to launch. 0 = "all of the particles".</param>
		public void start(bool Explode=true, float Lifespan=0, float Frequency=0.1f, uint Quantity=0)
		{
			revive();
			Visible = true;
			on = true;

			_explode = Explode;
			lifespan = Lifespan;
			frequency = Frequency;
			_quantity += Quantity;

			_counter = 0;
			_timer = 0;
		}

        /// <summary>
        /// This function can be used both internally and externally to emit the next particle.
        /// </summary>
		public void emitParticle()
		{
			var particle = recycle(typeof(FlxBasic)) as FlxParticle;

            if (particle == null)
            {
                throw new Exception("RecyclingWTFException");
            }

            particle.Lifespan = lifespan;
            particle.Elasticity = bounce;
            particle.reset(X - ((int)particle.Width >> 1) + FlxG.random() * Width, Y - ((int)particle.Height >> 1) + FlxG.random() * Height);
            particle.Visible = true;

            // jlorek: revisit

            if (minParticleSpeed.X != maxParticleSpeed.X)
            {
                particle.Velocity.X = minParticleSpeed.X + FlxG.random()*(maxParticleSpeed.X - minParticleSpeed.X);
            }
            else
            {
                particle.Velocity.X = minParticleSpeed.X;                
            }

            if (minParticleSpeed.Y != maxParticleSpeed.Y)
            {
                particle.Velocity.Y = minParticleSpeed.Y + FlxG.random()*(maxParticleSpeed.Y - minParticleSpeed.Y);
            }
            else
            {
                particle.Velocity.Y = minParticleSpeed.Y;                
            }

            particle.Acceleration.Y = gravity;

            if (minRotation != maxRotation)
            {
                particle.AngularVelocity = minRotation + FlxG.random()*(maxRotation - minRotation);
            }
            else
            {
                particle.AngularVelocity = minRotation;                
            }

            if (particle.AngularVelocity != 0)
            {
                particle.Angle = FlxG.random() * 360 - 180;                
            }

			particle.Visible = true;
			particle.Drag.X = particleDrag.X;
			particle.Drag.Y = particleDrag.Y;
			particle.onEmit();
		}

        /// <summary>
        /// A more compact way of setting the width and height of the emitter.
        /// </summary>
        /// <param name="Width">The desired width of the emitter (particles are spawned randomly within these dimensions).</param>
        /// <param name="Height">The desired height of the emitter.</param>
		public void setSize(uint Width, uint Height)
		{
			((FlxObject) this).Width = Width;
			((FlxObject) this).Height = Height;
		}

        /// <summary>
        /// A more compact way of setting the X velocity range of the emitter.
        /// </summary>
        /// <param name="Min">The minimum value for this range.</param>
        /// <param name="Max">The maximum value for this range.</param>
		public void setXSpeed(float Min=0, float Max=0)
		{
			minParticleSpeed.X = Min;
			maxParticleSpeed.X = Max;
		}

        /// <summary>
        /// A more compact way of setting the Y velocity range of the emitter.
        /// </summary>
        /// <param name="Min">The minimum value for this range.</param>
        /// <param name="Max">The maximum value for this range.</param>
		public void setYSpeed(float Min=0, float Max=0)
		{
			minParticleSpeed.Y = Min;
			maxParticleSpeed.Y = Max;
		}

        /// <summary>
        /// A more compact way of setting the angular velocity constraints of the emitter.
        /// </summary>
        /// <param name="Min">The minimum value for this range.</param>
        /// <param name="Max">The maximum value for this range.</param>
		public void setRotation(float Min=0, float Max=0)
		{
			minRotation = Min;
			maxRotation = Max;
		}

        /// <summary>
        /// Change the emitter's midpoint to match the midpoint of a <code>FlxObject</code>.
        /// </summary>
        /// <param name="flxObject">The <code>FlxObject</code> that you want to sync up with.</param>
		public void at(FlxObject flxObject)
		{
			flxObject.getMidpoint(_point);
			X = _point.X - (Convert.ToInt32(Width)>>1);
			Y = _point.Y - (Convert.ToInt32(Height)>>1);
		}
	}
}
