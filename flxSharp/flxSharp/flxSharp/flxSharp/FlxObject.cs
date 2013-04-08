using fliXNA_xbox;
using flxSharp.flxSharp.System;
using Microsoft.Xna.Framework;
using System;

namespace flxSharp.flxSharp
{
    /// <summary>
    /// This is the base class for most of the display objects (<code>FlxSprite</code>, <code>FlxText</code>, etc).
    /// It includes some basic attributes about game objects, including retro-style flickering,
    /// basic state information, sizes, scrolling, and basic physics and motion.
    /// </summary>
    public class FlxObject : FlxBasic
    {
        /// <summary>
        /// Generic value for "left" used by <code>facing</code>, <code>allowCollisions</code>, and <code>touching</code>.
        /// </summary>
        public const uint Left = 0x0001;

        /// <summary>
        /// Generic value for "right" used by <code>facing</code>, <code>allowCollisions</code>, and <code>touching</code>.
        /// </summary>
        public const uint Right = 0x0010;

        /// <summary>
        /// Generic value for "up" used by <code>facing</code>, <code>allowCollisions</code>, and <code>touching</code>.
        /// </summary>
        public const uint Up = 0x0100;

        /// <summary>
        /// Generic value for "down" used by <code>facing</code>, <code>allowCollisions</code>, and <code>touching</code>.
        /// </summary>
        public const uint Down = 0x1000;

        /// <summary>
        /// Special-case constant meaning no collisions, used mainly by <code>allowCollisions</code>, and <code>touching</code>.
        /// </summary>
        public const uint None = 0;

        /// <summary>
        /// Special-case constant meaning up, used mainly by <code>allowCollisions</code>, and <code>touching</code>.
        /// </summary>
        public const uint Ceiling = Up;

        /// <summary>
        /// Special-case constant meaning down, used mainly by <code>allowCollisions</code>, and <code>touching</code>.
        /// </summary>
        public const uint Floor = Down;

        /// <summary>
        /// Special-case constant meaning only the left and right sides, used mainly by <code>allowCollisions</code>, and <code>touching</code>.
        /// </summary>
        public const uint Wall = Left | Right;

        /// <summary>
        /// Special-case constant meaning any direction, used mainly by <code>allowCollisions</code>, and <code>touching</code>.
        /// </summary>
        public const uint Any = Left | Right | Up | Down;

        /// <summary>
        /// Handy constant used during collision resolution (see <code>separateX()</code> and <code>separateY()</code>).
        /// </summary>
        public const float OverlapBias = 4;

        /// <summary>
        /// Path behavior controls: move from start of the path to the end then stop.
        /// </summary>
        public const uint PathForward = 0x000000;

        /// <summary>
        /// Path behavior controls: move from end of the path to the start then stop.
        /// </summary>
        public const uint PathBackward = 0x000001;

        /// <summary>
        /// Path behavior controls: move from start of the path to the end then directly back to start, and start over.
        /// </summary>
        public const uint PathLoopForward = 0x000010;

        /// <summary>
        /// Path behavior controls: move from end of the path to the start then directly back to end, and start over.
        /// </summary>
        public const uint PathLoopBackward = 0x000100;

        /// <summary>
        /// Path behavior controls: move from start of the path to the end then directly back to start, over and over.
        /// </summary>
        public const uint PathYoyo = 0x001000;

        /// <summary>
        /// Path behavior controls: ignores any vertical component to the path data, only follows side to side.
        /// </summary>
        public const uint PathHorizontalOnly = 0x010000;

        /// <summary>
        /// Path behavior controls: ignores any horizontal component to the path data, only follows up and down.
        /// </summary>
        public const uint PathVerticalOnly = 0x100000;

        /// <summary>
        /// X position of the upper left corner of this object in world space.
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Y position of the upper left corner of this object in world space.
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// The width of this object.
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// The height of this object.
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        /// Whether this object will move/alter position after a collision.
        /// </summary>
        public bool Immovable { get; set; }

        /// <summary>
        /// Basic speed of this object.
        /// </summary>
        public FlxPoint Velocity;

        /// <summary>
        /// Virtual mass of this object.
        /// Currently only used with <code>elasticity</code> during collision resolution.
        /// Change at your own risk; effects seem crazy unpredictable so far!
        /// Defauly value is 1.
        /// </summary>
        public float Mass { get; set; }

        /// <summary>
        /// The bounciness of this object, Only affects collisions.
        /// Default is 0, or "not bouncy at all."
        /// </summary>
        public float Elasticity { get; set; }

        /// <summary>
        /// How fast the speed of this object is changing.
        /// Useful for smooth movement and gravity
        /// </summary>
        public FlxPoint Acceleration;

        /// <summary>
        /// This is like deceleration that is only applied
        /// when acceleration is not affecting the sprite.
        /// </summary>
        public FlxPoint Drag;

        /// <summary>
        /// If you are using <code>acceleration</code>, you can use <code>MaxVelocity</code> with it
        /// to cap the speed automatically (very useful!).
        /// </summary>
        public FlxPoint MaxVelocity;

        /// <summary>
        /// Set the angle of a sprite to rotate it.
        /// WARNING: rotating sprites decreases rendering
        /// performance for this sprite by a factor of 10x!
        /// 
        /// flx# - This performance impact references FlashFlixel.
        /// </summary>
        public float Angle { get; set; }

        /// <summary>
        /// How fast you want the sprite to spin.
        /// </summary>
        public float AngularVelocity { get; set; }

        /// <summary>
        /// How fast the spin should change.
        /// </summary>
        public float AngularAcceleration { get; set; }

        /// <summary>
        /// Like <code>Drag</code> but for spinning.
        /// </summary>
        public float AngularDrag { get; set; }

        /// <summary>
        /// Use in conjunction with <code>AngularAcceleration</code> for fluid spin speed control.
        /// </summary>
        public float MaxAngular { get; set; }

