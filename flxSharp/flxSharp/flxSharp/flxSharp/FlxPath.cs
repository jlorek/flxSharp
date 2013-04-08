using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace flxSharp.flxSharp
{
    /// <summary>
    /// This is a simple path data container. Basically a list of points that
    /// a <code>FlxObject</code> can follow. Also has code for drawing debug visuals.
    /// <code>FlxTilemap.findPath()</code> returns a path object, but you can
    /// also just make your own, using the <code>add()</code> functions below
    /// or by creating your own array of points.
    /// </summary>
    public class FlxPath
    {
        /// <summary>
        /// The list of <code>FlxPoint</code>s that make up the path data.
        /// </summary>
        public List<FlxPoint> Nodes { get; set; }

        /// <summary>
        /// Specify a debug display color for the path. Default is white.
        /// </summary>
        public Color DebugColor { get; set; }

        /// <summary>
        /// Specify a debug display scroll factor for the path.  Default is (1,1).
        /// NOTE: does not affect world movement! Object scroll factors take care of that.
        /// </summary>
        public FlxPoint DebugScrollFactor { get; set; }

        /// <summary>
        /// Setting this to true will prevent the object from appearing
        /// when the visual debug mode in the debugger overlay is toggled on.
        /// @default false
        /// </summary>
        public bool IgnoreDrawDebug { get; set; }

        protected FlxPoint _point;

        /// <summary>
        /// Instantiate a new path object.
        /// </summary>
        /// <param name="nodes">Optional, can specify all the points for the path up front if you want.</param>
        public FlxPath(IEnumerable<FlxPoint> nodes = null)
        {
            Nodes = (nodes == null) ? new List<FlxPoint>() : nodes.ToList();

            _point = new FlxPoint();
            DebugColor = Color.White;
            DebugScrollFactor = new FlxPoint(1, 1);
            IgnoreDrawDebug = false;

            // @TODO
            /*
			var debugPathDisplay:DebugPathDisplay = manager;
			if(debugPathDisplay != null)
				debugPathDisplay.add(this); 
            */
        }

        public void destroy()
        {
            DebugScrollFactor = null;
            _point = null;
            Nodes = null;
        }

        /// <summary>
        /// Add a new node to the end of the path at the specified location.
        /// </summary>
        /// <param name="x">X position of the new path point in world coordinates.</param>
        /// <param name="y">Y position of the new path point in world coordinates.</param>
        public void Add(float x, float y)
        {
            Nodes.Add(new FlxPoint(x, y));
        }

        /// <summary>
        /// Add a new node to the path at the specified location and index within the path.
        /// </summary>
        /// <param name="x">X position of the new path point in world coordinates.</param>
        /// <param name="y">Y position of the new path point in world coordinates.</param>
        /// <param name="index">Where within the list of path nodes to insert this new point.</param>
        public void AddAt(float x, float y, int index)
        {
            if (index > Nodes.Count)
            {
                index = Nodes.Count;                
            }

            Nodes.Insert(index, new FlxPoint(x, y));
        }

        /// <summary>
        /// Sometimes its easier or faster to just pass a point object instead of separate X and Y coordinates.
        /// This also gives you the option of not creating a new node but actually adding that specific
        /// <code>FlxPoint</code> object to the path. This allows you to do neat things, like dynamic paths.
        /// </summary>
        /// <param name="node">The point in world coordinates you want to add to the path.</param>
        /// <param name="asReference">Whether to add the point as a reference, or to create a new point with the specified values.</param>
        public void AddPoint(FlxPoint node, bool asReference = false)
        {
            Nodes.Add(asReference ? node : new FlxPoint(node.X, node.Y));
        }

        /// <summary>
        /// Sometimes its easier or faster to just pass a point object instead of separate X and Y coordinates.
        /// This also gives you the option of not creating a new node but actually adding that specific
        /// <code>FlxPoint</code> object to the path.  This allows you to do neat things, like dynamic paths.
        /// </summary>
        /// <param name="node">The point in world coordinates you want to add to the path.</param>
        /// <param name="index">Where within the list of path nodes to insert this new point.</param>
        /// <param name="asReference">Whether to add the point as a reference, or to create a new point with the specified values.</param>
        public void AddPointAt(FlxPoint node, int index, bool asReference = false)
        {
            if (index > Nodes.Count)
            {
                index = Nodes.Count;                
            }

            Nodes.Insert(index, asReference ? node : new FlxPoint(node.X, node.Y));
        }

        /// <summary>
        /// Remove a node from the path.
        /// NOTE: only works with points added by reference or with references from <code>nodes</code> itself!
        /// </summary>
        /// <param name="node">The point object you want to remove from the path.</param>
        /// <returns>The node that was excised. Returns null if the node was not found.</returns>
        public FlxPoint Remove(FlxPoint node)
        {
            if (Nodes.Remove(node))
            {
                return node;
            }

            return null;
        }

        /// <summary>
        /// Remove a node from the path using the specified position in the list of path nodes.
        /// </summary>
        /// <param name="index">Where within the list of path nodes you want to remove a node.</param>
        /// <returns>The node that was excised. Returns null if there were no nodes in the path.</returns>
        public FlxPoint RemoveAt(int index)
        {
            if (!Nodes.Any())
            {
                return null;                
            }

            // @TODO: if called with wrong arguments this should crash
            /*
            if (index >= Nodes.Count)
            {
                index = Nodes.Count - 1;                
            }
            */

            FlxPoint removedNode = Nodes[index];
            Nodes.RemoveAt(index);
            return removedNode;
        }

        /// <summary>
        /// Get the first node in the list.
        /// </summary>
        /// <returns>The first node in the path.</returns>
        public FlxPoint Head()
        {
            return Nodes.First();
        }

        /// <summary>
        /// Get the last node in the list.
        /// </summary>
        /// <returns>The last node in the path.</returns>
        public FlxPoint Tail()
        {
            return Nodes.Last();
        }

        public object DebugPathManager
        {
            get
            {
                throw new NotImplementedException();
                // return FlxG.getPlugin(DebugPathDisplay) as DebugPathDisplay;
            }
        }

        /// <summary>
        /// While this doesn't override <code>FlxBasic.DrawDebug()</code>, the behavior is very similar.
        /// Based on this path data, it draws a simple lines-and-boxes representation of the path
        /// if the visual debug mode was toggled in the debugger overlay. You can use <code>DebugColor</code>
        /// and <code>DebugScrollFactor</code> to control the path's appearance.
        /// </summary>
        /// <param name="camera">The camera object the path will draw to.</param>
        public void DrawDebug(FlxCamera camera)
        {
            throw new NotImplementedException();

            /*
			if(nodes.length <= 0)
				return;
			if(Camera == null)
				Camera = FlxG.camera;
			
			//Set up our global flash graphics object to draw out the path
			var gfx:Graphics = FlxG.flashGfx;
			gfx.clear();
			
			//Then fill up the object with node and path graphics
			var node:FlxPoint;
			var nextNode:FlxPoint;
			var i:uint = 0;
			var l:uint = nodes.length;
			while(i < l)
			{
				//get a reference to the current node
				node = nodes[i] as FlxPoint;
				
				//find the screen position of the node on this camera
				_point.x = node.x - int(Camera.scroll.x*debugScrollFactor.x); //copied from getScreenXY()
				_point.y = node.y - int(Camera.scroll.y*debugScrollFactor.y);
				_point.x = int(_point.x + ((_point.x > 0)?0.0000001:-0.0000001));
				_point.y = int(_point.y + ((_point.y > 0)?0.0000001:-0.0000001));
				
				//decide what color this node should be
				var nodeSize:uint = 2;
				if((i == 0) || (i == l-1))
					nodeSize *= 2;
				var nodeColor:uint = debugColor;
				if(l > 1)
				{
					if(i == 0)
						nodeColor = FlxG.GREEN;
					else if(i == l-1)
						nodeColor = FlxG.RED;
				}
				
				//draw a box for the node
				gfx.beginFill(nodeColor,0.5);
				gfx.lineStyle();
				gfx.drawRect(_point.x-nodeSize*0.5,_point.y-nodeSize*0.5,nodeSize,nodeSize);
				gfx.endFill();

				//then find the next node in the path
				var linealpha:Number = 0.3;
				if(i < l-1)
					nextNode = nodes[i+1];
				else
				{
					nextNode = nodes[0];
					linealpha = 0.15;
				}
				
				//then draw a line to the next node
				gfx.moveTo(_point.x,_point.y);
				gfx.lineStyle(1,debugColor,linealpha);
				_point.x = nextNode.x - int(Camera.scroll.x*debugScrollFactor.x); //copied from getScreenXY()
				_point.y = nextNode.y - int(Camera.scroll.y*debugScrollFactor.y);
				_point.x = int(_point.x + ((_point.x > 0)?0.0000001:-0.0000001));
				_point.y = int(_point.y + ((_point.y > 0)?0.0000001:-0.0000001));
				gfx.lineTo(_point.x,_point.y);

				i++;
			}
			
			//then stamp the path down onto the game buffer
			Camera.buffer.draw(FlxG.flashGfxSprite);
            */
        }
    }
}
