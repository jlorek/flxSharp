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

namespace fliXNA_xbox
{
    public class FlxTile : FlxObject
    {
        public Texture2D tileGraphic;
        public Rectangle tileGraphicSectionToDraw;
        public float tileSize;
        public int index;
        public Action<FlxTile, FlxObject> callback;
        public uint mapIndex;
        public FlxTilemap tilemap;
        public Vector2 drawPosition;

        public FlxTile(FlxTilemap Tilemap, int Tx, int Ty, int Index, int Width, int Height, Boolean Visible, uint AllowCollisions)
            : base(0, 0, Width, Height)
        {
            immovable = true;
            moves = false;
            callback = null;

            if (width > height)
                tileSize = width;
            else
                tileSize = height;

            tilemap = Tilemap;
            tileGraphic = tilemap.tileGraphic;
            index = Index;
            visible = Visible;
            allowCollisions = AllowCollisions;

            mapIndex = 0;

            tileGraphicSectionToDraw = new Rectangle((int)index * (int)tileSize, 0, (int)width, (int)height);
            drawPosition = new Vector2(Tx * Width, Ty * Height);

            //if(Convert.ToBoolean(allowCollisions))
            //    FlxG.log("my tile index: " + index +", coll: "+allowCollisions);

            ID = 2;

        }

        public override void draw()
        {
            if (visible)
                FlxG.spriteBatch.Draw(tileGraphic, drawPosition, tileGraphicSectionToDraw, FlxColor.WHITE);
        }
    }
}
