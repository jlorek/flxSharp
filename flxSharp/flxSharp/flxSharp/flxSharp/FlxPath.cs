using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using flxSharp.flxSharp;
using flxSharp.flxSharp.System;

namespace fliXNA_xbox
{
    public class FlxPath
    {

        public List<FlxPoint> nodes;

        public FlxColor debugColor;

        public FlxPoint debugScrollFactor;

        public bool ignoreDrawDebug;

        protected FlxPoint _point;

        public FlxPath(List<FlxPoint> Nodes = null)
        {
            if (Nodes == null)
                nodes = new List<FlxPoint>();
            else
                nodes = Nodes;
            _point = new FlxPoint();
            debugScrollFactor = new FlxPoint(1, 1);
            ignoreDrawDebug = false;

            //display path stuff

        }

        public void destroy()
        {
            debugScrollFactor = null;
            _point = null;
            nodes = null;
        }

        public void add(float X, float Y)
        {
            nodes.Add(new FlxPoint(X, Y));
        }


        public void addAt(float X, float Y, int Index)
        {
            if (Index > nodes.Count)
                Index = nodes.Count;
            nodes.Insert(Index, new FlxPoint(X, Y));
        }

        public void addPoint(FlxPoint Node, bool AsReference = false)
        {
            if (AsReference)
                nodes.Add(Node);
            else
                nodes.Add(new FlxPoint(Node.X, Node.Y));
        }

        public void addPointAt(FlxPoint Node, int Index, bool AsReference = false)
        {
            if (Index > nodes.Count)
                Index = nodes.Count;
            if (AsReference)
                nodes.Insert(Index, Node);
            else
                nodes.Insert(Index, new FlxPoint(Node.X, Node.Y));
        }

        public FlxPoint remove(FlxPoint Node)
        {
            int index = nodes.IndexOf(Node);
            if (index >= 0)
                nodes.RemoveAt(index);
            return null;
        }

        public FlxPoint removeAt(int Index)
        {
            if (nodes.Count <= 0)
                return null;
            if (Index >= nodes.Count)
                Index = nodes.Count - 1;
            nodes.RemoveAt(Index);
            return null;
        }

        public FlxPoint head()
        {
            if (nodes.Count > 0)
                return nodes[0];
            return null;
        }

        public FlxPoint tail()
        {
            if (nodes.Count > 0)
                return nodes[nodes.Count - 1];
            return null;
        }

        //draw debug
    }
}