        /// <summary>
        /// Should always represent (0,0) - useful for different things, for avoideing unnecessary <code>new</code> calls
        /// </summary>
        protected static FlxPoint PZero = new FlxPoint();

        /// <summary>
        /// A point that can store numbers from 0 to 1 (for X and Y independently)
        /// that governs how much this object is affected by the camera subsystem.
        /// 0 means it never moves, like a HUD element or far background graphic.
        /// 1 means it scrolls along a the same speed as the foreground layer.
        /// scrollFactor is initialized as (1,1) by default.
        /// 
        /// flx# - CURRENTLY NOT IMPLEMENTED
        /// </summary>
        public FlxPoint ScrollFactor;

        /// <summary>
        /// Internal helper for Retro-styled flickering.
        /// </summary>
        protected bool _flicker;

        /// <summary>
        /// Internal helper for Retro-styled flickering.
        /// </summary>
        protected float _flickerTimer;

        /// <summary>
        /// Handy for storing health percentage or armor points or whatever.
        /// </summary>
        public float Health { get; set; }

        /// <summary>
        /// This is just a pre-allocated x-y point container to be used however you like.
        /// </summary>
        protected FlxPoint _tagPoint;

        /// <summary>
        /// This is just a pre-allocated rectangle container to be used however you like.
        /// </summary>
        protected FlxRect _tagRect;

        /// <summary>
        /// Set this to false if you want to skip the automatic motion/movement stuff (see <code>updateMotion()</code>).
        /// FlxObject and FlxSprite default to true.
        /// FlxText, FlxTileblock, FlxTilemap and FlxSound default to false.
        /// </summary>
        public bool Moves { get; set; }

        /// <summary>
        /// Bit field of flags (use with UP, DOWN, LEFT, RIGHT, etc) indicating surface contacts.
        /// Use bitwise operators to check the values stored here, or use touching(), justStartedTouching(), etc.
        /// You can even use them broadly as boolean values if you're feeling saucy!
        /// </summary>
        public uint Touching { get; set; }

        /// <summary>
        /// Bit field of flags (use with UP, DOWN, LEFT, RIGHT, etc) indicating surface contacts from the previous game loop step.
        /// Use bitwise operators to check the values stored here, or use touching(), justStartedTouching(), etc.
        /// You can even use them broadly as boolean values if you're feeling saucy!
        /// </summary>
        public uint WasTouching { get; set; }

        /// <summary>
        /// Bit field of flags (use with UP, DOWN, LEFT, RIGHT, etc) indicating collision directions.
        /// Use bitwise operators to check the values stored here.
        /// Useful for things like one-way platforms (e.g. allowCollisions = UP;)
        /// The accessor "solid" just flips this variable between NONE and ANY/
        /// </summary>
        public uint AllowCollisions { get; set; }

        /// <summary>
        /// Important variable for collision processing.
        /// By default this value is set automatically during <code>preUpdate()</code>.
        /// </summary>
        public FlxPoint Last { get; set; }

        /// <summary>
        /// A reference to a path object.
        /// Null by default, assigned by <code>followPath()</code>.
        /// </summary>
        public FlxPath Path { get; set; }

        /// <summary>
        /// The speed at which the object is moving on the path.
        /// When an object completes a non-looping path circuit,
        /// the pathSpeed will be zeroed out, but the <code>Path</code> reference
        /// will NOT be nulled out.  So <code>PathSpeed</code> is a good way
        /// to check if this object is currently following a path or not.
        /// </summary>
        public float PathSpeed { get; set; }

        /// <summary>
        /// The angle in degrees between this object and the next node, where 0 is directly upward, and 90 is to the right.
        /// </summary>
        public float PathAngle { get; set; }

        /// <summary>
        /// Internal helper, tracks which node of the path this object is moving toward.
        /// </summary>
        protected int _pathNodeIndex;
        
        /// <summary>
        /// Internal tracker for path behavior flags (like looping, horizontal only, etc).
        /// </summary>
        protected uint _pathMode;

        /// <summary>
        /// Internal helper for node navigation, specifically yo-yo and backwards movement.
        /// </summary>
        protected int _pathInc;

        /// <summary>
        /// Internal flag for whether the object's angle should be adjusted to the path angle during path follow behavior.
        /// </summary>
        protected bool _pathRotate;

        /// <summary>
        /// Instantiates a <code>FlxObject</code>.
        /// </summary>
        /// <param name="x">X-coordinate of the object in space.</param>
        /// <param name="y">y-coordinate of the object in space.</param>
        /// <param name="width">Desired width of the rectangle.</param>
        /// <param name="height">Desired height of the rectangle.</param>
        public FlxObject(float x = 0, float y = 0, float width = 0, float height = 0) : base()
        {
            X = x;
            Y = y;
            Last = new FlxPoint(x, y);
            Width = width;
            Height = height;
            Mass = 1.0f;
            Elasticity = 0.0f;

            Immovable = false;
            Moves = true;

            Touching = None;
            WasTouching = None;
            AllowCollisions = Any;

            Velocity = new FlxPoint();
            Acceleration = new FlxPoint();
            Drag = new FlxPoint();
            MaxVelocity = new FlxPoint(10000, 10000);

            Angle = 0;
            AngularVelocity = 0;
            AngularAcceleration = 0;
            AngularDrag = 0;
            MaxAngular = 10000;

            ScrollFactor = new FlxPoint(1, 1);
            _flicker = false;
            _flickerTimer = 0;

            _tagPoint = new FlxPoint();
            _tagRect = new FlxRect();

            Path = null;
            PathSpeed = 0;
            PathAngle = 0;

            // flx# - ?
            //Health = 1;
        }

