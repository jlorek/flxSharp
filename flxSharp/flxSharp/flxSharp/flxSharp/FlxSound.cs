using System;
using Microsoft.Xna.Framework.Audio;

namespace flxSharp.flxSharp
{
    /// <summary>
    /// This is the universal flixel sound object, used for streaming, music, and sound effects.
    /// 
    /// For all the pan stuff
    /// http://allcomputers.us/windows_phone/xna-game-studio-4_0---playing-sound-effects-(part-1)---using-soundeffect-for-audio-playback.aspx
    /// 
    /// TODO: Implement overloaded functionality for Sound (mp3 files) all over the place...
    /// </summary>
    public class FlxSound : FlxBasic
    {
        /// <summary>
        /// The X position of this sound in world coordinates.
        /// Only really matters if you are doing proximity/panning stuff.
        /// </summary>
        public float x;

        /// <summary>
        /// The Y position of this sound in world coordinates.
        /// Only really matters if you are doing proximity/panning stuff.
        /// </summary>
        public float y;

        /// <summary>
        /// Whether or not this sound should be automatically destroyed when you switch states.
        /// </summary>
        public bool survive;

        /// <summary>
        /// The ID3 song name.  Defaults to null.
        /// Currently only works for streamed sounds.
        /// </summary>
        public string name;

        /// <summary>
        /// The ID3 artist name.  Defaults to null.
        /// Currently only works for streamed sounds.
        /// </summary>
        public string artist;

        /// <summary>
        /// Stores the average wave amplitude of both stereo channels.
        /// </summary>
        public float amplitude;

        /// <summary>
        /// Just the amplitude of the left stereo channel.
        /// </summary>
        public float amplitudeLeft;

        /// <summary>
        /// Just the amplitude of the right stereo channel.
        /// </summary>
        public float amplitudeRight;

        /// <summary>
        /// Whether to call destroy() when the sound has finished.
        /// </summary>
        public bool autoDestroy;

        /// <summary>
        /// Internal tracker for a XNA sound object.
        /// </summary>
        protected SoundEffectInstance _sound;

        /**
		 * Internal tracker for a Flash sound channel object.
		 */
		//protected var _channel:SoundChannel;
		
        /**
		 * Internal tracker for a Flash sound transform object.
		 */
		//protected var _transform:SoundTransform;

        /// <summary>
        /// Internal tracker for the position in runtime of the music playback.
        /// </summary>
        protected float _position;

        /// <summary>
        /// Internal tracker for how loud the sound is.
        /// </summary>
        protected float _volume;

        /// <summary>
        /// Set <code>volume</code> to a value between 0 and 1 to change how this sound is.
        /// </summary>
        public float Volume
        {
            get { return _volume; }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                else if (value > 1)
                {
                    value = 1;
                }

                _volume = value;
                updateTransform();
            }
        }

        /// <summary>
        /// Internal tracker for total volume adjustment.
        /// </summary>
        protected float _volumeAdjust;

        /// <summary>
        /// Internal tracker for whether the sound is looping or not.
        /// </summary>
        protected bool _looped;

        /// <summary>
        /// Internal tracker for the sound's "target" (for proximity and panning).
        /// </summary>
        protected FlxObject _target;

        /// <summary>
        /// Internal tracker for the maximum effective radius of this sound (for proximity and panning).
        /// </summary>
        protected float _radius;

        /// <summary>
        /// Internal tracker for whether to pan the sound left and right. Default is false.
        /// </summary>
        protected bool _pan;

        /// <summary>
        /// Internal timer used to keep track of requests to fade out the sound playback.
        /// </summary>
        protected float _fadeOutTimer;

        /// <summary>
        /// Internal helper for fading out sounds.
        /// </summary>
        protected float _fadeOutTotal;

        /// <summary>
        /// Internal flag for whether to pause or stop the sound when it's done fading out.
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

        /*
        protected int playCount;
         
        /// <summary>
        /// Internal tracker for sound's source - for proximity and panning
        /// </summary>
        protected FlxObject _source;

        /// <summary>
        /// Internal tracker to help with proximity
        /// </summary>
        protected bool proximitized;

        /// <summary>
        /// Internal helper for sound proximity.  The sound will maintain the same position as the target if true.
        /// </summary>
        protected bool _followTarget;
        */

