using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace fliXNA_xbox
{
    public class FlxSound : FlxBasic
    {

        protected int playCount;

        /// <summary>
        /// X position of this sound in world coordinates.
        /// Only matters if you're doing proximity/panning
        /// </summary>
        public float x;

        /// <summary>
        /// Y position of this sound in world coordinates.
        /// Only matters if you're doing proximity/panning
        /// </summary>
        public float y;

        /// <summary>
        /// Whether or not this sound should be automatically destroyed when you switch states
        /// </summary>
        public bool survive;

        public string name;
        public string artist;

        /// <summary>
        /// Stores the average wave amplitude of both stereo channels
        /// </summary>
        public float amplitude;

        /// <summary>
        /// Amplitude of left stereo channel
        /// </summary>
        public float amplitudeLeft;

        /// <summary>
        /// Amplitude of right stereo channel
        /// </summary>
        public float amplitudeRight;

        /// <summary>
        /// Whether to call destroy() when the sound has finished
        /// </summary>
        public bool autoDestroy;

        /// <summary>
        /// Internal tracker for the sound object
        /// </summary>
        protected SoundEffectInstance _sound;

        /// <summary>
        /// Internal tracker for the position in runtime of the sound playback
        /// </summary>
        protected float _position;

        /// <summary>
        /// Internal tracker for the volume of the sound
        /// </summary>
        protected float _volume;

        /// <summary>
        /// Internal tracker for total volume adjustment
        /// </summary>
        protected float _volumeAdjust;

        /// <summary>
        /// Internal tracker for whether the sound is looping or not
        /// </summary>
        protected bool _looped;

        /// <summary>
        /// Internal tracker for sound's target - for proximity and panning
        /// </summary>
        protected FlxObject _target;

        /// <summary>
        /// Internal tracker for sound's source - for proximity and panning
        /// </summary>
        protected FlxObject _source;

        /// <summary>
        /// Internal tracker to help with proximity
        /// </summary>
        protected bool proximitized;

        /// <summary>
        /// Internal tracker for maximum effective radius of this sound.
        /// </summary>
        protected float _radius;

        /// <summary>
        /// Internal tracker for whether to pan the sound left and right.  Defualt is false
        /// </summary>
        protected bool _pan;

        /// <summary>
        /// Internal tracker used to keep track of requests to fade out the sound playback
        /// </summary>
        protected float _fadeOutTimer;

        /// <summary>
        /// Internal helper for fading out sounds
        /// </summary>
        protected float _fadeOutTotal;

        /// <summary>
        /// Internal flag for whether to pause or stop the sound when its done fading out
        /// </summary>
        protected bool _pauseOnFadeOut;

        /// <summary>
        /// Internal timer for fading in the sound playback.
        /// </summary>
        protected float _fadeInTimer;

        /// <summary>
        /// Internal helper for fading in sounds.
        /// </summary>
        protected float _fadeInTotal;

        /// <summary>
        /// Internal helper for sound proximity.  The sound will maintain the same position as the target if true.
        /// </summary>
        protected bool _followTarget;

        /// <summary>
        /// FlxSound constructor gets all the variables initialized, but NOT ready to play a sound yet.
        /// </summary>
        public FlxSound() : base()
        {
            createSound();
        }

        protected void createSound()
        {
            destroy();
            x = 0;
            y = 0;
            playCount = 0;
            _sound = null;
            _position = 0;
            _volume = 1.0f;
            _volumeAdjust = 1.0f;
            _looped = false;
            _target = null;
            _source = null;
            _radius = 0;
            _pan = false;
            _fadeOutTimer = 0;
            _fadeOutTotal = 0;
            _pauseOnFadeOut = false;
            _fadeInTimer = 0;
            _fadeInTotal = 0;
            exists = true;
            active = true;
            visible = false;
            name = null;
            artist = null;
            amplitude = 0;
            amplitudeLeft = 0;
            amplitudeRight = 0;
            autoDestroy = false;
            _followTarget = false;
            proximitized = false;
        }

        /**
		 * Clean up memory.
		 */
		override public void destroy()
		{
			kill();

			//_transform = null;
			_sound = null;
			//_channel = null;
			_target = null;
			name = null;
			artist = null;

			base.destroy();
		}

        /**
		 * Handles fade out, fade in, panning, proximity, and amplitude operations each frame.
		 */
		override public void update()
		{
            base.update();
            if (_source != null)
            {
                x = _source.getMidpoint().x;
                y = _source.getMidpoint().y;
            }

            float radial = 1.0f;
            float fade = 1.0f;

			//Distance-based volume control
			if(_target != null)
			{
				radial = (_radius-FlxU.getDistance(_target.getMidpoint(),new FlxPoint(x,y)))/_radius;
				if(radial < 0) radial = 0;
				if(radial > 1) radial = 1;
			}

			//Cross-fading volume control
			if(_fadeOutTimer > 0)
			{
				_fadeOutTimer -= FlxG.elapsed;
				if(_fadeOutTimer <= 0)
				{
					if(_pauseOnFadeOut)
						pause();
					else
						stop();
				}
				fade = _fadeOutTimer/_fadeOutTotal;
				if(fade < 0) fade = 0;
			}
			else if(_fadeInTimer > 0)
			{
				_fadeInTimer -= FlxG.elapsed;
				fade = _fadeInTimer/_fadeInTotal;
				if(fade < 0) fade = 0;
				fade = 1 - fade;
			}

            if ( (autoDestroy && playCount > 0) && (_sound.State != SoundState.Paused && _sound.State != SoundState.Playing))
                _sound.Dispose();

			_volumeAdjust = radial*fade;
			updateTransform();
		}


        /**
		 * One of two main setup functions for sounds, this function loads a sound from an embedded MP3.
		 * 
		 * @param	EmbeddedSound	An embedded Class object representing an MP3 file.
		 * @param	Looped			Whether or not this sound should loop endlessly.
		 * @param	AutoDestroy		Whether or not this <code>FlxSound</code> instance should be destroyed when the sound finishes playing.  Default value is false, but FlxG.play() and FlxG.stream() will set it to true by default.
		 * 
		 * @return	This <code>FlxSound</code> instance (nice for chaining stuff together, if you're into that).
		 */
		public FlxSound loadEmbedded(SoundEffect EmbeddedSound, bool Looped=false, bool AutoDestroy=false)
		{
			stop();
			createSound();
			_sound = EmbeddedSound.CreateInstance();
            autoDestroy = AutoDestroy;
			//NOTE: can't pull ID3 info from embedded sound currently
            _sound.IsLooped = Looped;
			_looped = Looped;
			updateTransform();
			exists = true;
			return this;
		}

        /**
		 * Call this function if you want this sound's volume to change
		 * based on distance from a particular FlxCore object.
		 * 
		 * @param	X		The X position of the sound.
		 * @param	Y		The Y position of the sound.
		 * @param	Object	The object you want to track.
		 * @param	Radius	The maximum distance this sound can travel.
		 * @param	Pan		Whether the sound should pan in addition to the volume changes (default: true).
		 * 
		 * @return	This FlxSound instance (nice for chaining stuff together, if you're into that).
		 */
		public FlxSound proximity(FlxObject Source, FlxObject Target, float Radius)
		{
            x = Source.x;
			y = Source.y;
            _target = Target;
            _source = Source;
			_radius = Radius;
            _followTarget = true;
            proximitized = true;
			return this;
		}

        /**
		 * Call this function to play the sound - also works on paused sounds.
		 * 
		 * @param	ForceRestart	Whether to start the sound over or not.  Default value is false, meaning if the sound is already playing or was paused when you call <code>play()</code>, it will continue playing from its current position, NOT start again from the beginning.
		 */
		public void play(bool ForceRestart=false)
		{
            playCount++;
            if ( (_sound != null) && (_sound.State == SoundState.Paused || _sound.State == SoundState.Stopped) )
                _sound.Play();
		}

        /**
		 * Unpause a sound.  Only works on sounds that have been paused.
		 */
		public void resume()
		{
            if (_sound != null && _sound.State == SoundState.Paused)
                _sound.Resume();
		}

        /**
		 * Call this function to pause this sound.
		 */
		public void pause()
		{
            if (_sound != null && _sound.State == SoundState.Playing)
                _sound.Pause();
		}

        /**
		 * Call this function to stop this sound.
		 */
		public void stop()
		{
            if(_sound != null && _sound.State == SoundState.Playing)
                _sound.Stop();
		}

        /**
		 * Call this function to make this sound fade out over a certain time interval.
		 * 
		 * @param	Seconds			The amount of time the fade out operation should take.
		 * @param	PauseInstead	Tells the sound to pause on fadeout, instead of stopping.
		 */
		public void fadeOut(float Seconds, bool PauseInstead=false)
		{
			_pauseOnFadeOut = PauseInstead;
			_fadeInTimer = 0;
			_fadeOutTimer = Seconds;
			_fadeOutTotal = _fadeOutTimer;
		}

		/**
		 * Call this function to make a sound fade in over a certain
		 * time interval (calls <code>play()</code> automatically).
		 * 
		 * @param	Seconds		The amount of time the fade-in operation should take.
		 */
		public void fadeIn(float Seconds)
		{
			_fadeOutTimer = 0;
			_fadeInTimer = Seconds;
			_fadeInTotal = _fadeInTimer;
			play();
		}

        public float volume
        {
            get { return _volume; }
            set
            {
                _volume = value;
                if (_volume < 0)
                    _volume = 0;
                else if (_volume > 1)
                    _volume = 1;
                updateTransform();
            }
        }

        /**
		 * Returns the currently selected "real" volume of the sound (takes fades and proximity into account).
		 * 
		 * @return	The adjusted volume of the sound.
		 */
		public float getActualVolume()
		{
			return _volume*_volumeAdjust;
		}

		/**
		 * Call after adjusting the volume to update the sound channel's settings.
		 */
		internal void updateTransform()
		{
			_sound.Volume = (FlxG.mute?0:1)*FlxG.volume*_volume*_volumeAdjust;
            //FlxG.log("svol: " + _sound.Volume+" flxgvol: "+FlxG.volume+" _vol "+_volume+" _volAdj "+_volumeAdjust);
		}



    }
}
