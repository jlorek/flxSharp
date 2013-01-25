using System;
using System.Collections;
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
    public class FlxObject : FlxBasic
    {
        /// <summary>
        /// Generic value for "left" used by <code>facing</code>, <code>allowCollisions</code>, and <code>touching</code>
        /// </summary>
        public const uint LEFT = 0x0001;

        /// <summary>
        /// Generic value for "right" used by <code>facing</code>, <code>allowCollisions</code>, and <code>touching</code>
        /// </summary>
        public const uint RIGHT = 0x0010;

        /// <summary>
        /// Generic value for "up" used by <code>facing</code>, <code>allowCollisions</code>, and <code>touching</code>
        /// </summary>
        public const uint UP = 0x0100;

        /// <summary>
        /// Generic value for "down" used by <code>facing</code>, <code>allowCollisions</code>, and <code>touching</code>
        /// </summary>
        public const uint DOWN = 0x1000;

        /// <summary>
        /// Special-case constant meaning no collisions, used mainly by <code>allowCollisions</code>, and <code>touching</code>
        /// </summary>
        public const uint NONE = 0;

        /// <summary>
        /// Special-case constant meaning up, used mainly by <code>allowCollisions</code>, and <code>touching</code>
        /// </summary>
        public const uint CEILING = UP;

        /// <summary>
        /// Special-case constant meaning down, used mainly by <code>allowCollisions</code>, and <code>touching</code>
        /// </summary>
        public const uint FLOOR = DOWN;

        /// <summary>
        /// Special-case constant meaning only the left and right sides, used mainly by <code>allowCollisions</code>, and <code>touching</code>
        /// </summary>
        public const uint WALL = LEFT | RIGHT;

        /// <summary>
        /// Special-case constant meaning any direction, used mainly by <code>allowCollisions</code>, and <code>touching</code>
        /// </summary>
        public const uint ANY = LEFT | RIGHT | UP | DOWN;

        /// <summary>
        /// Handy constant used during collision resolution (see <code>separateX()</code> and <code>separateY()</code>)
        /// </summary>
        public const float OVERLAP_BIAS = 4;

        /// <summary>
        /// Path behavior controls: move from start of the path to the end then stop.
        /// </summary>
        public const uint PATH_FORWARD = 0x000000;

        /// <summary>
        /// Path behavior controls: move from end of the path to the start then stop.
        /// </summary>
        public const uint PATH_BACKWARD = 0x000001;

        /// <summary>
        /// Path behavior controls: move from start of the path to the end then directly back to start, and start over
        /// </summary>
        public const uint PATH_LOOP_FORWARD = 0x000010;

        /// <summary>
        /// Path behavior controls: move from end of the path to the start then directly back to end, and start over
        /// </summary>
        public const uint PATH_LOOP_BACKWARD = 0x000100;

        /// <summary>
        /// Path behavior controls: move from start of the path to the end then directly back to start, over and over
        /// </summary>
        public const uint PATH_YOYO = 0x001000;

        /// <summary>
        /// Path behavior controls: ignores any vertical component to the path data, only follows side to side
        /// </summary>
        public const uint PATH_HORIZONTAL_ONLY = 0x010000;

        /// <summary>
        /// Path behavior controls: ignores any horizontal component to the path data, only follows up and down
        /// </summary>
        public const uint PATH_VERTICAL_ONLY = 0x100000;

        /// <summary>
        /// X position of the upper left corner of this object in world space
        /// </summary>
        public float x;

        /// <summary>
        /// Y position of the upper left corner of this object in world space
        /// </summary>
        public float y;

        /// <summary>
        /// The width of this object
        /// </summary>
        public float width;

        /// <summary>
        /// The height of this object
        /// </summary>
        public float height;

        /// <summary>
        /// Whether this object will move/alter position after a collision
        /// </summary>
        public bool immovable;

        /// <summary>
        /// The bounciness of this object, Only affects collisions.  Default is 0, or "not bouncy at all."
        /// </summary>
        public float elasticity;

        /// <summary>
        /// Virtual mass of this object.  Defauly value is 1.  Currently only used with <code>elasticity</code> during collision resolution.
        /// Change at your own risk; effects seem crazy unpredictable so far!
        /// </summary>
        public float mass;

        /// <summary>
        /// Basic speed of this object
        /// </summary>
        public FlxPoint velocity;

        /// <summary>
        /// Max speed of this object
        /// </summary>
        public FlxPoint maxVelocity;

        /// <summary>
        /// How fast the speed of this object is changing.
        /// Useful for smooth movement and gravity
        /// </summary>
        public FlxPoint acceleration;

        /// <summary>
        /// This is like deceleration that is only applied when acceleration is not affecting the sprite.
        /// </summary>
        public FlxPoint drag;

        /// <summary>
        /// The angle of the sprite, used for rotation
        /// </summary>
        public float angle;

        /// <summary>
        /// How fast you want the sprite to spin
        /// </summary>
        public float angularVelocity;

        /// <summary>
        /// How fast the spin should change
        /// </summary>
        public float angularAcceleration;

        /// <summary>
        /// Like <code>drag</code> but for spinning
        /// </summary>
        public float angularDrag;

        /// <summary>
        /// Use in conjunction with <code>angularAcceleration</code> fir fluid spin speed control
        /// </summary>
        public float maxAngular;

        /// <summary>
        /// Should always represent (0,0) - useful for different things, for avoideing unnecessary <code>new</code> calls
        /// </summary>
        protected static FlxPoint _pZero = new FlxPoint();

        /// <summary>
        /// How much this object is affected by the camera subsystem.
        /// 0 means it never moves, like a HUD element or background graphic.
        /// 1 means it scrolls along at the same speed as the foreground layer.
        /// (1,1) by default.
        /// 
        /// CURRENTLY NOT IMPLEMENTED
        /// </summary>
        public FlxPoint scrollFactor;

        /// <summary>
        /// Internal helper for Retro-styled flickering
        /// </summary>
        protected bool _flicker;

        /// <summary>
        /// Internal helper for Retro-styled flickering
        /// </summary>
        protected float _flickerTimer;

        /// <summary>
        /// Handy for storing health percentage or armor points or whatever
        /// </summary>
        public float health;

        /// <summary>
        /// Pre-allocated x-y point container to be used however you like
        /// </summary>
        protected FlxPoint _point;

        /// <summary>
        /// Pre-allocated rectangle container to be used however you like
        /// </summary>
        protected FlxRect _rect;

        /// <summary>
        /// Set this to false if you want to skip automatic motion/movement stuff.
        /// Default is true for FlxObject and FlxSprite.
        /// Default is false for FlxText, FlxTileblock, FlxTilemap, and FlxSound.
        /// </summary>
        public bool moves;

        /// <summary>
        /// Bit field of flags (use with UP, DOWN, LEFT, RIGHT, etc) indicating surface contacts.
        /// Use bitwise operators to check the values stored here, or use touching(), justStartedTouching(), etc.
        /// You can even use them broadly as boolean values if you're feeling saucy!
        /// </summary>
        public uint touching;

        /// <summary>
        /// Bit field of flags (use with UP, DOWN, LEFT, RIGHT, etc) indicating surface contacts from the previous game loop step.
        /// Use bitwise operators to check the values stored here, or use touching(), justStartedTouching(), etc.
        /// You can even use them broadly as boolean values if you're feeling saucy!
        /// </summary>
        public uint wasTouching;

        /// <summary>
        /// Bit field of flags (use with UP, DOWN, LEFT, RIGHT, etc) indicating collision directions.
        /// Use bitwise operators to check the values stored here.
        /// Useful for things like one-way platforms (e.g. allowCollisions = UP;)
        /// The accessor "solid" just flips this variable between NONE and ANY/
        /// </summary>
        public uint allowCollisions;

        /// <summary>
        /// Important variable for collision processing.
        /// Set automatically during <code>preUpdate()</code>
        /// </summary>
        public FlxPoint last;

        /// <summary>
        /// A reference to a path object.  Null by default, assigned by followPath()
        /// </summary>
        public FlxPath path;

        /// <summary>
        /// The speed at which the object is moving on the path.
        /// When an object completes a non-looping path circuit,
        /// the pathSpeed will be zeroed out, but the path reference
        /// will NOT be nulled out.  So pathSpeed is a good way
        /// to check if this object is currently following a path of not
        /// </summary>
        public float pathSpeed;

        /// <summary>
        /// The angle in degrees between this object and the next node, where 0 is directly North, 90 is East
        /// </summary>
        public float pathAngle;

        /// <summary>
        /// Internal helper, tracks which node of the path this object is moving toward
        /// </summary>
        protected int _pathNodeIndex;
        
        /// <summary>
        /// Internal tracker for path behavior flags (looping, horizontal only, etc)
        /// </summary>
        protected uint _pathMode;

        /// <summary>
        /// Internal helper for node navigation, specifically yoyo and backwards movement
        /// </summary>
        protected int _pathInc;

        /// <summary>
        /// Internal flag for whether the object's angle should be adjusted to the path angle during path follow behavior
        /// </summary>
        protected bool _pathRotate;

        /// <summary>
        /// Instantiates a <code>FlxObject</code>
        /// </summary>
        /// <param name="X">X-coordinate of the object in space</param>
        /// <param name="Y">y-coordinate of the object in space</param>
        /// <param name="Width">Desired width of the rectangle</param>
        /// <param name="Height">Desired height of the rectangle</param>
        public FlxObject(float X = 0, float Y = 0, float Width = 0, float Height = 0)
            : base()
        {
            x = X;
            y = Y;
            last = new FlxPoint(x, y);
            width = Width;
            height = Height;
            mass = 1.0f;
            elasticity = 0.0f;

            health = 1;

            immovable = false;
            moves = true;

            touching = NONE;
            wasTouching = NONE;
            allowCollisions = ANY;

            velocity = new FlxPoint();
            acceleration = new FlxPoint();
            drag = new FlxPoint();
            maxVelocity = new FlxPoint(10000, 10000);

            angle = 0;
            angularVelocity = 0;
            angularAcceleration = 0;
            angularDrag = 0;
            maxAngular = 10000;

            scrollFactor = new FlxPoint(1, 1);
            _flicker = false;
            _flickerTimer = 0;

            _point = new FlxPoint();
            _rect = new FlxRect();

            path = null;
            pathSpeed = 0;
            pathAngle = 0;
        }

        /// <summary>
        /// Override to null out variables manually
        /// </summary>
        public override void destroy()
		{
            velocity = null;
            acceleration = null;
            drag = null;
            maxVelocity = null;
            scrollFactor = null;
            _point = null;
            _rect = null;
            last = null;
            //cameras = null;
            if (path != null)
                path.destroy();
            path = null;
		}
        
        /// <summary>
        /// Called right before update()
        /// </summary>
        public override void preUpdate()
        {
            _ACTIVECOUNT++;

            if (_flickerTimer != 0)
            {
                if (_flickerTimer > 0)
                {
                    _flickerTimer = _flickerTimer - FlxG.elapsed;
                    if (_flickerTimer <= 0)
                    {
                        _flickerTimer = 0;
                        _flicker = false;
                        reset(x, y);
                    }
                }
            }

            last.x = x;
            last.y = y;

            if ((path != null) && (pathSpeed != 0) && (path.nodes[_pathNodeIndex] != null))
                updatePathMotion();
        }

        public override void update()
        {
            base.update();
            position.make(x, y);
        }

        /// <summary>
        /// Called right after update()
        /// </summary>
        public override void postUpdate()
        {
            if (moves)
                updateMotion();

            wasTouching = touching;
            touching = NONE;
        }

        /// <summary>
        /// Internal function for updating the position and speed of this object.
        /// </summary>
        private void updateMotion()
        {
            float delta;
            float velocityDelta;

            velocityDelta = (FlxU.computeVelocity(angularVelocity, angularAcceleration, angularDrag, maxAngular) - angularVelocity) / 2;
            angularVelocity += velocityDelta;
            angle += angularVelocity * FlxG.elapsed;
            angularVelocity += velocityDelta;

            velocityDelta = (FlxU.computeVelocity(velocity.x, acceleration.x, drag.x, maxVelocity.x) - velocity.x) / 2;
            velocity.x += velocityDelta;
            delta = velocity.x * FlxG.elapsed;
            velocity.x += velocityDelta;
            x += delta;

            velocityDelta = (FlxU.computeVelocity(velocity.y, acceleration.y, drag.y, maxVelocity.y) - velocity.y) / 2;
            velocity.y += velocityDelta;
            delta = velocity.y * FlxG.elapsed;
            velocity.y += velocityDelta;
            y += delta;
        }

        /// <summary>
        /// Rarely called.
        /// Not yet implemented like AS3 Flixel
        /// </summary>
        public override void draw()
        {
            if (cameras == null)
                cameras = FlxG.cameras;
            FlxCamera camera;// = FlxG.camera;
            int i = 0;
            int l = cameras.Count;
            while (i < l)
            {
                camera = cameras[i++];
                //camera = FlxG.camera;
                if (!onScreen(camera))
                    continue;
                _VISIBLECOUNT++;
                if (FlxG.visualDebug && !ignoreDrawDebug)
                    drawDebug(camera);
            }
        }

        /// <summary>
        /// Not yet implemented
        /// </summary>
        /// <param name="Camera">Which Camera - currently only one exists</param>
        public override void drawDebug(FlxCamera Camera=null)
		{
			if(Camera == null)
				Camera = FlxG.camera;

			//get bounding box coordinates
			float boundingBoxX = x - (int)(Camera.scroll.x*scrollFactor.x); //copied from getScreenXY()
			float boundingBoxY = y - (int)(Camera.scroll.y*scrollFactor.y);
			boundingBoxX = (int)(boundingBoxX + ((boundingBoxX > 0)?0.0000001f:-0.0000001f));
			boundingBoxY = (int)(boundingBoxY + ((boundingBoxY > 0)?0.0000001f:-0.0000001f));
            int boundingBoxWidth = (int)((width != (int)(width)) ? width : width - 1f);
            int boundingBoxHeight = (int)((height != (int)(height)) ?height : height - 1f);

            ////fill static graphics object with square shape
            //var gfx:Graphics = FlxG.flashGfx;
            //gfx.clear();
            //gfx.moveTo(boundingBoxX,boundingBoxY);
			Color boundingBoxColor;
			if(Convert.ToBoolean(allowCollisions))
			{
				if(allowCollisions != ANY)
                    boundingBoxColor = FlxColor.PINK;
				if(immovable)
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

        /**
		 * Call this function to give this object a path to follow.
		 * If the path does not have at least one node in it, this function
		 * will log a warning message and return.
		 * 
		 * @param	Path		The <code>FlxPath</code> you want this object to follow.
		 * @param	Speed		How fast to travel along the path in pixels per second.
		 * @param	Mode		Optional, controls the behavior of the object following the path using the path behavior constants.  Can use multiple flags at once, for example PATH_YOYO|PATH_HORIZONTAL_ONLY will make an object move back and forth along the X axis of the path only.
		 * @param	AutoRotate	Automatically point the object toward the next node.  Assumes the graphic is pointing upward.  Default behavior is false, or no automatic rotation.
		 */
        public void followPath(FlxPath Path, float Speed = 100, uint Mode = PATH_FORWARD, bool AutoRotate = false)
        {
            if (Path.nodes.Count <= 0)
            {
                FlxG.log("WARNING: Paths need at least one node in them to be followed.");
                return;
            }

            path = Path;
            pathSpeed = FlxU.abs(Speed);
            _pathMode = Mode;
            _pathRotate = AutoRotate;

            //get starting node
            if ((_pathMode == PATH_BACKWARD) || (_pathMode == PATH_LOOP_BACKWARD))
            {
                _pathNodeIndex = path.nodes.Count - 1;
                _pathInc = -1;
            }
            else
            {
                _pathNodeIndex = 0;
                _pathInc = 1;
            }
        }

        /**
		 * Tells this object to stop following the path its on.
		 * 
		 * @param	DestroyPath		Tells this function whether to call destroy on the path object.  Default value is false.
		 */
		public void stopFollowingPath(bool DestroyPath=false)
		{
			pathSpeed = 0;
			if(DestroyPath && (path != null))
			{
				path.destroy();
				path = null;
			}
		}

        /**
		 * Internal function that decides what node in the path to aim for next based on the behavior flags.
		 * 
		 * @return	The node (a <code>FlxPoint</code> object) we are aiming for next.
		 */
		protected FlxPoint advancePath(bool Snap=true)
		{
			if(Snap)
			{
				FlxPoint oldNode = path.nodes[_pathNodeIndex];
				if(oldNode != null)
				{
					if((_pathMode & PATH_VERTICAL_ONLY) == 0)
						x = oldNode.x - width*0.5f;
					if((_pathMode & PATH_HORIZONTAL_ONLY) == 0)
						y = oldNode.y - height*0.5f;
				}
			}

			_pathNodeIndex += _pathInc;

			if((_pathMode & PATH_BACKWARD) > 0)
			{
				if(_pathNodeIndex < 0)
				{
					_pathNodeIndex = 0;
					pathSpeed = 0;
				}
			}
			else if((_pathMode & PATH_LOOP_FORWARD) > 0)
			{
				if(_pathNodeIndex >= path.nodes.Count)
					_pathNodeIndex = 0;
			}
			else if((_pathMode & PATH_LOOP_BACKWARD) > 0)
			{
				if(_pathNodeIndex < 0)
				{
					_pathNodeIndex = path.nodes.Count-1;
					if(_pathNodeIndex < 0)
						_pathNodeIndex = 0;
				}
			}
			else if((_pathMode & PATH_YOYO) > 0)
			{
				if(_pathInc > 0)
				{
                    if (_pathNodeIndex >= path.nodes.Count)
					{
                        _pathNodeIndex = path.nodes.Count - 2;
						if(_pathNodeIndex < 0)
							_pathNodeIndex = 0;
						_pathInc = -_pathInc;
					}
				}
				else if(_pathNodeIndex < 0)
				{
					_pathNodeIndex = 1;
                    if (_pathNodeIndex >= path.nodes.Count)
                        _pathNodeIndex = path.nodes.Count - 1;
					if(_pathNodeIndex < 0)
						_pathNodeIndex = 0;
					_pathInc = -_pathInc;
				}
			}
			else
			{
                if (_pathNodeIndex >= path.nodes.Count)
				{
                    _pathNodeIndex = path.nodes.Count - 1;
					pathSpeed = 0;
				}
			}

			return path.nodes[_pathNodeIndex];
		}

        /**
		 * Internal function for moving the object along the path.
		 * Generally this function is called automatically by <code>preUpdate()</code>.
		 * The first half of the function decides if the object can advance to the next node in the path,
		 * while the second half handles actually picking a velocity toward the next node.
		 */
		protected void updatePathMotion()
		{
			//first check if we need to be pointing at the next node yet
			_point.x = x + width*0.5f;
			_point.y = y + height*0.5f;
			FlxPoint node = path.nodes[_pathNodeIndex];
			float deltaX = node.x - _point.x;
			float deltaY = node.y - _point.y;

			bool horizontalOnly = (_pathMode & PATH_HORIZONTAL_ONLY) > 0;
			bool verticalOnly = (_pathMode & PATH_VERTICAL_ONLY) > 0;

			if(horizontalOnly)
			{
				if(((deltaX>0)?deltaX:-deltaX) < pathSpeed*FlxG.elapsed)
					node = advancePath();
			}
			else if(verticalOnly)
			{
				if(((deltaY>0)?deltaY:-deltaY) < pathSpeed*FlxG.elapsed)
					node = advancePath();
			}
			else
			{
				if(Math.Sqrt(deltaX*deltaX + deltaY*deltaY) < pathSpeed*FlxG.elapsed)
					node = advancePath();
			}

			//then just move toward the current node at the requested speed
			if(pathSpeed != 0)
			{
				//set velocity based on path mode
				_point.x = x + width*0.5f;
				_point.y = y + height*0.5f;
				if(horizontalOnly || (_point.y == node.y))
				{
					velocity.x = (_point.x < node.x)?pathSpeed:-pathSpeed;
					if(velocity.x < 0)
						pathAngle = -90;
					else
						pathAngle = 90;
					if(!horizontalOnly)
						velocity.y = 0;
				}
				else if(verticalOnly || (_point.x == node.x))
				{
					velocity.y = (_point.y < node.y)?pathSpeed:-pathSpeed;
					if(velocity.y < 0)
						pathAngle = 0;
					else
						pathAngle = 180;
					if(!verticalOnly)
						velocity.x = 0;
				}
				else
				{
					pathAngle = FlxU.getAngle(_point,node);
					FlxU.rotatePoint(0,pathSpeed,0,0,pathAngle,velocity);
				}

				//then set object rotation if necessary
				if(_pathRotate)
				{
					angularVelocity = 0;
					angularAcceleration = 0;
					angle = pathAngle;
				}
			}			
		}


        /// <summary>
        /// Checks to see if some FlxObject overlaps this FlxObject or FlxGroup.
        /// If the group has a LOT of things in it, it might be faster to use <code>FlxG.ovelaps()</code>
        /// </summary>
        /// <param name="ObjectOrGroup">The object or group being tested</param>
        /// <param name="InScreenSpace">Whether to take scroll factors into account.</param>
        /// <param name="Camera">Which Camera - currently only one exists</param>
        /// <returns></returns>
        public virtual bool overlaps(FlxBasic ObjectOrGroup, bool InScreenSpace=false, FlxCamera Camera=null)
		{
			if(ObjectOrGroup is FlxGroup)
			{
				bool results = false;
				int i = 0;
                List<FlxBasic> members = new List<FlxBasic>();
                members = (ObjectOrGroup as FlxGroup).members;
                //uint l = (uint)(ObjectOrGroup as FlxGroup).length;
                uint length = (uint)members.Count;
				while(i < length)
				{
					if(overlaps(members[i++],InScreenSpace,Camera))
						results = true;
				}
				return results;
			}

            if (ObjectOrGroup is FlxTilemap)
            {
                //Since tilemap's have to be the caller, not the target, to do proper tile-based collisions,
                // we redirect the call to the tilemap overlap here.
                return (ObjectOrGroup as FlxTilemap).overlaps(this, InScreenSpace, Camera);
            }

			FlxObject Object = ObjectOrGroup as FlxObject;
			if(!InScreenSpace)
			{
                return (Object.x + Object.width > x) && (Object.x < x + width) &&
                        (Object.y + Object.height > y) && (Object.y < y + height);
			}

			if(Camera == null)
				Camera = FlxG.camera;
            FlxPoint objectScreenPos = Object.getScreenXY(null, Camera);
			getScreenXY(_point,Camera);
            return (objectScreenPos.x + Object.width > _point.x) && (objectScreenPos.x < _point.x + width) &&
                    (objectScreenPos.y + Object.height > _point.y) && (objectScreenPos.y < _point.y + height);
		}

        /// <summary>
        /// Checks to see if this FlxObject were located at the given position
        /// </summary>
        /// <param name="X">X position you want to check</param>
        /// <param name="Y">Y position you want to check</param>
        /// <param name="ObjectOrGroup">The object or group being tested</param>
        /// <param name="InScreenSpace">Whether to take scroll factors into account.</param>
        /// <param name="Camera">Which Camera - currently only one exists</param>
        /// <returns></returns>
        public virtual bool overlapsAt(float X, float Y, FlxBasic ObjectOrGroup, bool InScreenSpace = false, FlxCamera Camera = null)
        {
            if (ObjectOrGroup is FlxGroup)
            {
                bool results = false;
                int i = 0;List<FlxBasic> members = new List<FlxBasic>();
                members = (ObjectOrGroup as FlxGroup).members;
                uint length = (uint)members.Count;
                while (i < length)
                {
                        if (overlapsAt(X, Y, members[i++], InScreenSpace, Camera))
                            results = true;
                }
                return results;
            }

            if (ObjectOrGroup is FlxTilemap)
            {
                FlxTilemap tilemap = ObjectOrGroup as FlxTilemap;
                return tilemap.overlapsAt(tilemap.x - (X - x), tilemap.y - (Y - y), this, InScreenSpace, Camera);
            }

            FlxObject Object = ObjectOrGroup as FlxObject;
            if(!InScreenSpace)
            {
                return (Object.x + Object.width > X) && (Object.x < X + width) &&
                        (Object.y + Object.height > Y) && (Object.y < Y + height);
            }

            if (Camera == null)
                Camera = FlxG.camera;
            FlxPoint objectScreenPos = Object.getScreenXY(null, Camera);
            _point.x = X - Camera.scroll.x * scrollFactor.x;
            _point.y = Y - Camera.scroll.y * scrollFactor.y;
            _point.x += (_point.x > 0) ? 0.0000001f : -0.0000001f;
            _point.y += (_point.y > 0) ? 0.0000001f : -0.0000001f;

            return (objectScreenPos.x + Object.width > _point.x) && (objectScreenPos.x < _point.x + width) &&
                (objectScreenPos.y + Object.height > _point.y) && (objectScreenPos.y < _point.y + height);
        }

        /// <summary>
        /// Check to see if a point in 2D world space overlaps this FlxObject
        /// </summary>
        /// <param name="Point">The point in world space you want to check</param>
        /// <param name="InScreenSpace">Whether to take scroll factors into account.</param>
        /// <param name="Camera">Which Camera - currently only one exists</param>
        /// <returns></returns>
        public virtual bool overlapsPoint(FlxPoint Point, bool InScreenSpace = false, FlxCamera Camera = null)
        {
            if (!InScreenSpace)
                return (Point.x > x) && (Point.x < x + width) && (Point.y > y) && (Point.y < y + height);

            if (Camera == null)
                Camera = FlxG.camera;
            float X = Point.x - Camera.scroll.x;
            float Y = Point.y - Camera.scroll.y;
            getScreenXY(_point, Camera);
            return (X > _point.x) && (X < _point.x + width) && (Y > _point.y) && (Y < _point.y + height);
        }

        /// <summary>
        /// Check to see if this object is currently on the screen
        /// </summary>
        /// <param name="Camera">Which Camera - currently only one exists</param>
        /// <returns>bool</returns>
        public virtual bool onScreen(FlxCamera Camera=null)
		{
			if(Camera == null)
				Camera = FlxG.camera;
			getScreenXY(_point,Camera);
			return (_point.x + width > 0) && (_point.x < Camera.width) && (_point.y + height > 0) && (_point.y < Camera.height);
		}

        /// <summary>
        /// Call this to figure out the on-screen position of the object
        /// </summary>
        /// <param name="Point">Take a FlxPoint object and assign the post-scrolled X and Y values of this object to it</param>
        /// <param name="Camera">Which Camera - currently only one exists</param>
        /// <returns></returns>
        public FlxPoint getScreenXY(FlxPoint Point=null, FlxCamera Camera=null)
		{
			if(Point == null)
				Point = new FlxPoint();
			if(Camera == null)
				Camera = FlxG.camera;
			Point.x = x - (Camera.scroll.x*scrollFactor.x);
			Point.y = y - (Camera.scroll.y*scrollFactor.y);
			Point.x += (Point.x > 0)?0.0000001f:-0.0000001f;
			Point.y += (Point.y > 0)?0.0000001f:-0.0000001f;
			return Point;
		}

        /// <summary>
        /// Flicker this object
        /// </summary>
        /// <param name="Duration">How many seconds</param>
        public void flicker(float Duration=1.0f)
		{
			_flickerTimer = Duration;
			if(_flickerTimer == 0)
				_flicker = false;
		}

        /// <summary>
        /// Check to see if it is still flickering
        /// </summary>
        public bool flickering
        {
            get { return _flickerTimer != 0; }
        }

        /// <summary>
        /// Get/Set solid
        /// </summary>
        public bool solid
        {
            get { return (allowCollisions & ANY) > NONE; }
            set
            {
                if (value)
                    allowCollisions = ANY;
                else
                    allowCollisions = NONE;
            }

        }

        /// <summary>
        /// Retrieve midpoint of this object in world coordinates
        /// </summary>
        /// <param name="Point"></param>
        /// <returns></returns>
        public FlxPoint getMidpoint(FlxPoint Point=null)
		{
			if(Point == null)
				Point = new FlxPoint();
			Point.x = x + (width / 2);
			Point.y = y + (height / 2);
			return Point;
		}

        /// <summary>
        /// Handy function for reviving game objects.  
        /// Resets their existence flags and position
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        public virtual void reset(float X, float Y)
		{
			revive();
			touching = NONE;
			wasTouching = NONE;
			x = X;
			y = Y;
			last.x = x;
			last.y = y;
			velocity.x = 0;
			velocity.y = 0;
		}

        /// <summary>
        /// Check to see if this object is touching a particular surface
        /// </summary>
        /// <param name="Direction">Any of the collision flags (e.g. LEFT, FLOOR, etc)</param>
        /// <returns>true if touching is true</returns>
        public bool isTouching(uint Direction)
		{
			return (touching & Direction) > NONE;
		}

        /// <summary>
        /// Check to see if this object just touched a particular surface
        /// </summary>
        /// <param name="Direction">Any of the collision flags (e.g. LEFT, FLOOR, etc)</param>
        /// <returns>true if touching is true</returns>
        public bool justTouched(uint Direction)
		{
			return ((touching & Direction) > NONE) && ((wasTouching & Direction) <= NONE);
		}

        /// <summary>
        /// Reduces the health
        /// </summary>
        /// <param name="Damage">Amount to reduce by</param>
        public void hurt(float Damage)
		{
			health = health - Damage;
			if(health <= 0)
				kill();
		}

        /// <summary>
        /// The main collision resolution function
        /// </summary>
        /// <param name="Object1"></param>
        /// <param name="Object2"></param>
        /// <returns></returns>
        static public bool separate(FlxObject Object1, FlxObject Object2)
		{
			bool separatedX = separateX(Object1,Object2);
			bool separatedY = separateY(Object1,Object2);
			return separatedX || separatedY;
		}

        /// <summary>
        /// X-axis component of the object separation process
        /// </summary>
        /// <param name="Object1"></param>
        /// <param name="Object2"></param>
        /// <returns></returns>
        static public bool separateX(FlxObject Object1, FlxObject Object2)
		{

			//can't separate two immovable objects
			bool obj1immovable = Object1.immovable;
			bool obj2immovable = Object2.immovable;
			if(obj1immovable && obj2immovable)
				return false;

			//If one of the objects is a tilemap, just pass it off.
            if (Object1 is FlxTilemap)
                return (Object1 as FlxTilemap).overlapsWithCallback(Object2, separateX);
            if (Object2 is FlxTilemap)
                return (Object2 as FlxTilemap).overlapsWithCallback(Object1, separateX, true);

			//First, get the two object deltas
            float overlap = 0;
            float obj1delta = Object1.x - Object1.last.x;
            float obj2delta = Object2.x - Object2.last.x;
			if(obj1delta != obj2delta)
			{
				//Check if the X hulls actually overlap
                float obj1deltaAbs = (obj1delta > 0) ? obj1delta : -obj1delta;
                float obj2deltaAbs = (obj2delta > 0) ? obj2delta : -obj2delta;
				FlxRect obj1rect = new FlxRect(Object1.x-((obj1delta > 0)?obj1delta:0),Object1.last.y,Object1.width+((obj1delta > 0)?obj1delta:-obj1delta),Object1.height);
				FlxRect obj2rect = new FlxRect(Object2.x-((obj2delta > 0)?obj2delta:0),Object2.last.y,Object2.width+((obj2delta > 0)?obj2delta:-obj2delta),Object2.height);
				if((obj1rect.x + obj1rect.width > obj2rect.x) && (obj1rect.x < obj2rect.x + obj2rect.width) && (obj1rect.y + obj1rect.height > obj2rect.y) && (obj1rect.y < obj2rect.y + obj2rect.height))
				{
                    float maxOverlap = obj1deltaAbs + obj2deltaAbs + OVERLAP_BIAS;

					//If they did overlap (and can), figure out by how much and flip the corresponding flags
					if(obj1delta > obj2delta)
					{
						overlap = Object1.x + Object1.width - Object2.x;
						if((overlap > maxOverlap) || !Convert.ToBoolean(Object1.allowCollisions & RIGHT) || !Convert.ToBoolean(Object2.allowCollisions & LEFT))
							overlap = 0;
						else
						{
							Object1.touching |= RIGHT;
							Object2.touching |= LEFT;
						}
					}
					else if(obj1delta < obj2delta)
					{
						overlap = Object1.x - Object2.width - Object2.x;
						if((-overlap > maxOverlap) || !Convert.ToBoolean(Object1.allowCollisions & LEFT) || !Convert.ToBoolean(Object2.allowCollisions & RIGHT))
							overlap = 0;
						else
						{
							Object1.touching |= LEFT;
							Object2.touching |= RIGHT;
						}
					}
				}
			}


			//Then adjust their positions and velocities accordingly (if there was any overlap)
			if(overlap != 0)
			{
                float obj1v = Object1.velocity.x;
                float obj2v = Object2.velocity.x;

				if(!obj1immovable && !obj2immovable)
				{
					overlap *= 0.5f;
					Object1.x = Object1.x - overlap;
					Object2.x += overlap;

                    float obj1velocity = (float)Math.Sqrt((obj2v * obj2v * Object2.mass) / Object1.mass) * ((obj2v > 0) ? 1f : -1f);
                    float obj2velocity = (float)Math.Sqrt((obj1v * obj1v * Object1.mass) / Object2.mass) * ((obj1v > 0) ? 1f : -1f);
                    float average = (obj1velocity + obj2velocity) * 0.5f;
					obj1velocity -= average;
					obj2velocity -= average;
					Object1.velocity.x = average + obj1velocity * Object1.elasticity;
					Object2.velocity.x = average + obj2velocity * Object2.elasticity;
				}
				else if(!obj1immovable)
				{
					Object1.x = Object1.x - overlap;
					Object1.velocity.x = obj2v - obj1v*Object1.elasticity;
				}
				else if(!obj2immovable)
				{
					Object2.x += overlap;
					Object2.velocity.x = obj1v - obj2v*Object2.elasticity;
				}
				return true;
			}
			else
				return false;
		}

        /// <summary>
        /// Y-axis component of the object separation process
        /// </summary>
        /// <param name="Object1"></param>
        /// <param name="Object2"></param>
        /// <returns></returns>
        static public bool separateY(FlxObject Object1, FlxObject Object2)
		{
			//can't separate two immovable objects
			bool obj1immovable = Object1.immovable;
			bool obj2immovable = Object2.immovable;
			if(obj1immovable && obj2immovable)
				return false;

            ////If one of the objects is a tilemap, just pass it off.
            if (Object1 is FlxTilemap)
                return (Object1 as FlxTilemap).overlapsWithCallback(Object2, separateY);
            if (Object2 is FlxTilemap)
                return (Object2 as FlxTilemap).overlapsWithCallback(Object1, separateY, true);

			//First, get the two object deltas
            float overlap = 0;
            float obj1delta = Object1.y - Object1.last.y;
            float obj2delta = Object2.y - Object2.last.y;
			if(obj1delta != obj2delta)
			{
				//Check if the Y hulls actually overlap
                float obj1deltaAbs = (obj1delta > 0) ? obj1delta : -obj1delta;
                float obj2deltaAbs = (obj2delta > 0) ? obj2delta : -obj2delta;
				FlxRect obj1rect = new FlxRect(Object1.x,Object1.y-((obj1delta > 0)?obj1delta:0),Object1.width,Object1.height+obj1deltaAbs);
				FlxRect obj2rect = new FlxRect(Object2.x,Object2.y-((obj2delta > 0)?obj2delta:0),Object2.width,Object2.height+obj2deltaAbs);
				if((obj1rect.x + obj1rect.width > obj2rect.x) && (obj1rect.x < obj2rect.x + obj2rect.width) && (obj1rect.y + obj1rect.height > obj2rect.y) && (obj1rect.y < obj2rect.y + obj2rect.height))
				{
                    float maxOverlap = obj1deltaAbs + obj2deltaAbs + OVERLAP_BIAS;

					//If they did overlap (and can), figure out by how much and flip the corresponding flags
					if(obj1delta > obj2delta)
					{
						overlap = Object1.y + Object1.height - Object2.y;
						if((overlap > maxOverlap) || !Convert.ToBoolean(Object1.allowCollisions & DOWN) || !Convert.ToBoolean(Object2.allowCollisions & UP))
							overlap = 0;
						else
						{
							Object1.touching |= DOWN;
							Object2.touching |= UP;
						}
					}
					else if(obj1delta < obj2delta)
					{
						overlap = Object1.y - Object2.height - Object2.y;
						if((-overlap > maxOverlap) || !Convert.ToBoolean(Object1.allowCollisions & UP) || !Convert.ToBoolean(Object2.allowCollisions & DOWN))
							overlap = 0;
						else
						{
							Object1.touching |= UP;
							Object2.touching |= DOWN;
						}
					}
				}
			}

			//Then adjust their positions and velocities accordingly (if there was any overlap)
			if(overlap != 0)
			{
                float obj1v = Object1.velocity.y;
                float obj2v = Object2.velocity.y;

				if(!obj1immovable && !obj2immovable)
				{
					overlap *= 0.5f;
					Object1.y = Object1.y - overlap;
					Object2.y += overlap;

                    float obj1velocity = (float)Math.Sqrt((obj2v * obj2v * Object2.mass) / Object1.mass) * ((obj2v > 0) ? 1f : -1f);
                    float obj2velocity = (float)Math.Sqrt((obj1v * obj1v * Object1.mass) / Object2.mass) * ((obj1v > 0) ? 1f : -1f);
                    float average = (obj1velocity + obj2velocity) * 0.5f;
					obj1velocity -= average;
					obj2velocity -= average;
					Object1.velocity.y = average + obj1velocity *  Object1.elasticity;
					Object2.velocity.y = average + obj2velocity *  Object2.elasticity;
				}
				else if(!obj1immovable)
				{
					Object1.y = Object1.y - overlap;
					Object1.velocity.y = obj2v - obj1v*Object1.elasticity;
					//This is special case code that handles cases like horizontal moving platforms you can ride
					if(Object2.active && Object2.moves && (obj1delta > obj2delta))
						Object1.x += Object2.x - Object2.last.x;
				}
				else if(!obj2immovable)
				{
					Object2.y += overlap;
					Object2.velocity.y = obj1v - obj2v*Object2.elasticity;
					//This is special case code that handles cases like horizontal moving platforms you can ride
					if(Object1.active && Object1.moves && (obj1delta < obj2delta))
						Object2.x += Object1.x - Object1.last.x;
				}
				return true;
			}
			else
				return false;
		}
    }
}
