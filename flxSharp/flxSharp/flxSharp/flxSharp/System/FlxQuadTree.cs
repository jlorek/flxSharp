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
    public class FlxQuadTree : FlxRect
    {
        public const uint A_LIST = 0;
        public const uint B_LIST = 1;
        static public uint divisions;
        protected Boolean _canSubdivide;
        protected FlxList _headA;
        protected FlxList _tailA;
        protected FlxList _headB;
        protected FlxList _tailB;
        protected uint _min;
        protected FlxQuadTree _northWestTree;
        protected FlxQuadTree _northEastTree;
        protected FlxQuadTree _southEastTree;
        protected FlxQuadTree _southWestTree;
        protected float _leftEdge;
        protected float _rightEdge;
        protected float _topEdge;
        protected float _bottomEdge;
        protected float _halfWidth;
        protected float _halfHeight;
        protected float _midpointX;
        protected float _midpointY;
        protected FlxObject _object;
        protected float _objectLeftEdge;
        protected float _objectTopEdge;
        protected float _objectRightEdge;
        protected float _objectBottomEdge;
        protected uint _list;
        protected Boolean _useBothLists;
        protected Func<FlxObject, FlxObject, Boolean> _processingCallback;
        protected Func<FlxObject, FlxObject, Boolean> _notifyCallback;
        protected FlxList _iterator;
        protected float _objectHullX;
        protected float _objectHullY;
        protected float _objectHullWidth;
        protected float _objectHullHeight;
        protected float _checkObjectHullX;
        protected float _checkObjectHullY;
        protected float _checkObjectHullWidth;
        protected float _checkObjectHullHeight;

        public FlxQuadTree(float X, float Y, float Width, float Height, FlxQuadTree Parent = null)
            : base(X, Y, Width, Height)
        {
            _headA = _tailA = new FlxList();
            _headB = _tailB = new FlxList();

            //Copy the parent's children (if there are any)
			if(Parent != null)
			{
				FlxList iterator;
				FlxList ot;
				if(Parent._headA.Object != null)
				{
					iterator = Parent._headA;
					while(iterator != null)
					{
						if(_tailA.Object != null)
						{
							ot = _tailA;
							_tailA = new FlxList();
							ot.next = _tailA;
						}
						_tailA.Object = iterator.Object;
						iterator = iterator.next;
					}
				}
				if(Parent._headB.Object != null)
				{
					iterator = Parent._headB;
					while(iterator != null)
					{
						if(_tailB.Object != null)
						{
							ot = _tailB;
							_tailB = new FlxList();
							ot.next = _tailB;
						}
						_tailB.Object = iterator.Object;
						iterator = iterator.next;
					}
				}
			}
            else
                _min = (uint)(base.Width + base.Height) / (2 * divisions);
            _canSubdivide = (base.Width > _min) || (base.Height > _min);

            //Set up comparison/sort helpers
            _northWestTree = null;
            _northEastTree = null;
            _southEastTree = null;
            _southWestTree = null;
            _leftEdge = base.X;
            _rightEdge = base.X + base.Width;
            _halfWidth = base.Width / 2;
            _midpointX = _leftEdge + _halfWidth;
            _topEdge = base.Y;
            _bottomEdge = base.Y + base.Height;
            _halfHeight = base.Height / 2;
            _midpointY = _topEdge + _halfHeight;
        }

        public void destroy()
		{
			_headA.destroy();
			_headA = null;
			_tailA.destroy();
			_tailA = null;
			_headB.destroy();
			_headB = null;
			_tailB.destroy();
			_tailB = null;

			if(_northWestTree != null)
				_northWestTree.destroy();
			_northWestTree = null;
			if(_northEastTree != null)
				_northEastTree.destroy();
			_northEastTree = null;
			if(_southEastTree != null)
				_southEastTree.destroy();
			_southEastTree = null;
			if(_southWestTree != null)
				_southWestTree.destroy();
			_southWestTree = null;

			_object = null;
			_processingCallback = null;
			_notifyCallback = null;
		}

        public void load(FlxBasic ObjectOrGroup1, FlxBasic ObjectOrGroup2 = null, Func<FlxObject, FlxObject, Boolean> NotifyCallback = null, Func<FlxObject, FlxObject, Boolean> ProcessCallback = null)
		{
			add(ObjectOrGroup1, A_LIST);
			if(ObjectOrGroup2 != null)
			{
				add(ObjectOrGroup2, B_LIST);
				_useBothLists = true;
			}
			else
				_useBothLists = false;
			_notifyCallback = NotifyCallback;
			_processingCallback = ProcessCallback;
		}

        public void add(FlxBasic ObjectOrGroup, uint List)
		{
			_list = List;
			if(ObjectOrGroup is FlxGroup)
			{
				uint i = 0;
				FlxBasic basic;
                List<FlxBasic> members = new List<FlxBasic>();
                members = (ObjectOrGroup as FlxGroup).Members;
                uint l = (uint)members.Count;
				while(i < l)
				{
					basic = members[(int)i++] as FlxBasic;
					if((basic != null) && basic.Exists)
					{
						if(basic is FlxGroup)
							add(basic,List);
						else if(basic is FlxObject)
						{
							_object = basic as FlxObject;
                            if(_object.Exists && Convert.ToBoolean(_object.AllowCollisions))
							{
								_objectLeftEdge = _object.X;
								_objectTopEdge = _object.Y;
								_objectRightEdge = _object.X + _object.Width;
								_objectBottomEdge = _object.Y + _object.Height;
								addObject();
							}
						}
					}
				}
			}
			else
			{
				_object = ObjectOrGroup as FlxObject;
				if(_object.Exists && Convert.ToBoolean(_object.AllowCollisions) )
				{
					_objectLeftEdge = _object.X;
					_objectTopEdge = _object.Y;
					_objectRightEdge = _object.X + _object.Width;
					_objectBottomEdge = _object.Y + _object.Height;
					addObject();
				}
			}
		}



        protected void addObject()
		{
			//If this quad (not its children) lies entirely inside this object, add it here
			if(!_canSubdivide || ((_leftEdge >= _objectLeftEdge) && (_rightEdge <= _objectRightEdge) && (_topEdge >= _objectTopEdge) && (_bottomEdge <= _objectBottomEdge)))
			{
				addToList();
				return;
			}
			
			//See if the selected object fits completely inside any of the quadrants
			if((_objectLeftEdge > _leftEdge) && (_objectRightEdge < _midpointX))
			{
				if((_objectTopEdge > _topEdge) && (_objectBottomEdge < _midpointY))
				{
					if(_northWestTree == null)
						_northWestTree = new FlxQuadTree(_leftEdge,_topEdge,_halfWidth,_halfHeight,this);
					_northWestTree.addObject();
					return;
				}
				if((_objectTopEdge > _midpointY) && (_objectBottomEdge < _bottomEdge))
				{
					if(_southWestTree == null)
						_southWestTree = new FlxQuadTree(_leftEdge,_midpointY,_halfWidth,_halfHeight,this);
					_southWestTree.addObject();
					return;
				}
			}
			if((_objectLeftEdge > _midpointX) && (_objectRightEdge < _rightEdge))
			{
				if((_objectTopEdge > _topEdge) && (_objectBottomEdge < _midpointY))
				{
					if(_northEastTree == null)
						_northEastTree = new FlxQuadTree(_midpointX,_topEdge,_halfWidth,_halfHeight,this);
					_northEastTree.addObject();
					return;
				}
				if((_objectTopEdge > _midpointY) && (_objectBottomEdge < _bottomEdge))
				{
					if(_southEastTree == null)
						_southEastTree = new FlxQuadTree(_midpointX,_midpointY,_halfWidth,_halfHeight,this);
					_southEastTree.addObject();
					return;
				}
			}
			
			//If it wasn't completely contained we have to check out the partial overlaps
			if((_objectRightEdge > _leftEdge) && (_objectLeftEdge < _midpointX) && (_objectBottomEdge > _topEdge) && (_objectTopEdge < _midpointY))
			{
				if(_northWestTree == null)
					_northWestTree = new FlxQuadTree(_leftEdge,_topEdge,_halfWidth,_halfHeight,this);
				_northWestTree.addObject();
			}
			if((_objectRightEdge > _midpointX) && (_objectLeftEdge < _rightEdge) && (_objectBottomEdge > _topEdge) && (_objectTopEdge < _midpointY))
			{
				if(_northEastTree == null)
					_northEastTree = new FlxQuadTree(_midpointX,_topEdge,_halfWidth,_halfHeight,this);
				_northEastTree.addObject();
			}
			if((_objectRightEdge > _midpointX) && (_objectLeftEdge < _rightEdge) && (_objectBottomEdge > _midpointY) && (_objectTopEdge < _bottomEdge))
			{
				if(_southEastTree == null)
					_southEastTree = new FlxQuadTree(_midpointX,_midpointY,_halfWidth,_halfHeight,this);
				_southEastTree.addObject();
			}
			if((_objectRightEdge > _leftEdge) && (_objectLeftEdge < _midpointX) && (_objectBottomEdge > _midpointY) && (_objectTopEdge < _bottomEdge))
			{
				if(_southWestTree == null)
					_southWestTree = new FlxQuadTree(_leftEdge,_midpointY,_halfWidth,_halfHeight,this);
				_southWestTree.addObject();
			}
		}


        protected void addToList()
		{
			FlxList ot;
			if(_list == A_LIST)
			{
				if(_tailA.Object != null)
				{
					ot = _tailA;
					_tailA = new FlxList();
					ot.next = _tailA;
				}
				_tailA.Object = _object;
			}
			else
			{
				if(_tailB.Object != null)
				{
					ot = _tailB;
					_tailB = new FlxList();
					ot.next = _tailB;
				}
				_tailB.Object = _object;
			}
			if(!_canSubdivide)
				return;
			if(_northWestTree != null)
				_northWestTree.addToList();
			if(_northEastTree != null)
				_northEastTree.addToList();
			if(_southEastTree != null)
				_southEastTree.addToList();
			if(_southWestTree != null)
				_southWestTree.addToList();
		}


        public Boolean execute()
		{
			Boolean overlapProcessed = false;
			FlxList iterator;
			
			if(_headA.Object != null)
			{
				iterator = _headA;
				while(iterator != null)
				{
					_object = iterator.Object;
					if(_useBothLists)
						_iterator = _headB;
					else
						_iterator = iterator.next;
					if(	_object.Exists && (_object.AllowCollisions > 0) &&
						(_iterator != null) && (_iterator.Object != null) &&
						_iterator.Object.Exists && overlapNode())
					{
						overlapProcessed = true;
					}
					iterator = iterator.next;
				}
			}
			
			//Advance through the tree by calling overlap on each child
			if((_northWestTree != null) && _northWestTree.execute())
				overlapProcessed = true;
			if((_northEastTree != null) && _northEastTree.execute())
				overlapProcessed = true;
			if((_southEastTree != null) && _southEastTree.execute())
				overlapProcessed = true;
			if((_southWestTree != null) && _southWestTree.execute())
				overlapProcessed = true;
			
			return overlapProcessed;
		}

        protected Boolean overlapNode()
		{
			//Walk the list and check for overlaps
			Boolean overlapProcessed = false;
			FlxObject checkObject;
			while(_iterator != null)
			{
				if(!_object.Exists || (_object.AllowCollisions <= 0))
					break;
				
				checkObject = _iterator.Object;
				if((_object == checkObject) || !checkObject.Exists || (checkObject.AllowCollisions <= 0))
				{
					_iterator = _iterator.next;
					continue;
				}
				
				//calculate bulk hull for _object
				_objectHullX = (_object.X < _object.Last.X)?_object.X:_object.Last.X;
				_objectHullY = (_object.Y < _object.Last.Y)?_object.Y:_object.Last.Y;
				_objectHullWidth = _object.X - _object.Last.X;
				_objectHullWidth = _object.Width + ((_objectHullWidth>0)?_objectHullWidth:-_objectHullWidth);
				_objectHullHeight = _object.Y - _object.Last.Y;
				_objectHullHeight = _object.Height + ((_objectHullHeight>0)?_objectHullHeight:-_objectHullHeight);
				
				//calculate bulk hull for checkObject
				_checkObjectHullX = (checkObject.X < checkObject.Last.X)?checkObject.X:checkObject.Last.X;
				_checkObjectHullY = (checkObject.Y < checkObject.Last.Y)?checkObject.Y:checkObject.Last.Y;
				_checkObjectHullWidth = checkObject.X - checkObject.Last.X;
				_checkObjectHullWidth = checkObject.Width + ((_checkObjectHullWidth>0)?_checkObjectHullWidth:-_checkObjectHullWidth);
                _checkObjectHullHeight = checkObject.Y - checkObject.Last.Y;
				_checkObjectHullHeight = checkObject.Height + ((_checkObjectHullHeight>0)?_checkObjectHullHeight:-_checkObjectHullHeight);
				
				//check for intersection of the two hulls
				if(	(_objectHullX + _objectHullWidth > _checkObjectHullX) &&
					(_objectHullX < _checkObjectHullX + _checkObjectHullWidth) &&
					(_objectHullY + _objectHullHeight > _checkObjectHullY) &&
					(_objectHullY < _checkObjectHullY + _checkObjectHullHeight) )
				{
					//Execute callback functions if they exist
					if((_processingCallback == null) || _processingCallback(_object,checkObject))
						overlapProcessed = true;
					if(overlapProcessed && (_notifyCallback != null))
						_notifyCallback(_object,checkObject);
				}
				_iterator = _iterator.next;
			}
			
			return overlapProcessed;
		}
    }
}
