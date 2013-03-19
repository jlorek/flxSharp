using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace flxSharp.flxSharp
{
    /// <summary>
    /// This is an organizational class that can update and render a bunch of <code>FlxBasic</code>s.
    /// NOTE: Although <code>FlxGroup</code> extends <code>FlxBasic</code>, it will not automatically
    /// add itself to the global collisions quad tree, it will only add its members.
    /// </summary>
    public class FlxGroup : FlxObject, IEnumerable<FlxBasic>
    {
        /// <summary>
        /// Use with <code>sort()</code> to sort in ascending order.
        /// </summary>
        public const int Ascending = -1;

        /// <summary>
        /// Use with <code>sort()</code> to sort in descending order.
        /// </summary>
        public const int Descending = 1;

        /// <summary>
        /// Array of all the <code>FlxBasic</code>s that exist in this group.
        /// </summary>
        public List<FlxBasic> Members;
        
        /// <summary>
        /// The number of entries in the members array.
        /// For performance and safety you should check this variable
        /// instead of members.length unless you really know what you're doing!
        /// </summary>
        public float length
        {
            get { return Members.Count; }
        }
        
        /// <summary>
        /// Internal tracker for the maximum capacity of the group.
        /// Default is 0, or no max capacity.
        /// </summary>
        protected uint _maxSize;

        public uint maxSize
        {
            get { return _maxSize; }
            set { _maxSize = value; }
        }

        /// <summary>
        /// The maximum capacity of this group.
        /// Default is 0, meaning no max capacity, and the group can just grow.
        /// 
        /// flx# - GC magic!
        /// </summary>
        public uint MaxSize
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }
        
        /// <summary>
        /// Internal helper variable for recycling objects a la <code>FlxEmitter</code>.
        /// </summary>
        protected uint _marker;
        
        /// <summary>
        /// Helper for sort.
        /// </summary>
        protected string _sortIndex;
        
        /// <summary>
        /// Helper for sort.
        /// </summary>
        protected int _sortOrder;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="maxSize"></param>
        public FlxGroup(uint maxSize = 0) : base()
        {
            Members = new List<FlxBasic>();
            //length = 0;
            _maxSize = maxSize;
            _marker = 0;
            _sortIndex = null;
        }

        /// <summary>
        /// Override this function to handle any deleting or "shutdown" type operations you might need,
        /// such as removing traditional Flash children like Sprite objects.
        /// </summary>
        public override void destroy()
        {
            foreach (FlxBasic flxBasic in Members)
            {
                flxBasic.destroy();
            }

            _sortIndex = null;
            base.destroy();
        }

        /// <summary>
        /// Just making sure we don't increment the active objects count.
        /// </summary>
        public override void preUpdate()
        { }

        /// <summary>
        /// Automatically goes through and calls update on everything you added.
        /// </summary>
        public override void update()
        {
            foreach (FlxBasic flxBasic in Members)
            {
                if (flxBasic.Exists && flxBasic.Active)
                {
                    flxBasic.preUpdate();
                    flxBasic.update();
                    flxBasic.postUpdate();
                }
            }
        }

        /// <summary>
        /// Automatically goes through and calls render on everything you added.
        /// </summary>
        public override void draw()
        {
            if (Visible)
            {
                foreach (FlxBasic flxBasic in Members)
                {
                    if (flxBasic.Exists && flxBasic.Visible)
                    {
                        flxBasic.draw();
                    }
                }
            }
        }

        /// <summary>
        /// Adds a new <code>FlxBasic</code> subclass (FlxBasic, FlxSprite, Enemy, etc) to the group.
        /// FlxGroup will try to replace a null member of the array first.
        /// Failing that, FlxGroup will add it to the end of the member array,
        /// assuming there is room for it, and doubling the size of the array if necessary.
        ///
        /// <p>WARNING: If the group has a maxSize that has already been met,
        /// the object will NOT be added to the group!</p>
        /// </summary>
        /// <param name="flxBasic">The object you want to add to the group.</param>
        /// <returns>The same <code>FlxBasic</code> object that was passed in.</returns>
        public FlxBasic add(FlxBasic flxBasic)
        {
            if (Members.Contains(flxBasic))
            {
                return flxBasic;
            }

            Members.Add(flxBasic);
            return flxBasic;
        }

        /// <summary>
        /// Recycling is designed to help you reuse game objects without always re-allocating or "newing" them.
        /// 
        /// <p>If you specified a maximum size for this group (like in FlxEmitter),
        /// then recycle will employ what we're calling "rotating" recycling.
        /// Recycle() will first check to see if the group is at capacity yet.
        /// If group is not yet at capacity, recycle() returns a new object.
        /// If the group IS at capacity, then recycle() just returns the next object in line.</p>
        /// 
        /// <p>If you did NOT specify a maximum size for this group,
        /// then recycle() will employ what we're calling "grow-style" recycling.
        /// Recycle() will return either the first object with exists == false,
        /// or, finding none, add a new object to the array,
        /// doubling the size of the array if necessary.</p>
        /// 
        /// <p>WARNING: If this function needs to create a new object,
        /// and no object class was provided, it will return null
        /// instead of a valid object!</p>
        /// </summary>
        /// <param name="ObjectClass">The class type you want to recycle (e.g. FlxSprite, EvilRobot, etc). Do NOT "new" the class in the parameter!</param>
        /// <returns>A reference to the object that was created. Don't forget to cast it back to the Class you want (e.g. myObject = myGroup.recycle(myObjectClass) as myObjectClass;).</returns>
        public FlxBasic recycle(Object ObjectClass)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes an object from the group.
        /// </summary>
        /// <param name="flxBasic">The <code>FlxBasic</code> you want to remove.</param>
        /// <param name="splice">Whether the object should be cut from the array entirely or not.</param>
        /// <returns>The removed object.</returns>
        public FlxBasic Remove(FlxBasic flxBasic, bool splice = false)
        {
            if (splice) throw new NotSupportedException();

            bool wasFound = Members.Remove(flxBasic);

            if (!wasFound)
            {
                return null;
            }

            return flxBasic;
        }

        /// <summary>
        /// Replaces an existing <code>FlxBasic</code> with a new one.
        /// </summary>
        /// <param name="oldObject">The object you want to replace.</param>
        /// <param name="newObject">The new object you want to use instead.</param>
        /// <returns></returns>
        public FlxBasic Replace(FlxBasic oldObject, FlxBasic newObject)
        {
            int index = Members.IndexOf(oldObject);

            if (index < 0)
            {
                return null;
            }

            // flx# - what about the oldObject?

            Members[index] = newObject;
            return newObject;
        }

        /// <summary>
        /// Call this function to sort the group according to a particular value and order.
        /// For example, to sort game objects for Zelda-style overlaps you might call
        /// <code>myGroup.sort("y",ASCENDING)</code> at the bottom of your
        /// <code>FlxState.update()</code> override.  To sort all existing objects after
        /// a big explosion or bomb attack, you might call <code>myGroup.sort("exists",DESCENDING)</code>.
        /// </summary>
        /// <param name="propertyName">The <code>String</code> name of the member variable you want to sort on.  Default value is "y".</param>
        /// <param name="order">A <code>FlxGroup</code> constant that defines the sort order. Possible values are <code>ASCENDING</code> and <code>DESCENDING</code>. Default value is <code>ASCENDING</code>.</param>
        public void Sort(string propertyName = "y", int order = Ascending)
        {
            Func<FlxBasic, object> propertySelector = flxBasic =>
            {
                Type type = flxBasic.GetType();
                PropertyInfo propertyInfo = type.GetProperty(propertyName);
                MethodInfo getMethod = propertyInfo.GetGetMethod();
                return getMethod.Invoke(flxBasic, null);
            };

            if (order == Ascending)
            {
                Members = Members.OrderBy(propertySelector).ToList();
            }
            else
            {
                Members = Members.OrderByDescending(propertySelector).ToList();
            }
        }

        /// <summary>
        /// Go through and set the specified variable to the specified value on all members of the group.
        /// </summary>
        /// <param name="propertyName">The string representation of the variable name you want to modify, for example "visible" or "scrollFactor".</param>
        /// <param name="value">The value you want to assign to that variable.</param>
        /// <param name="recurse">Default value is true, meaning if <code>setAll()</code> encounters a member that is a group, it will call <code>setAll()</code> on that group rather than modifying its variable.</param>
        public void SetAll(string propertyName, object value, bool recurse = true)
        {
            foreach (FlxBasic flxBasic in Members)
            {
                if (recurse && (flxBasic is FlxGroup))
                {
                    (flxBasic as FlxGroup).SetAll(propertyName, value, recurse);
                }
                else
                {
                    Type type = flxBasic.GetType();
                    PropertyInfo propertyInfo = type.GetProperty(propertyName);
                    MethodInfo setMethod = propertyInfo.GetSetMethod();
                    setMethod.Invoke(flxBasic, new[] {value});
                }
            }
        }

        /// <summary>
        /// Go through and call the specified function on all members of the group.
        /// Currently only works on functions that have no required parameters.
        /// </summary>
        /// <param name="methodName">The string representation of the function you want to call on each object, for example "kill()" or "init()".</param>
        /// <param name="recurse">Default value is true, meaning if <code>callAll()</code> encounters a member that is a group, it will call <code>callAll()</code> on that group rather than calling the group's function.</param>
        public void CallAll(string methodName, bool recurse = true)
        {
            foreach (FlxBasic flxBasic in Members)
            {
                if (recurse && (flxBasic is FlxGroup))
                {
                    (flxBasic as FlxGroup).CallAll(methodName, recurse);
                }
                else
                {
                    Type type = flxBasic.GetType();
                    MethodInfo method = type.GetMethod(methodName);
                    method.Invoke(flxBasic, null);
                }
            }
        }

        /// <summary>
        /// Call this function to retrieve the first object with exists == false in the group.
        /// This is handy for recycling in general, e.g. respawning enemies.
        /// </summary>
        /// <param name="objectClass">An optional parameter that lets you narrow the results to instances of this particular class.</param>
        /// <returns>A <code>FlxBasic</code> currently flagged as not existing.</returns>
        public FlxBasic GetFirstAvailable(Type objectClass)
        {
            foreach (FlxBasic flxBasic in Members)
            {
                if ((flxBasic != null) &&
                    !flxBasic.Exists &&
                    ((objectClass == null) || (flxBasic.GetType() == objectClass)))
                {
                    return flxBasic;
                }
            }

            return null;
        }

        /// <summary>
        /// Call this function to retrieve the first index set to 'null'.
        /// Returns -1 if no index stores a null object.
        /// </summary>
        /// <returns>An <code>int</code> indicating the first null slot in the group.</returns>
        public int GetFirstNull()
        {
            for (int i = 0; i < Members.Count; ++i)
            {
                if (Members[i] == null)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Call this function to retrieve the first object with exists == true in the group.
        /// This is handy for checking if everything's wiped out, or choosing a squad leader, etc.
        /// </summary>
        /// <returns>A <code>FlxBasic</code> currently flagged as existing.</returns>
        public FlxBasic GetFirstExtant()
        {
            /*
            foreach (FlxBasic flxBasic in Members)
            {
                if ((flxBasic != null) &&
                    flxBasic.Exists)
                {
                    return flxBasic;
                }
            }

            return null;
            */

            return Members.FirstOrDefault(
                flxBasic => (flxBasic != null) && flxBasic.Exists);
        }

        /// <summary>
        /// Call this function to retrieve the first object with dead == false in the group.
        /// This is handy for checking if everything's wiped out, or choosing a squad leader, etc.
        /// </summary>
        /// <returns>A <code>FlxBasic</code> currently flagged as not dead.</returns>
        public FlxBasic GetFirstAlive()
        {
            return Members.FirstOrDefault(
                flxBasic => (flxBasic != null) && flxBasic.Exists && flxBasic.Alive);
        }

        /// <summary>
        /// Call this function to retrieve the first object with dead == true in the group.
        /// This is handy for checking if everything's wiped out, or choosing a squad leader, etc.
        /// </summary>
        /// <returns>A <code>FlxBasic</code> currently flagged as dead.</returns>
        public FlxBasic GetFirstDead()
        {
            return Members.FirstOrDefault(
                flxBasic => (flxBasic != null) && !flxBasic.Alive);
        }

        /// <summary>
        /// Call this function to find out how many members of the group are not dead.
        /// </summary>
        /// <returns>The number of <code>FlxBasic</code>s flagged as not dead.  Returns -1 if group is empty.</returns>
        public int CountLiving()
        {
            if (!Members.Any(flxBasic => flxBasic != null))
            {
                return -1;
            }

            return Members.Count(flxBasic => flxBasic.Exists && flxBasic.Alive);
        }

        /// <summary>
        /// Call this function to find out how many members of the group are dead.
        /// </summary>
        /// <returns>The number of <code>FlxBasic</code>s flagged as dead.  Returns -1 if group is empty.</returns>
        public int CountDead()
        {
            if (!Members.Any(flxBasic => flxBasic != null))
            {
                return -1;
            }

            return Members.Count(flxBasic => !flxBasic.Alive);
        }

        /// <summary>
        /// Returns a member at random from the group.
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="selectLength"></param>
        /// <returns></returns>
        public FlxBasic GetRandom(int startIndex = 0, int selectLength = 0)
        {
            if (selectLength == 0)
            {
                selectLength = Members.Count;
            }

            return FlxG.GetRandom(
                this.ToArray(), startIndex, selectLength) as FlxBasic;
        }

        /// <summary>
        /// Remove all instances of <code>FlxBasic</code> subclass (FlxSprite, FlxBlock, etc) from the list.
        /// WARNING: does not destroy() or kill() any of these objects!
        /// </summary>
        public void Clear()
        {
            Members.Clear();
        }

        /// <summary>
        /// Calls kill on the group's members and then on the group itself.
        /// </summary>
        public override void kill()
        {
            foreach (FlxBasic flxBasic in Members)
            {
                if ((flxBasic != null) && flxBasic.Exists)
                {
                    flxBasic.kill();
                }
            }

            base.kill();
        }

        /// <summary>
        /// Easy <code>FlxGroup</code> enumeration.
        /// </summary>
        /// <returns>Yields a <code>FlxGroup</code> member.</returns>
        public IEnumerator<FlxBasic> GetEnumerator()
        {
            return Members.GetEnumerator();
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
