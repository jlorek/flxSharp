using System;
using System.Collections;
using System.Collections.Generic;

namespace flxSharp.flxSharp
{
    public class FlxGroup : FlxObject, IEnumerable<FlxBasic>
    {
        public const int ASCENDING = -1;
        public const int DESCENDING = 1;
        public List<FlxBasic> members;
        public float length;
        protected uint _maxSize;
        protected uint _marker;
        protected string _sortIndex;
        protected int _sortOrder;
         
        public FlxGroup(uint MaxSize=0) : base()
        {
            members = new List<FlxBasic>();
            length = 0;
            _maxSize = MaxSize;
            _marker = 0;
            _sortIndex = null;
            ID = -3;
        }

        public override void preUpdate()
        {
        }

        public override void update()
        {
            FlxBasic basic;
            uint i = 0;
            //length = members.Count;
            while (i < length)
            {
                basic = members[(int)i++] as FlxBasic;
                if ((basic != null) && basic.Exists && basic.Active)
                {
                    basic.preUpdate();
                    basic.update();
                    basic.postUpdate();
                }
            }
        }

        /// <summary>
        /// Add a FlxObject to the group
        /// </summary>
        /// <param name="Object"></param>
        /// <returns></returns>
        public FlxBasic add(FlxBasic Object)
        {
            members.Add(Object);
            length++;
            return Object;
        }

        /// <summary>
        /// Destroy all objects in the group
        /// </summary>
        public override void destroy()
        {
            base.destroy();
            if (members.Count != 0)
            {
                foreach (FlxBasic m in members)
                {
                    m.destroy();
                }
            }
        }

        /// <summary>
        /// Kill all objects in the group
        /// </summary>
        override public void kill()
        {
            if (members.Count != 0)
            {
                foreach (FlxBasic m in members)
                {
                    if (m.Visible)
                    {
                        m.kill();
                    }
                }
            }
        }

        public override void draw()
        {
            if (Visible)
            {
                if (members.Count != 0)
                {
                    foreach (FlxBasic member in members)
                    {
                        if (member.Visible)
                        {
                            member.draw();                            
                        }
                    }
                }
            }
        }

        public uint maxSize
        {
            get { return _maxSize; }
            set { _maxSize = value; }
        }

        public FlxBasic recycle(Object ObjectClass)
        {
            FlxBasic basic;
            FlxBasic rt = ObjectClass as FlxBasic;
            if (_maxSize > 0)
            {
                if (length < _maxSize)
                {
                    if (ObjectClass == null)
                        return null;
                    rt = new FlxBasic();
                    return add(rt);
                }
                else
                {
                    basic = members[(int)_marker++];
                    if (_marker >= _maxSize)
                        _marker = 0;
                    return basic;
                }
            }
            else
            {
                basic = getFirstAvailable(ObjectClass);
                if (basic != null)
                    return basic;
                if (ObjectClass == null)
                    return null;
                return add(ObjectClass as FlxBasic);
            }
        }

        public FlxBasic getFirstAvailable(Object ObjectClass)
        {
            FlxBasic basic;
            //Object _oc = ObjectClass;
            uint i = 0;
            while (i < length)
            {
                basic = members[(int)i++] as FlxBasic;
                if ((basic != null) && !basic.Exists && ((ObjectClass == null) || (basic is FlxBasic)))
                    return basic;
            }
            return null;
        }

        /// <summary>
        /// Easy <code>FlxGroup</code> enumeration.
        /// </summary>
        /// <returns>Yields a <code>FlxGroup</code> member.</returns>
        public IEnumerator<FlxBasic> GetEnumerator()
        {
            return members.GetEnumerator();
        }

        /// <summary>
        /// Internal enumerator implementation.
        /// </summary>
        /// <returns>Internal usage.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
