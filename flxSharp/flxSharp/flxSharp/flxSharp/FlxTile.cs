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
using flxSharp.flxSharp.System;

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
            Immovable = true;
            Moves = false;
            callback = null;

            if (base.Width > base.Height)
                tileSize = base.Width;
            else
                tileSize = base.Height;

            tilemap = Tilemap;
            tileGraphic = tilemap.tileGraphic;
            index = Index;
            ((FlxBasic) this).Visible = Visible;
            base.AllowCollisions = AllowCollisions;

            mapIndex = 0;

            tileGraphicSectionToDraw = new Rectangle((int)index * (int)tileSize, 0, (int)base.Width, (int)base.Height);
            drawPosition = new Vector2(Tx * Width, Ty * Height);

            //if(Convert.ToBoolean(allowCollisions))
            //    FlxG.log("my tile index: " + index +", coll: "+allowCollisions);

            ID = 2;

        }

        public override void draw()
        {
            if (Visible)
            {
                FlxS.SpriteBatch.Draw(tileGraphic, drawPosition, tileGraphicSectionToDraw, FlxColor.WHITE);                
            }
        }
    }
}