        /// <summary>
        /// Override this function to null out variables or
        /// manually call destroy() on class members if necessary.
        /// Don't forget to call super.destroy()!
        /// </summary>
        public override void destroy()
		{
            Velocity = null;
            Acceleration = null;
            Drag = null;
            MaxVelocity = null;
            ScrollFactor = null;
            _tagPoint = null;
            _tagRect = null;
            Last = null;
            
            // flx# - ?
            //cameras = null;

            if (Path != null)
            {
                Path.destroy();
                Path = null;
            }

            base.destroy();
		}
        
        /// <summary>
        /// Pre-update is called right before <code>update()</code> on each object in the game loop.
        /// In <code>FlxObject</code> it controls the flicker timer,
        /// tracking the last coordinates for collision purposes,
        /// and checking if the object is moving along a path or not.
        /// </summary>
        public override void preUpdate()
        {
            activeCount++;

            // flx# - floatround
            if (_flickerTimer != 0)
            {
                if (_flickerTimer > 0)
                {
                    _flickerTimer = _flickerTimer - FlxG.elapsed;

                    if (_flickerTimer <= 0)
                    {
                        _flickerTimer = 0;
                        _flicker = false;
                        reset(X, Y);
                    }
                }
            }

            Last.X = X;
            Last.Y = Y;

            if ((Path != null) && (PathSpeed != 0) && (Path.Nodes[_pathNodeIndex] != null))
            {
                updatePathMotion();
            }
        }

        /// <summary>
        /// Post-update is called right after <code>update()</code> on each object in the game loop.
        /// In <code>FlxObject</code> this function handles integrating the objects motion
        /// based on the velocity and acceleration settings, and tracking/clearing the <code>touching</code> flags.
        /// </summary>
        public override void postUpdate()
        {
            if (Moves)
            {
                updateMotion();                
            }

            WasTouching = Touching;
            Touching = None;
        }

        /// <summary>
        /// Internal function for updating the position and speed of this object.
        /// Useful for cases when you need to update this but are buried down in too many supers.
        /// Does a slightly fancier-than-normal integration to help with higher fidelity framerate-independenct motion.
        /// </summary>
        protected void updateMotion()
        {
            float delta;
            float velocityDelta;

            velocityDelta = (FlxU.computeVelocity(AngularVelocity, AngularAcceleration, AngularDrag, MaxAngular) - AngularVelocity) / 2;
            AngularVelocity += velocityDelta;
            Angle += AngularVelocity * FlxG.elapsed;
            AngularVelocity += velocityDelta;

            velocityDelta = (FlxU.computeVelocity(Velocity.X, Acceleration.X, Drag.X, MaxVelocity.X) - Velocity.X) / 2;
            Velocity.X += velocityDelta;
            delta = Velocity.X * FlxG.elapsed;
            Velocity.X += velocityDelta;
            X += delta;

            velocityDelta = (FlxU.computeVelocity(Velocity.Y, Acceleration.Y, Drag.Y, MaxVelocity.Y) - Velocity.Y) / 2;
            Velocity.Y += velocityDelta;
            delta = Velocity.Y * FlxG.elapsed;
            Velocity.Y += velocityDelta;
            Y += delta;
        }

        /// <summary>
        /// Rarely called, and in this case just increments the visible objects count and calls <code>drawDebug()</code> if necessary.
        /// </summary>
        public override void draw()
        {
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

                visibleCount++;

                if (FlxG.visualDebug && !IgnoreDrawDebug)
                {
                    drawDebug(camera);
                }
            }
        }

        /// <summary>
        /// #flx - Not yet implemented / missing FlxG.flashGfx for direct drawing (draw on immovable debug overlay?)
        /// </summary>
        /// <param name="Camera">Which Camera - currently only one exists</param>
        public override void drawDebug(FlxCamera Camera=null)
		{
            throw new NotImplementedException();

			if(Camera == null)
				Camera = FlxG.camera;

			//get bounding box coordinates
			float boundingBoxX = X - (int)(Camera.Scroll.X*ScrollFactor.X); //copied from getScreenXY()
			float boundingBoxY = Y - (int)(Camera.Scroll.Y*ScrollFactor.Y);
			boundingBoxX = (int)(boundingBoxX + ((boundingBoxX > 0)?0.0000001f:-0.0000001f));
			boundingBoxY = (int)(boundingBoxY + ((boundingBoxY > 0)?0.0000001f:-0.0000001f));
            int boundingBoxWidth = (int)((Width != (int)(Width)) ? Width : Width - 1f);
            int boundingBoxHeight = (int)((Height != (int)(Height)) ?Height : Height - 1f);

            ////fill static graphics object with square shape
            //var gfx:Graphics = FlxG.flashGfx;
            //gfx.clear();
            //gfx.moveTo(boundingBoxX,boundingBoxY);
			Color boundingBoxColor;
			if(Convert.ToBoolean(AllowCollisions))
			{
				if(AllowCollisions != Any)
                    boundingBoxColor = FlxColor.PINK;
				if(Immovable)
                    boundingBoxColor = FlxColor.GREEN;
				else
                    boundingBoxColor = FlxColor.RED;
			}
			else
                boundingBoxColor = FlxColor.BLUE;
            //gfx.lineStyle(1,boundingBoxColor,0.5);
            //gfx.lineTo(boundingBoxX+boundingBoxWidth,boundingBoxY);
            //gfx.lineTo(boundingBoxX+boundingBoxWidth,boundingBoxY+boundingBoxHeight);
            //gfx.lineTo(boundingBoxX,boundingBoxY+boundingBoxHeight);
            //gfx.lineTo(boundingBoxX,boundingBoxY);

            ////draw graphics shape to camera buffer
            //Camera.buffer.draw(FlxG.flashGfxSprite);
		}

