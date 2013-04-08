using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
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
    public class FlxTilemap : FlxObject
    {
        public const uint OFF = 0;
        public const uint AUTO = 1;
        public const uint ALT = 2;
        public uint auto;
        int widthInTiles;
        int heightInTiles;
        int totalTiles;

        public Texture2D tileGraphic;
        protected Texture2D _tiles;   // reference to tile graphics
        protected List<int> _data;
        protected Rectangle[] _rects;
        protected int _tileWidth;
        protected int _tileHeight;
        protected int tileSize;

        protected FlxObject _block;

        protected List<FlxTile> _tileObjectsForDrawing;
        protected FlxTile[] _tileObjects;

        protected uint _startingIndex;
        protected int collideIndex;

        public FlxTilemap()
            : base()
        {
            auto = OFF;

            widthInTiles = 0;
            heightInTiles = 0;
            totalTiles = 0;

            tileGraphic = null;
            _rects = null;
            _tiles = null;
            _tileObjectsForDrawing = new List<FlxTile>();
            _tileObjects = null;
            

            _tileWidth = 0;
            _tileHeight = 0;

            Immovable = true;

            _block = new FlxObject();
            _block.Width = _block.Height = 0;
            _block.Immovable = true;

            _startingIndex = 0;
            collideIndex = 1;

            AllowCollisions = Any;
            ID = 3;
        }

        public FlxTilemap loadMap(String CSVdataFile, Texture2D Graphic, int TileWidth, int TileHeight, int DrawIndex = 0, int CollideIndex = 1)
        {
            collideIndex = CollideIndex;
            //_tileWidth = _tileHeight = tileSize = TileSize;

            StreamReader stream = new StreamReader(TitleContainer.OpenStream(CSVdataFile));
            CSVdataFile = stream.ReadToEnd();

            String[] columns;
            String[] rows = new String[] { };
            rows = CSVdataFile.Split('\n');

            heightInTiles = rows.Length;
            _data = new List<int>();

            int row = 0;
            int column;
            while (row < heightInTiles)
            {
                columns = rows[row++].Split(',');
                if (columns.Length <= 1)
                {
                    heightInTiles = heightInTiles - 1;
                    continue;
                }
                if (widthInTiles == 0)
                    widthInTiles = columns.Length;
                column = 0;
                while (column < widthInTiles)
                    _data.Add(Convert.ToInt32(columns[column++]));
            }

            totalTiles = heightInTiles * widthInTiles;

            tileGraphic = _tiles = Graphic;
            _tileWidth = TileWidth;
            if (_tileWidth == 0)
                _tileWidth = _tiles.Height;
            _tileHeight = TileHeight;
            if (_tileHeight == 0)
                _tileHeight = _tileWidth;

            //create some tile objects that we'll use for overlap checks, one for each tile
            uint i = 0;
            uint l = (uint)(_tiles.Width / _tileWidth) * (uint)(_tiles.Height / _tileHeight);
            //_tileObjects = new List<FlxTile>
            _tileObjects = new FlxTile[l];

            //FlxG.log("size " + l);

            while (i < l)
            {
                //                           (FlxTilemap Tilemap, int Tx, int Ty, int Index, int Width, int Height, Boolean Visible, uint AllowCollisions)
                _tileObjects[i] = new FlxTile(this, 0, 0, (int)i, _tileWidth, _tileHeight, (i >= DrawIndex), (i >= CollideIndex) ? AllowCollisions : None);
                i++;
            }

            

            Width = widthInTiles * _tileWidth;
            Height = heightInTiles * _tileHeight;
            _rects = new Rectangle[totalTiles];

            //create the actual map
            i = 0;
            while (i < totalTiles)
                updateTile((int)i++);




            int ty = 0;
            int tx = 0;
            int index = 0;
            for (ty = 0; ty < heightInTiles; ty++)
            {
                for (tx = 0; tx < widthInTiles; tx++)
                {
                    FlxTile _temp = new FlxTile(this, tx, ty, (int)_data[index], _tileWidth, _tileHeight, (_data[index] >= DrawIndex) ? true : false, (_data[index] >= CollideIndex) ? AllowCollisions : None);
                    _tileObjectsForDrawing.Add(_temp);

                    


                    //if (_data[index] >= CollideIndex)
                    //{
                    //    FlxSprite _newTile = new FlxSprite(tx * _tileWidth, ty * _tileHeight);
                    //    _newTile.makeGraphic(_temp.width, _temp.height, FlxColor.BLUE);
                    //    _newTile.solid = _newTile.immovable = true;
                    //    _temp.solid = true;
                    //    add(_newTile);
                    //}

                    if (index < totalTiles)
                        index++;
                }
            }

            

            return this;
        }

        protected void drawTilemap(FlxCamera Camera)
        {
            //no need because draw() actually renders the tilemap
        }

        public override void draw()
        {
            foreach (FlxTile t in _tileObjectsForDrawing)
            {
                //t.tileGraphicSectionToDraw = new Rectangle((int)t.index * (int)t.tileSize, 0, (int)t.width, (int)t.height);
                //if (t.visible)
                //    FlxG.spriteBatch.Draw(_tiles, t.drawPosition, t.tileGraphicSectionToDraw, FlxColor.WHITE);
                t.draw();
            }
            //foreach (FlxSprite s in members)
            //{
            //    s.draw();
            //}
        }

        // getData

        // setDirty

        // findPath

        // simplifyPath

        // raySimplifyPath

        // computePathDistance

        // walkPath

        override public Boolean overlaps(FlxBasic objectOrGroup, Boolean inScreenSpace = false, FlxCamera camera = null)
        {
            if (objectOrGroup is FlxGroup)
            {
                Boolean results = false;
                FlxBasic basic;
                int i = 0;
                List<FlxBasic> members = new List<FlxBasic>();
                members = (objectOrGroup as FlxGroup).Members;
                while (i < members.Count)
                {
                    basic = members[i++] as FlxBasic;
                    if (basic is FlxObject)
                    {
                        if (overlapsWithCallback(basic as FlxObject))
                        results = true;
                    }
                    else
                    {
                        if (overlaps(basic, inScreenSpace, camera))
                            results = true;
                    }
                }
                return results;
            }
            else if (objectOrGroup is FlxObject)
            {
                return overlapsWithCallback(objectOrGroup as FlxObject);
            }
            return false;
        }

        // overlapsWithCallBack
        public Boolean overlapsWithCallback(FlxObject Object, Func<FlxObject, FlxObject, Boolean> Callback=null, Boolean FlipCallbackParams=false, FlxPoint Position=null)
        {
            Boolean results = false;

            float X = base.X;
            float Y = base.Y;
            if (Position != null)
            {
                X = Position.X;
                Y = Position.X;
            }

            //Figure out what tiles we need to check against
            int selectionX = (int)FlxU.floor((Object.X - X) / _tileWidth);
            int selectionY = (int)FlxU.floor((Object.Y - Y) / _tileHeight);
            uint selectionWidth = (uint)(selectionX + (FlxU.ceil((int)Object.Width / _tileWidth)) + 2);
            uint selectionHeight = (uint)(selectionY + (FlxU.ceil((int)Object.Height / _tileHeight)) + 2);

            //Then bound these coordinates by the map edges
            if (selectionX < 0)
                selectionX = 0;
            if (selectionY < 0)
                selectionY = 0;
            if (selectionWidth > widthInTiles)
                selectionWidth = (uint)widthInTiles;
            if (selectionHeight > heightInTiles)
                selectionHeight = (uint)heightInTiles;

            //Then loop through this selection of tiles and call FlxObject.separate() accordingly
            uint rowStart = (uint)selectionY * (uint)widthInTiles;
            uint row = (uint)selectionY;
            uint column;
            FlxTile tile;
            Boolean overlapFound;
            float deltaX = X - Last.X;
            float deltaY = Y - Last.Y;

           

            while (row < selectionHeight)
            {
                column = (uint)selectionX;
                while (column < selectionWidth)
                {
                    overlapFound = false;
                    tile = _tileObjects[(int)_data[(int)(rowStart+column)]] as FlxTile;
                    if ( Convert.ToBoolean(tile.AllowCollisions) )
                    {
                        tile.X = X + (int)column * _tileWidth;
                        tile.Y = Y + (int)row * _tileHeight;
                        tile.Last.X = tile.X - deltaX;
                        tile.Last.Y = tile.Y - deltaY;
                        if (Callback != null)
                        {
                            if (FlipCallbackParams)
                                overlapFound = Callback(Object, tile);
                            else
                                overlapFound = Callback(tile, Object);
                        }
                        else
                        {
                            overlapFound = (Object.X + Object.Width > tile.X) && (Object.X < tile.X + tile.Width) && (Object.Y + Object.Height > tile.Y) && (Object.Y < tile.Y + tile.Height);
                        }
                        if (overlapFound)
                        {
                            if ((tile.callback != null))
                            {
                                tile.mapIndex = (uint)rowStart + column;
                                tile.callback(tile, Object);
                            }
                            results = true;
                        }
                    }
                    else if ((tile.callback != null))
                    {
                        tile.mapIndex = (uint)rowStart + (uint)column;
                        tile.callback(tile, Object);
                    }
                    column++;
                }
                rowStart += (uint)widthInTiles;
                row++;
            }
            return results;
        }


        // overlapsPoint
        override public Boolean overlapsPoint(FlxPoint point, Boolean inScreenSpace = false, FlxCamera camera = null)
        {
            if (!inScreenSpace)
                return (_tileObjects[_data[(int)(((point.Y - Y) / _tileHeight) * widthInTiles + (point.X - X) / _tileWidth)]] as FlxTile).AllowCollisions > 0;

            if (camera == null)
                camera = FlxG.camera;
            point.X = point.X - camera.Scroll.X;
            point.Y = point.Y - camera.Scroll.Y;
            getScreenXY(_tagPoint, camera);
            return (_tileObjects[_data[(int)(((point.Y - _tagPoint.Y) / _tileHeight) * widthInTiles + (point.X - _tagPoint.X) / _tileWidth)]] as FlxTile).AllowCollisions > 0;
        }


        protected void updateTile(int Index)
        {
            FlxTile tile = _tileObjects[_data[Index]] as FlxTile;
            if ((tile == null) || !tile.Visible)
            {
                _rects[Index] = Rectangle.Empty;
                return;
            }
            int rx = (int)(_data[Index] - _startingIndex) * _tileWidth;
            int ry = 0;
            if (rx >= _tiles.Width)
            {
                ry = (rx / _tiles.Width) * _tileHeight;
                rx %= _tiles.Width;
            }
            _rects[Index] = (new Rectangle(rx, ry, _tileWidth, _tileHeight));
        }



        public void follow(FlxCamera Camera = null, int Border = 0, Boolean UpdateWorld = false)
        {
            if (Camera == null)
                Camera = FlxG.camera;
            Camera.setBounds(X + Border * tileSize, Y + Border * tileSize, Width - Border * tileSize * 2, Height - Border * tileSize * 2, UpdateWorld);
        }

        public int getTile(int X, int Y)
        {
            int i = (Y * widthInTiles + X);
            return (int)_data[i];
        }


        public void setTileProperties(uint Tile, uint AllowCollisions = 0x1111, Action<FlxTile, FlxObject> Callback = null, uint Range = 1)
        {
            if (Range <= 0)
                Range = 1;
            FlxTile tile;
            uint i = Tile;
            uint l = Tile + Range;
            while (i < l)
            {
                tile = _tileObjects[(int)i++] as FlxTile;
                tile.AllowCollisions = AllowCollisions;
                tile.callback = Callback;
            }
        }

        /**
		 * Find a path through the tilemap.  Any tile with any collision flags set is treated as impassable.
		 * If no path is discovered then a null reference is returned.
		 * 
		 * @param	Start		The start point in world coordinates.
		 * @param	End			The end point in world coordinates.
		 * @param	Simplify	Whether to run a basic simplification algorithm over the path data, removing extra points that are on the same line.  Default value is true.
		 * @param	RaySimplify	Whether to run an extra raycasting simplification algorithm over the remaining path data.  This can result in some close corners being cut, and should be used with care if at all (yet).  Default value is false.
		 * 
		 * @return	A <code>FlxPath</code> from the start to the end.  If no path could be found, then a null reference is returned.
		 */
		public FlxPath findPath(FlxPoint Start, FlxPoint End, bool Simplify=true, bool RaySimplify=false)
		{
			//figure out what tile we are starting and ending on.
			int startIndex = (int)((Start.Y-Y)/_tileHeight) * widthInTiles + (int)((Start.X-X)/_tileWidth);
			int endIndex = (int)((End.Y-Y)/_tileHeight) * widthInTiles + (int)((End.X-X)/_tileWidth);

			//check that the start and end are clear.
			if( ((_tileObjects[_data[startIndex]] as FlxTile).AllowCollisions > 0) ||
				((_tileObjects[_data[endIndex]] as FlxTile).AllowCollisions > 0) )
				return null;

			//figure out how far each of the tiles is from the starting tile
			List<int> distances = computePathDistance(startIndex,endIndex);
			if(distances == null)
				return null;

			//then count backward to find the shortest path.
			List<FlxPoint> points = new List<FlxPoint>();
			walkPath(distances,endIndex,points);

			//reset the start and end points to be exact
			FlxPoint node;
			node = points[points.Count-1] as FlxPoint;
			node.X = Start.X;
			node.Y = Start.Y;
			node = points[0] as FlxPoint;
			node.X = End.X;
			node.Y = End.Y;

			//some simple path cleanup options
			if(Simplify)
				simplifyPath(points);
			if(RaySimplify)
				raySimplifyPath(points);

			//finally load the remaining points into a new path object and return it
			FlxPath path = new FlxPath();
			int i = points.Count - 1;
			while(i >= 0)
			{
				node = points[i--] as FlxPoint;
				if(node != null)
					path.AddPoint(node,true);
			}
			return path;
		}

        /**
		 * Pathfinding helper function, strips out extra points on the same line.
		 *
		 * @param	Points		An array of <code>FlxPoint</code> nodes.
		 */
		protected void simplifyPath(List<FlxPoint> Points)
		{
			float deltaPrevious;
			float deltaNext;
			FlxPoint last = Points[0];
			FlxPoint node;
			int i = 1;
			int l = Points.Count-1;
			while(i < l)
			{
				node = Points[i];
				deltaPrevious = (node.X - last.X)/(node.Y - last.Y);
				deltaNext = (node.X - Points[i+1].X)/(node.Y - Points[i+1].Y);
				if((last.X == Points[i+1].X) || (last.Y == Points[i+1].Y) || (deltaPrevious == deltaNext))
					Points[i] = null;
				else
					last = node;
				i++;
			}
		}

        /**
		 * Pathfinding helper function, strips out even more points by raycasting from one point to the next and dropping unnecessary points.
		 * 
		 * @param	Points		An array of <code>FlxPoint</code> nodes.
		 */
		protected void raySimplifyPath(List<FlxPoint> Points)
		{
			FlxPoint source = Points[0];
			int lastIndex = -1;
			FlxPoint node;
			int i = 1;
			int l = Points.Count;
			while(i < l)
			{
				node = Points[i++];
				if(node == null)
					continue;
				if(ray(source,node,_tagPoint))	
				{
					if(lastIndex >= 0)
						Points[lastIndex] = null;
				}
				else
					source = Points[lastIndex];
				lastIndex = i-1;
			}
		}

        /**
		 * Pathfinding helper function, floods a grid with distance information until it finds the end point.
		 * NOTE: Currently this process does NOT use any kind of fancy heuristic!  It's pretty brute.
		 * 
		 * @param	StartIndex	The starting tile's map index.
		 * @param	EndIndex	The ending tile's map index.
		 * 
		 * @return	A Flash <code>Array</code> of <code>FlxPoint</code> nodes.  If the end tile could not be found, then a null <code>Array</code> is returned instead.
		 */
        protected List<int> computePathDistance(int StartIndex, int EndIndex)
		{
			//Create a distance-based representation of the tilemap.
			//All walls are flagged as -2, all open areas as -1.
			int mapSize = widthInTiles*heightInTiles;
            List<int> distances = new List<int>(mapSize);
			int i = 0;
			while(i < mapSize)
			{
				if(Convert.ToBoolean((_tileObjects[_data[i]] as FlxTile).AllowCollisions))
					distances[i] = -2;
				else
					distances[i] = -1;
				i++;
			}
			distances[StartIndex] = 0;
			int distance = 1;
			List<int> neighbors = new List<int>(StartIndex);
			List<int> current;
			int currentIndex;
			bool left;
			bool right;
			bool up;
			bool down;
			int currentLength;
			bool foundEnd = false;
			while(neighbors.Count > 0)
			{
				current = neighbors;
				neighbors = new List<int>();

				i = 0;
				currentLength = current.Count;
				while(i < currentLength)
				{
					currentIndex = current[i++];
					if(currentIndex == EndIndex)
					{
						foundEnd = true;
						//neighbors.Count = 0;
						break;
					}

					//basic map bounds
					left = currentIndex%widthInTiles > 0;
					right = currentIndex%widthInTiles < widthInTiles-1;
					up = currentIndex/widthInTiles > 0;
					down = currentIndex/widthInTiles < heightInTiles-1;

					int index;
					if(up)
					{
						index = currentIndex - widthInTiles;
						if(distances[index] == -1)
						{
							distances[index] = distance;
							neighbors.Add(index);
						}
					}
					if(right)
					{
						index = currentIndex + 1;
						if(distances[index] == -1)
						{
							distances[index] = distance;
							neighbors.Add(index);
						}
					}
					if(down)
					{
						index = currentIndex + widthInTiles;
						if(distances[index] == -1)
						{
							distances[index] = distance;
                            neighbors.Add(index);
						}
					}
					if(left)
					{
						index = currentIndex - 1;
						if(distances[index] == -1)
						{
							distances[index] = distance;
                            neighbors.Add(index);
						}
					}
					if(up && right)
					{
						index = currentIndex - widthInTiles + 1;
						if((distances[index] == -1) && (distances[currentIndex-widthInTiles] >= -1) && (distances[currentIndex+1] >= -1))
						{
							distances[index] = distance;
                            neighbors.Add(index);
						}
					}
					if(right && down)
					{
						index = currentIndex + widthInTiles + 1;
						if((distances[index] == -1) && (distances[currentIndex+widthInTiles] >= -1) && (distances[currentIndex+1] >= -1))
						{
							distances[index] = distance;
                            neighbors.Add(index);
						}
					}
					if(left && down)
					{
						index = currentIndex + widthInTiles - 1;
						if((distances[index] == -1) && (distances[currentIndex+widthInTiles] >= -1) && (distances[currentIndex-1] >= -1))
						{
							distances[index] = distance;
                            neighbors.Add(index);
						}
					}
					if(up && left)
					{
						index = currentIndex - widthInTiles - 1;
						if((distances[index] == -1) && (distances[currentIndex-widthInTiles] >= -1) && (distances[currentIndex-1] >= -1))
						{
							distances[index] = distance;
                            neighbors.Add(index);
						}
					}
				}
				distance++;
			}
			if(!foundEnd)
				distances = null;
			return distances;
		}

        /**
		 * Pathfinding helper function, recursively walks the grid and finds a shortest path back to the start.
		 * 
		 * @param	Data	A Flash <code>Array</code> of distance information.
		 * @param	Start	The tile we're on in our walk backward.
		 * @param	Points	A Flash <code>Array</code> of <code>FlxPoint</code> nodes composing the path from the start to the end, compiled in reverse order.
		 */
		protected void walkPath(List<int> Data, int Start, List<FlxPoint> Points)
		{
            Points.Add(new FlxPoint((int)X + (int)(Start % widthInTiles) * _tileWidth + _tileWidth * 0.5f, (int)Y + (int)(Start / widthInTiles) * _tileHeight + _tileHeight * 0.5f));
			if(Data[Start] == 0)
				return;

			//basic map bounds
			bool left = Start%widthInTiles > 0;
			bool right = Start%widthInTiles < widthInTiles-1;
			bool up = Start/widthInTiles > 0;
			bool down = Start/widthInTiles < heightInTiles-1;

			int current = Data[Start];
			int i;
			if(up)
			{
				i = Start - widthInTiles;
				if((Data[i] >= 0) && (Data[i] < current))
				{
					walkPath(Data,i,Points);
					return;
				}
			}
			if(right)
			{
				i = Start + 1;
				if((Data[i] >= 0) && (Data[i] < current))
				{
					walkPath(Data,i,Points);
					return;
				}
			}
			if(down)
			{
				i = Start + widthInTiles;
				if((Data[i] >= 0) && (Data[i] < current))
				{
					walkPath(Data,i,Points);
					return;
				}
			}
			if(left)
			{
				i = Start - 1;
				if((Data[i] >= 0) && (Data[i] < current))
				{
					walkPath(Data,i,Points);
					return;
				}
			}
			if(up && right)
			{
				i = Start - widthInTiles + 1;
				if((Data[i] >= 0) && (Data[i] < current))
				{
					walkPath(Data,i,Points);
					return;
				}
			}
			if(right && down)
			{
				i = Start + widthInTiles + 1;
				if((Data[i] >= 0) && (Data[i] < current))
				{
					walkPath(Data,i,Points);
					return;
				}
			}
			if(left && down)
			{
				i = Start + widthInTiles - 1;
				if((Data[i] >= 0) && (Data[i] < current))
				{
					walkPath(Data,i,Points);
					return;
				}
			}
			if(up && left)
			{
				i = Start - widthInTiles - 1;
				if((Data[i] >= 0) && (Data[i] < current))
				{
					walkPath(Data,i,Points);
					return;
				}
			}
		}


        /**
		 * Shoots a ray from the start point to the end point.
		 * If/when it passes through a tile, it stores that point and returns false.
		 * 
		 * @param	Start		The world coordinates of the start of the ray.
		 * @param	End			The world coordinates of the end of the ray.
		 * @param	Result		A <code>Point</code> object containing the first wall impact.
		 * @param	Resolution	Defaults to 1, meaning check every tile or so.  Higher means more checks!
		 * @return	Returns true if the ray made it from Start to End without hitting anything.  Returns false and fills Result if a tile was hit.
		 */
		public bool ray(FlxPoint Start, FlxPoint End, FlxPoint Result=null, float Resolution=1f)
		{
			float step = _tileWidth;
			if(_tileHeight < _tileWidth)
				step = _tileHeight;
			step /= Resolution;
			float deltaX = End.X - Start.X;
			float deltaY = End.Y - Start.Y;
			float distance = (float)Math.Sqrt(deltaX*deltaX + deltaY*deltaY);
			int steps = (int)FlxU.ceil(distance/step);
			float stepX = deltaX/steps;
			float stepY = deltaY/steps;
			float curX = Start.X - stepX - X;
			float curY = Start.Y - stepY - Y;
			int tileX;
			int tileY;
			int i = 0;
			while(i < steps)
			{
				curX += stepX;
				curY += stepY;

				if((curX < 0) || (curX > Width) || (curY < 0) || (curY > Height))
				{
					i++;
					continue;
				}

				tileX = (int)curX/_tileWidth;
				tileY = (int)curY/_tileHeight;
				if( Convert.ToBoolean((_tileObjects[_data[tileY*widthInTiles+tileX]] as FlxTile).AllowCollisions))
				{
					//Some basic helper stuff
					tileX *= _tileWidth;
					tileY *= _tileHeight;
					float rx = 0;
					float ry = 0;
					float q;
					float lx = curX-stepX;
					float ly = curY-stepY;

					//Figure out if it crosses the X boundary
					q = tileX;
					if(deltaX < 0)
						q += _tileWidth;
					rx = q;
					ry = ly + stepY*((q-lx)/stepX);
					if((ry > tileY) && (ry < tileY + _tileHeight))
					{
						if(Result == null)
							Result = new FlxPoint();
						Result.X = rx;
						Result.Y = ry;
						return false;
					}

					//Else, figure out if it crosses the Y boundary
					q = tileY;
					if(deltaY < 0)
						q += _tileHeight;
					rx = lx + stepX*((q-ly)/stepY);
					ry = q;
					if((rx > tileX) && (rx < tileX + _tileWidth))
					{
						if(Result == null)
							Result = new FlxPoint();
						Result.X = rx;
						Result.Y = ry;
						return false;
					}
					return true;
				}
				i++;
			}
			return true;
		}

    }
}