        /// <summary>
        /// The FlxSound constructor gets all the variables initialized, but NOT ready to play a sound yet.
        /// </summary>
        public FlxSound() : base()
        {
            createSound();
        }

        /// <summary>
        /// An internal function for clearing all the variables used by sounds.
        /// </summary>
        protected void createSound()
        {
            destroy();
            
            x = 0;
            y = 0;
            _sound = null;
            _position = 0;
            _volume = 1.0f;
            _volumeAdjust = 1.0f;
            _looped = false;
            _target = null;
            _radius = 0;
            _pan = false;
            _fadeOutTimer = 0;
            _fadeOutTotal = 0;
            _pauseOnFadeOut = false;
            _fadeInTimer = 0;
            _fadeInTotal = 0;
            Exists = false;
            Active = false;
            Visible = false;
            name = null;
            artist = null;
            amplitude = 0;
            amplitudeLeft = 0;
            amplitudeRight = 0;
            autoDestroy = false;
        }

        /// <summary>
        /// Clean up memory.
        /// </summary>
		override public void destroy()
		{
			kill();

            if (_sound != null)
            {
                _sound.Dispose();
            }
		    _sound = null;
			_target = null;
			name = null;
			artist = null;

			base.destroy();
		}

        /// <summary>
        /// Handles fade out, fade in, panning, proximity, and amplitude operations each frame.
        /// </summary>
		override public void update()
		{
            base.update();

            /*
            if (_source != null)
            {
                x = _source.getMidpoint().X;
                y = _source.getMidpoint().Y;
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
            */
		}

        public override void kill()
        {
            base.kill();

            if ((_sound != null) && _sound.State == SoundState.Playing)
            {
                _sound.Stop(true);
            }
        }

        /// <summary>
        /// One of two main setup functions for sounds, this function loads a sound from an embedded MP3.
        /// </summary>
        /// <param name="embeddedSound">An embedded Class object representing an MP3 file.</param>
        /// <param name="looped">Whether or not this sound should loop endlessly.</param>
        /// <param name="autoDestroy">Whether or not this <code>FlxSound</code> instance should be destroyed when the sound finishes playing.  Default value is false, but FlxG.play() and FlxG.stream() will set it to true by default.</param>
        /// <returns>This <code>FlxSound</code> instance (nice for chaining stuff together, if you're into that).</returns>
		public FlxSound loadEmbedded(SoundEffect embeddedSound, bool looped = false, bool autoDestroy = false)
        {
			stop();
			createSound();
            this.autoDestroy = autoDestroy;
            // NOTE: can't pull ID3 info from embedded sound currently
            _sound = embeddedSound.CreateInstance();
            _looped = looped;
			updateTransform();
			Exists = true;

            // flx# stuff
            _sound.IsLooped = looped;

			return this;

            // flx# - what about autoDestroy?
		}

        /// <summary>
        /// One of two main setup functions for sounds, this function loads a sound from a URL.
        /// </summary>
        /// <param name="soundUrl">A string representing the URL of the MP3 file you want to play.</param>
        /// <param name="looped">Whether or not this sound should loop endlessly.</param>
        /// <param name="autoDestroy">Whether or not this <code>FlxSound</code> instance should be destroyed when the sound finishes playing.  Default value is false, but FlxG.play() and FlxG.stream() will set it to true by default.</param>
        /// <returns>This <code>FlxSound</code> instance (nice for chaining stuff together, if you're into that).</returns>
        public FlxSound loadStream(string soundUrl, bool looped = false, bool autoDestroy = false)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Call this function if you want this sound's volume to change
        /// based on distance from a particular FlxCore object.
        /// </summary>
        /// <param name="x">The X position of the sound.</param>
        /// <param name="y">The Y position of the sound.</param>
        /// <param name="target">The object you want to track.</param>
        /// <param name="radius">The maximum distance this sound can travel.</param>
        /// <param name="pan">Whether the sound should pan in addition to the volume changes (default: true).</param>
        /// <returns>This FlxSound instance (nice for chaining stuff together, if you're into that).</returns>
		public FlxSound proximity(float x, float y, FlxObject target, float radius, bool pan = true)
		{
		    this.x = x;
		    this.y = y;
		    _target = target;
		    _radius = radius;
		    _pan = pan;
			return this;
		}