        /// <summary>
        /// Call this function to give this object a path to follow.
        /// If the path does not have at least one node in it, this function
        /// will log a warning message and return.
        /// </summary>
        /// <param name="path">The <code>FlxPath</code> you want this object to follow.</param>
        /// <param name="speed">How fast to travel along the path in pixels per second.</param>
        /// <param name="mode">Optional, controls the behavior of the object following the path using the path behavior constants. Can use multiple flags at once, for example PATH_YOYO|PATH_HORIZONTAL_ONLY will make an object move back and forth along the X axis of the path only.</param>
        /// <param name="autoRotate">Automatically point the object toward the next node.  Assumes the graphic is pointing upward.  Default behavior is false, or no automatic rotation.</param>
        public void followPath(FlxPath path, float speed = 100, uint mode = PathForward, bool autoRotate = false)
        {
            if (path.Nodes.Count <= 0)
            {
                FlxG.log("WARNING: Paths need at least one node in them to be followed.");
                return;
            }

            Path = path;
            PathSpeed = FlxU.abs(speed);
            _pathMode = mode;
            _pathRotate = autoRotate;

            // get starting node
            if ((_pathMode == PathBackward) || (_pathMode == PathLoopBackward)) // flx# - no bitmask?
            {
                _pathNodeIndex = Path.Nodes.Count - 1;
                _pathInc = -1;
            }
            else
            {
                _pathNodeIndex = 0;
                _pathInc = 1;
            }
        }

        /// <summary>
        /// Tells this object to stop following the path its on.
        /// </summary>
        /// <param name="destroyPath">Tells this function whether to call destroy on the path object. Default value is false.</param>
		public void stopFollowingPath(bool destroyPath = false)
		{
			PathSpeed = 0;

			if(destroyPath && (Path != null))
			{
				Path.destroy();
				Path = null;
			}
		}

        /// <summary>
        /// Internal function that decides what node in the path to aim for next based on the behavior flags.
        /// </summary>
        /// <param name="snap">?</param>
        /// <returns>The node (a <code>FlxPoint</code> object) we are aiming for next.</returns>
		protected FlxPoint advancePath(bool snap=true)
		{
			if(snap)
			{
				FlxPoint oldNode = Path.Nodes[_pathNodeIndex];
				if(oldNode != null)
				{
					if ((_pathMode & PathVerticalOnly) == 0)
					{
                        X = oldNode.X - Width * 0.5f;					    
					}

					if ((_pathMode & PathHorizontalOnly) == 0)
					{
                        Y = oldNode.Y - Height * 0.5f;					    
					}
				}
			}

			_pathNodeIndex = _pathNodeIndex + _pathInc;

			if((_pathMode & PathBackward) > 0)
			{
				if(_pathNodeIndex < 0)
				{
					_pathNodeIndex = 0;
					PathSpeed = 0;
				}
			}
			else if((_pathMode & PathLoopForward) > 0)
			{
				if (_pathNodeIndex >= Path.Nodes.Count)
				{
                    _pathNodeIndex = 0;				    
				}
			}
			else if((_pathMode & PathLoopBackward) > 0)
			{
				if(_pathNodeIndex < 0)
				{
					_pathNodeIndex = Path.Nodes.Count - 1;
					if (_pathNodeIndex < 0)
					{
                        _pathNodeIndex = 0;					    
					}
				}
			}
			else if((_pathMode & PathYoyo) > 0)
			{
				if(_pathInc > 0)
				{
                    if (_pathNodeIndex >= Path.Nodes.Count)
					{
                        _pathNodeIndex = Path.Nodes.Count - 2;
						if (_pathNodeIndex < 0)
						{
                            _pathNodeIndex = 0;						    
						}
						_pathInc = -_pathInc;
					}
				}
				else if(_pathNodeIndex < 0)
				{
					_pathNodeIndex = 1;
                    if (_pathNodeIndex >= Path.Nodes.Count)
                    {
                        _pathNodeIndex = Path.Nodes.Count - 1;                        
                    }

					if (_pathNodeIndex < 0)
					{
                        _pathNodeIndex = 0;					    
					}
					_pathInc = -_pathInc;
				}
			}
			else
			{
                if (_pathNodeIndex >= Path.Nodes.Count)
				{
                    _pathNodeIndex = Path.Nodes.Count - 1;
					PathSpeed = 0;
				}
			}

			return Path.Nodes[_pathNodeIndex];
		}

        /// <summary>
        /// Internal function for moving the object along the path.
        /// Generally this function is called automatically by <code>preUpdate()</code>.
        /// The first half of the function decides if the object can advance to the next node in the path,
        /// while the second half handles actually picking a velocity toward the next node.
        /// </summary>
		protected void updatePathMotion()
		{
			// first check if we need to be pointing at the next node yet
			_tagPoint.X = X + Width * 0.5f;
			_tagPoint.Y = Y + Height * 0.5f;
			FlxPoint node = Path.Nodes[_pathNodeIndex];
			float deltaX = node.X - _tagPoint.X;
			float deltaY = node.Y - _tagPoint.Y;

			bool horizontalOnly = (_pathMode & PathHorizontalOnly) > 0;
			bool verticalOnly = (_pathMode & PathVerticalOnly) > 0;

			if(horizontalOnly)
			{
				if(((deltaX>0)?deltaX:-deltaX) < PathSpeed*FlxG.elapsed)
					node = advancePath();
			}
			else if(verticalOnly)
			{
				if (((deltaY > 0) ? deltaY : -deltaY) < PathSpeed*FlxG.elapsed)
				{
                    node = advancePath();				    
				}
			}
			else
			{
				if (Math.Sqrt(deltaX*deltaX + deltaY*deltaY) < PathSpeed*FlxG.elapsed)
				{
                    node = advancePath();				    
				}
			}

			// then just move toward the current node at the requested speed
			if(PathSpeed != 0)
			{
				//set velocity based on path mode
				_tagPoint.X = X + Width * 0.5f;
				_tagPoint.Y = Y + Height * 0.5f;

				if(horizontalOnly || (_tagPoint.Y == node.Y))
				{
					Velocity.X = (_tagPoint.X < node.X) ? PathSpeed: -PathSpeed;

					if (Velocity.X < 0)
					{
					    PathAngle = -90;
					}
					else
					{
                        PathAngle = 90;					    
					}

					if (!horizontalOnly)
					{
                        Velocity.Y = 0;					    
					}
				}
				else if(verticalOnly || (_tagPoint.X == node.X))
				{
					Velocity.Y = (_tagPoint.Y < node.Y) ? PathSpeed : -PathSpeed;
					
                    if (Velocity.Y < 0)
                    {
                        PathAngle = 0;
                    }
                    else
                    {
                        PathAngle = 180;                        
                    }

					if (!verticalOnly)
					{
                        Velocity.X = 0;					    
					}
				}
				else
				{
					PathAngle = FlxU.getAngle(_tagPoint, node);
					FlxU.rotatePoint(0, PathSpeed, 0, 0, PathAngle, Velocity);
				}

				// then set object rotation if necessary
				if(_pathRotate)
				{
					AngularVelocity = 0;
					AngularAcceleration = 0;
					Angle = PathAngle;
				}
			}			
		}