        /// <summary>
        /// Call this function to play the sound - also works on paused sounds.
        /// </summary>
        /// <param name="forceRestart">Whether to start the sound over or not.  Default value is false, meaning if the sound is already playing or was paused when you call <code>play()</code>, it will continue playing from its current position, NOT start again from the beginning.</param>
		public void play(bool forceRestart = false)
		{
            if (forceRestart && ((_sound.State == SoundState.Playing) || (_sound.State == SoundState.Paused)))
            {
                _sound.Stop();
                _sound.Play();
            }

            _sound.Play();
            _position = 0;
		}

        /// <summary>
        /// Unpause a sound. Only works on sounds that have been paused.
        /// </summary>
		public void resume()
		{
            if (_sound != null)
            {
                if (_sound.State == SoundState.Paused)
                {
                    _sound.Resume();
                }
            }
		}

        /// <summary>
        /// Call this function to pause this sound.
        /// </summary>
		public void pause()
		{
            if (_sound != null)
            {
                if (_sound.State == SoundState.Playing)
                {
                    _sound.Pause();
                }
            }
		}

        /// <summary>
        /// Call this function to stop this sound.
        /// </summary>
		public void stop()
		{
            if (_sound != null)
            {
                _sound.Stop();                
            }
		}

        /// <summary>
        /// Call this function to make this sound fade out over a certain time interval.
        /// </summary>
        /// <param name="seconds">The amount of time the fade out operation should take.</param>
        /// <param name="pauseInstead">Tells the sound to pause on fadeout, instead of stopping.</param>
		public void fadeOut(float seconds, bool pauseInstead = false)
		{
			_pauseOnFadeOut = pauseInstead;
			_fadeInTimer = 0;
			_fadeOutTimer = seconds;
			_fadeOutTotal = _fadeOutTimer;
		}

        /// <summary>
        /// Call this function to make a sound fade in over a certain
        /// time interval (calls <code>play()</code> automatically).
        /// </summary>
        /// <param name="seconds">The amount of time the fade-in operation should take.</param>
		public void fadeIn(float seconds)
		{
			_fadeOutTimer = 0;
			_fadeInTimer = seconds;
			_fadeInTotal = _fadeInTimer;
			play();
		}

        /// <summary>
        /// Returns the currently selected "real" volume of the sound (takes fades and proximity into account).
        /// </summary>
        /// <returns>The adjusted volume of the sound.</returns>
		public float getActualVolume()
		{
			return _volume * _volumeAdjust;
		}

        /// <summary>
        /// Call after adjusting the volume to update the sound channel's settings.
        /// </summary>
		internal void updateTransform()
		{
			float soundVolume = (FlxG.mute ? 0 : 1) * FlxG.Volume * _volume * _volumeAdjust;
            _sound.Volume = soundVolume;

            /*
			_transform.volume = (FlxG.mute?0:1)*FlxG.volume*_volume*_volumeAdjust;
			if(_channel != null)
				_channel.soundTransform = _transform;
            */
		}

        /**
		 * An internal helper function used to help Flash resume playing a looped sound.
		 * 
		 * @param	event		An <code>Event</code> object.
		 */
        //protected function looped(event:Event=null):void
        //{
        //    if (_channel == null)
        //        return;
        //    _channel.removeEventListener(Event.SOUND_COMPLETE,looped);
        //    _channel = null;
        //    play();
        //}

		/**
		 * An internal helper function used to help Flash clean up and re-use finished sounds.
		 * 
		 * @param	event		An <code>Event</code> object.
		 */
        //protected function stopped(event:Event=null):void
        //{
        //    if(!_looped)
        //        _channel.removeEventListener(Event.SOUND_COMPLETE,stopped);
        //    else
        //        _channel.removeEventListener(Event.SOUND_COMPLETE,looped);
        //    _channel = null;
        //    active = false;
        //    if(autoDestroy)
        //        destroy();
        //}
		
		/**
		 * Internal event handler for ID3 info (i.e. fetching the song name).
		 * 
		 * @param	event	An <code>Event</code> object.
		 */
        //protected function gotID3(event:Event=null):void
        //{
        //    FlxG.log("got ID3 info!");
        //    if(_sound.id3.songName.length > 0)
        //        name = _sound.id3.songName;
        //    if(_sound.id3.artist.length > 0)
        //        artist = _sound.id3.artist;
        //    _sound.removeEventListener(Event.ID3, gotID3);
        //}

    }
}