        /// <summary>
        /// Checks to see if some <code>FlxObject</code> overlaps this <code>FlxObject</code> or <code>FlxGroup</code>.
        /// If the group has a LOT of things in it, it might be faster to use <code>FlxG.overlaps()</code>.
        /// WARNING: Currently tilemaps do NOT support screen space overlap checks!
        /// </summary>
        /// <param name="objectOrGroup">The object or group being tested.</param>
        /// <param name="inScreenSpace">Whether to take scroll factors into account when checking for overlap.  Default is false, or "only compare in world space."</param>
        /// <param name="camera">Specify which game camera you want. If null getScreenXY() will just grab the first global camera. flx# - currently only one exists</param>
        /// <returns></returns>
        public virtual bool overlaps(FlxBasic objectOrGroup, bool inScreenSpace = false, FlxCamera camera = null)
		{
			if(objectOrGroup is FlxGroup)
			{
				bool results = false;
			    var group = objectOrGroup as FlxGroup;
			    foreach (FlxBasic member in group.Members)
			    {
			        if (overlaps(member, inScreenSpace, camera))
			        {
			            results = true;
                        // flx# - we could break here, if overlaps does not trigger anything on the remaining members
			        }
			    }
                
                /*
				int i = 0;
                List<FlxBasic> members = new List<FlxBasic>();
                members = (objectOrGroup as FlxGroup).members;
                //uint l = (uint)(ObjectOrGroup as FlxGroup).length;
                uint length = (uint)members.Count;
				while(i < length)
				{
					if(overlaps(members[i++],inScreenSpace,camera))
						results = true;
				}
                */

				return results;
			}

            if (objectOrGroup is FlxTilemap)
            {
                // Since tilemap's have to be the caller, not the target, to do proper tile-based collisions,
                // we redirect the call to the tilemap overlap here.
                return (objectOrGroup as FlxTilemap).overlaps(this, inScreenSpace, camera);
            }

			var flxObject = objectOrGroup as FlxObject;
			
            if(!inScreenSpace)
			{
                return (flxObject.X + flxObject.Width > X) && (flxObject.X < X + Width) &&
                       (flxObject.Y + flxObject.Height > Y) && (flxObject.Y < Y + Height);
			}

			if (camera == null)
			{
                camera = FlxG.camera;			    
			}

            FlxPoint objectScreenPos = flxObject.getScreenXY(null, camera);
			getScreenXY(_tagPoint, camera);
            return (objectScreenPos.X + flxObject.Width > _tagPoint.X) && (objectScreenPos.X < _tagPoint.X + Width) &&
                   (objectScreenPos.Y + flxObject.Height > _tagPoint.Y) && (objectScreenPos.Y < _tagPoint.Y + Height);
		}

        /// <summary>
        /// Checks to see if this <code>FlxObject</code> were located at the given position, would it overlap the <code>FlxObject</code> or <code>FlxGroup</code>?
        /// This is distinct from overlapsPoint(), which just checks that point, rather than taking the object's size into account.
        /// WARNING: Currently tilemaps do NOT support screen space overlap checks!
        /// </summary>
        /// <param name="x">The X position you want to check. Pretends this object (the caller, not the parameter) is located here.</param>
        /// <param name="y">The Y position you want to check. Pretends this object (the caller, not the parameter) is located here.</param>
        /// <param name="objectOrGroup">The object or group being tested.</param>
        /// <param name="inScreenSpace">Whether to take scroll factors into account when checking for overlap.  Default is false, or "only compare in world space."</param>
        /// <param name="camera">Specify which game camera you want.  If null getScreenXY() will just grab the first global camera. flx# - currently only one exists</param>
        /// <returns></returns>
        public virtual bool overlapsAt(float x, float y, FlxBasic objectOrGroup, bool inScreenSpace = false, FlxCamera camera = null)
        {
            if (objectOrGroup is FlxGroup)
            {
                bool results = false;
                var group = objectOrGroup as FlxGroup;
                foreach (FlxBasic member in group.Members)
                {
                    if (overlapsAt(x, y, member, inScreenSpace, camera))
                    {
                        results = true;
                        // flx# - we could break here, if overlaps does not trigger anything on the remaining members
                    }
                }

                /*
                int i = 0;List<FlxBasic> members = new List<FlxBasic>();
                members = (objectOrGroup as FlxGroup).members;
                uint length = (uint)members.Count;
                while (i < length)
                {
                        if (overlapsAt(x, y, members[i++], inScreenSpace, camera))
                            results = true;
                }
                */
                return results;
            }

            if (objectOrGroup is FlxTilemap)
            {
                var tilemap = objectOrGroup as FlxTilemap;
                return tilemap.overlapsAt(tilemap.X - (x - this.X), tilemap.Y - (y - this.Y), this, inScreenSpace, camera);
            }

            var flxObject = objectOrGroup as FlxObject;
            
            if(!inScreenSpace)
            {
                return (flxObject.X + flxObject.Width > x) && (flxObject.X < x + Width) &&
                       (flxObject.Y + flxObject.Height > y) && (flxObject.Y < y + Height);
            }

            if (camera == null)
            {
                camera = FlxG.camera;                
            }

            FlxPoint objectScreenPos = flxObject.getScreenXY(null, camera);
            _tagPoint.X = x - camera.Scroll.X * ScrollFactor.X; //copied from getScreenXY()
            _tagPoint.Y = y - camera.Scroll.Y * ScrollFactor.Y;
            _tagPoint.X += (_tagPoint.X > 0) ? 0.0000001f : -0.0000001f;
            _tagPoint.Y += (_tagPoint.Y > 0) ? 0.0000001f : -0.0000001f;

            return (objectScreenPos.X + flxObject.Width > _tagPoint.X) && (objectScreenPos.X < _tagPoint.X + Width) &&
                   (objectScreenPos.Y + flxObject.Height > _tagPoint.Y) && (objectScreenPos.Y < _tagPoint.Y + Height);
        }

        /// <summary>
        /// Checks to see if a point in 2D world space overlaps this <code>FlxObject</code> object.
        /// </summary>
        /// <param name="point">The point in world space you want to check.</param>
        /// <param name="inScreenSpace">Whether to take scroll factors into account when checking for overlap.</param>
        /// <param name="camera">Specify which game camera you want.  If null getScreenXY() will just grab the first global camera. flx# - currently only one exists</param>
        /// <returns></returns>
        public virtual bool overlapsPoint(FlxPoint point, bool inScreenSpace = false, FlxCamera camera = null)
        {
            if (!inScreenSpace)
            {
                return (point.X > this.X) && (point.X < this.X + Width) && (point.Y > this.Y) && (point.Y < this.Y + Height);                
            }

            if (camera == null)
            {
                camera = FlxG.camera;                
            }

            float x = point.X - camera.Scroll.X;
            float y = point.Y - camera.Scroll.Y;
            getScreenXY(_tagPoint, camera);
            return (x > _tagPoint.X) && (x < _tagPoint.X + Width) && (y > _tagPoint.Y) && (y < _tagPoint.Y + Height);
        }

        /// <summary>
        /// Check and see if this object is currently on screen.
        /// </summary>
        /// <param name="camera">Specify which game camera you want. If null getScreenXY() will just grab the first global camera.</param>
        /// <returns>bool</returns>
        public virtual bool onScreen(FlxCamera camera = null)
		{
			if (camera == null)
			{
                camera = FlxG.camera;			    
			}

			getScreenXY(_tagPoint,camera);
			return (_tagPoint.X + Width > 0) && (_tagPoint.X < camera.Width) && (_tagPoint.Y + Height > 0) && (_tagPoint.Y < camera.Height);
		}

        /// <summary>
        /// Call this function to figure out the on-screen position of the object.
        /// </summary>
        /// <param name="point">Takes a <code>FlxPoint</code> object and assigns the post-scrolled X and Y values of this object to it.</param>
        /// <param name="camera">Specify which game camera you want.  If null getScreenXY() will just grab the first global camera.</param>
        /// <returns></returns>
        public FlxPoint getScreenXY(FlxPoint point = null, FlxCamera camera = null)
		{
			if (point == null)
			{
                point = new FlxPoint();			    
			}

			if (camera == null)
			{
                camera = FlxG.camera;			    
			}

			point.X = X - (camera.Scroll.X * ScrollFactor.X);
			point.Y = Y - (camera.Scroll.Y * ScrollFactor.Y);
			point.X += (point.X > 0) ? 0.0000001f : -0.0000001f;
			point.Y += (point.Y > 0) ? 0.0000001f : -0.0000001f;
			return point;
		}

        /// <summary>
        /// Tells this object to flicker, retro-style.
        /// Pass a negative value to flicker forever.
        /// </summary>
        /// <param name="duration">How many seconds to flicker for.</param>
        public void flicker(float duration = 1.0f)
		{
			_flickerTimer = duration;

			if (_flickerTimer == 0)
			{
                _flicker = false;			    
			}
		}

        /// <summary>
        /// Check to see if the object is still flickering.
        /// </summary>
        public bool flickering
        {
            get { return _flickerTimer != 0; }
        }

        /// <summary>
        /// Whether the object collides or not.  For more control over what directions
        /// the object will collide from, use collision constants (like LEFT, FLOOR, etc)
        /// to set the value of allowCollisions directly.
        /// </summary>
        public bool Solid
        {
            get { return (AllowCollisions & Any) > None; }
            set { AllowCollisions = (value) ? Any : None; }
        }

        /// <summary>
        /// Retrieve the midpoint of this object in world coordinates.
        /// </summary>
        /// <param name="point">Allows you to pass in an existing <code>FlxPoint</code> object if you're so inclined. Otherwise a new one is created.</param>
        /// <returns>A <code>FlxPoint</code> object containing the midpoint of this object in world coordinates.</returns>
        public FlxPoint getMidpoint(FlxPoint point = null)
		{
			if (point == null)
			{
                point = new FlxPoint();			    
			}

            // flx# - wtf we dont bitshift here but anywhere else?
			point.X = X + (Width / 2);
			point.Y = Y + (Height / 2);
			return point;
		}

        /// <summary>
        /// Handy function for reviving game objects.
        /// Resets their existence flags and position.
        /// </summary>
        /// <param name="x">The new X position of this object.</param>
        /// <param name="y">The new Y position of this object.</param>
        public virtual void reset(float x, float y)
		{
			revive();
			Touching = None;
			WasTouching = None;
			X = x;
			Y = y;
			Last.X = X;
			Last.Y = Y;
			Velocity.X = 0;
			Velocity.Y = 0;
		}

        /// <summary>
        /// Handy function for checking if this object is touching a particular surface.
        /// For slightly better performance you can just & the value directly into <code>touching</code>.
        /// However, this method is good for readability and accessibility.
        /// </summary>
        /// <param name="direction">Any of the collision flags (e.g. LEFT, FLOOR, etc).</param>
        /// <returns>Whether the object is touching an object in (any of) the specified direction(s) this frame.</returns>
        public bool isTouching(uint direction)
		{
			return (Touching & direction) > None;
		}

        /// <summary>
        /// Handy function for checking if this object is just landed on a particular surface.
        /// </summary>
        /// <param name="direction">Any of the collision flags (e.g. LEFT, FLOOR, etc).</param>
        /// <returns>Whether the object just landed on (any of) the specified surface(s) this frame.</returns>
        public bool justTouched(uint direction)
		{
			return ((Touching & direction) > None) && ((WasTouching & direction) <= None);
		}

        /// <summary>
        /// Reduces the "health" variable of this sprite by the amount specified in Damage.
        /// Calls kill() if health drops to or below zero.
        /// </summary>
        /// <param name="damage">How much health to take away (use a negative number to give a health bonus).</param>
        public void hurt(float damage)
		{
			Health = Health - damage;

			if (Health <= 0)
			{
                kill();			    
			}
		}

        /// <summary>
        /// The main collision resolution function in flixel.
        /// </summary>
        /// <param name="objectOne">Any <code>FlxObject</code>.</param>
        /// <param name="objectTwo">Any other <code>FlxObject</code>.</param>
        /// <returns>Whether the objects in fact touched and were separated.</returns>
        static public bool separate(FlxObject objectOne, FlxObject objectTwo)
		{
			bool separatedX = separateX(objectOne, objectTwo);
			bool separatedY = separateY(objectOne, objectTwo);
			return separatedX || separatedY;
		}

        /// <summary>
        /// The X-axis component of the object separation process.
        /// </summary>
        /// <param name="objectOne">Any <code>FlxObject</code>.</param>
        /// <param name="objectTwo">Any other <code>FlxObject</code>.</param>
        /// <returns></returns>
        static public bool separateX(FlxObject objectOne, FlxObject objectTwo)
		{
			// can't separate two immovable objects
			bool obj1immovable = objectOne.Immovable;
			bool obj2immovable = objectTwo.Immovable;
			if(obj1immovable && obj2immovable)
				return false;

			// If one of the objects is a tilemap, just pass it off.
            if (objectOne is FlxTilemap)
                return (objectOne as FlxTilemap).overlapsWithCallback(objectTwo, separateX);
            if (objectTwo is FlxTilemap)
                return (objectTwo as FlxTilemap).overlapsWithCallback(objectOne, separateX, true);

			// First, get the two object deltas
            float overlap = 0;
            float obj1delta = objectOne.X - objectOne.Last.X;
            float obj2delta = objectTwo.X - objectTwo.Last.X;
			if(obj1delta != obj2delta)
			{
				//Check if the X hulls actually overlap
                float obj1deltaAbs = (obj1delta > 0) ? obj1delta : -obj1delta;
                float obj2deltaAbs = (obj2delta > 0) ? obj2delta : -obj2delta;
				FlxRect obj1rect = new FlxRect(objectOne.X-((obj1delta > 0)?obj1delta:0),objectOne.Last.Y,objectOne.Width+((obj1delta > 0)?obj1delta:-obj1delta),objectOne.Height);
				FlxRect obj2rect = new FlxRect(objectTwo.X-((obj2delta > 0)?obj2delta:0),objectTwo.Last.Y,objectTwo.Width+((obj2delta > 0)?obj2delta:-obj2delta),objectTwo.Height);
				if((obj1rect.X + obj1rect.Width > obj2rect.X) && (obj1rect.X < obj2rect.X + obj2rect.Width) && (obj1rect.Y + obj1rect.Height > obj2rect.Y) && (obj1rect.Y < obj2rect.Y + obj2rect.Height))
				{
                    float maxOverlap = obj1deltaAbs + obj2deltaAbs + OverlapBias;

					//If they did overlap (and can), figure out by how much and flip the corresponding flags
					if(obj1delta > obj2delta)
					{
						overlap = objectOne.X + objectOne.Width - objectTwo.X;
						if((overlap > maxOverlap) || !Convert.ToBoolean(objectOne.AllowCollisions & Right) || !Convert.ToBoolean(objectTwo.AllowCollisions & Left))
							overlap = 0;
						else
						{
							objectOne.Touching |= Right;
							objectTwo.Touching |= Left;
						}
					}
					else if(obj1delta < obj2delta)
					{
						overlap = objectOne.X - objectTwo.Width - objectTwo.X;
						if((-overlap > maxOverlap) || !Convert.ToBoolean(objectOne.AllowCollisions & Left) || !Convert.ToBoolean(objectTwo.AllowCollisions & Right))
							overlap = 0;
						else
						{
							objectOne.Touching |= Left;
							objectTwo.Touching |= Right;
						}
					}
				}
			}


			//Then adjust their positions and velocities accordingly (if there was any overlap)
			if(overlap != 0)
			{
                float obj1v = objectOne.Velocity.X;
                float obj2v = objectTwo.Velocity.X;

				if(!obj1immovable && !obj2immovable)
				{
					overlap *= 0.5f;
					objectOne.X = objectOne.X - overlap;
					objectTwo.X += overlap;

                    float obj1velocity = (float)Math.Sqrt((obj2v * obj2v * objectTwo.Mass) / objectOne.Mass) * ((obj2v > 0) ? 1f : -1f);
                    float obj2velocity = (float)Math.Sqrt((obj1v * obj1v * objectOne.Mass) / objectTwo.Mass) * ((obj1v > 0) ? 1f : -1f);
                    float average = (obj1velocity + obj2velocity) * 0.5f;
					obj1velocity -= average;
					obj2velocity -= average;
					objectOne.Velocity.X = average + obj1velocity * objectOne.Elasticity;
					objectTwo.Velocity.X = average + obj2velocity * objectTwo.Elasticity;
				}
				else if(!obj1immovable)
				{
					objectOne.X = objectOne.X - overlap;
					objectOne.Velocity.X = obj2v - obj1v*objectOne.Elasticity;
				}
				else if(!obj2immovable)
				{
					objectTwo.X += overlap;
					objectTwo.Velocity.X = obj1v - obj2v*objectTwo.Elasticity;
				}
				return true;
			}
			else
				return false;
		}

        /// <summary>
        /// The Y-axis component of the object separation process.
        /// </summary>
        /// <param name="objectOne">Any <code>FlxObject</code>.</param>
        /// <param name="objectTwo">Any other <code>FlxObject</code>.</param>
        /// <returns></returns>
        static public bool separateY(FlxObject objectOne, FlxObject objectTwo)
		{
			//can't separate two immovable objects
			bool obj1immovable = objectOne.Immovable;
			bool obj2immovable = objectTwo.Immovable;
			if(obj1immovable && obj2immovable)
				return false;

            ////If one of the objects is a tilemap, just pass it off.
            if (objectOne is FlxTilemap)
                return (objectOne as FlxTilemap).overlapsWithCallback(objectTwo, separateY);
            if (objectTwo is FlxTilemap)
                return (objectTwo as FlxTilemap).overlapsWithCallback(objectOne, separateY, true);

			//First, get the two object deltas
            float overlap = 0;
            float obj1delta = objectOne.Y - objectOne.Last.Y;
            float obj2delta = objectTwo.Y - objectTwo.Last.Y;
			if(obj1delta != obj2delta)
			{
				//Check if the Y hulls actually overlap
                float obj1deltaAbs = (obj1delta > 0) ? obj1delta : -obj1delta;
                float obj2deltaAbs = (obj2delta > 0) ? obj2delta : -obj2delta;
				FlxRect obj1rect = new FlxRect(objectOne.X,objectOne.Y-((obj1delta > 0)?obj1delta:0),objectOne.Width,objectOne.Height+obj1deltaAbs);
				FlxRect obj2rect = new FlxRect(objectTwo.X,objectTwo.Y-((obj2delta > 0)?obj2delta:0),objectTwo.Width,objectTwo.Height+obj2deltaAbs);
				if((obj1rect.X + obj1rect.Width > obj2rect.X) && (obj1rect.X < obj2rect.X + obj2rect.Width) && (obj1rect.Y + obj1rect.Height > obj2rect.Y) && (obj1rect.Y < obj2rect.Y + obj2rect.Height))
				{
                    float maxOverlap = obj1deltaAbs + obj2deltaAbs + OverlapBias;

					//If they did overlap (and can), figure out by how much and flip the corresponding flags
					if(obj1delta > obj2delta)
					{
						overlap = objectOne.Y + objectOne.Height - objectTwo.Y;
						if((overlap > maxOverlap) || !Convert.ToBoolean(objectOne.AllowCollisions & Down) || !Convert.ToBoolean(objectTwo.AllowCollisions & Up))
							overlap = 0;
						else
						{
							objectOne.Touching |= Down;
							objectTwo.Touching |= Up;
						}
					}
					else if(obj1delta < obj2delta)
					{
						overlap = objectOne.Y - objectTwo.Height - objectTwo.Y;
						if((-overlap > maxOverlap) || !Convert.ToBoolean(objectOne.AllowCollisions & Up) || !Convert.ToBoolean(objectTwo.AllowCollisions & Down))
							overlap = 0;
						else
						{
							objectOne.Touching |= Up;
							objectTwo.Touching |= Down;
						}
					}
				}
			}

			//Then adjust their positions and velocities accordingly (if there was any overlap)
			if(overlap != 0)
			{
                float obj1v = objectOne.Velocity.Y;
                float obj2v = objectTwo.Velocity.Y;

				if(!obj1immovable && !obj2immovable)
				{
					overlap *= 0.5f;
					objectOne.Y = objectOne.Y - overlap;
					objectTwo.Y += overlap;

                    float obj1velocity = (float)Math.Sqrt((obj2v * obj2v * objectTwo.Mass) / objectOne.Mass) * ((obj2v > 0) ? 1f : -1f);
                    float obj2velocity = (float)Math.Sqrt((obj1v * obj1v * objectOne.Mass) / objectTwo.Mass) * ((obj1v > 0) ? 1f : -1f);
                    float average = (obj1velocity + obj2velocity) * 0.5f;
					obj1velocity -= average;
					obj2velocity -= average;
					objectOne.Velocity.Y = average + obj1velocity *  objectOne.Elasticity;
					objectTwo.Velocity.Y = average + obj2velocity *  objectTwo.Elasticity;
				}
				else if(!obj1immovable)
				{
					objectOne.Y = objectOne.Y - overlap;
					objectOne.Velocity.Y = obj2v - obj1v*objectOne.Elasticity;
					//This is special case code that handles cases like horizontal moving platforms you can ride
					if(objectTwo.Active && objectTwo.Moves && (obj1delta > obj2delta))
						objectOne.X += objectTwo.X - objectTwo.Last.X;
				}
				else if(!obj2immovable)
				{
					objectTwo.Y += overlap;
					objectTwo.Velocity.Y = obj1v - obj2v*objectTwo.Elasticity;
					//This is special case code that handles cases like horizontal moving platforms you can ride
					if(objectOne.Active && objectOne.Moves && (obj1delta < obj2delta))
						objectTwo.X += objectOne.X - objectOne.Last.X;
				}
				return true;
			}
			else
				return false;
		}

        /// <summary>
        /// flx# - ? method not present in flixel
        /// </summary>
        public override void update()
        {
            base.update();
            Position.make(X, Y);
        }
    }
}
