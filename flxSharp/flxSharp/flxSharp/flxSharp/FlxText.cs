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
    public class FlxText : FlxSprite
    {
        protected String _textField;
        protected SpriteFont _font;
        protected Boolean _regen;
        public float _scale;
        //protected Action<Color> _color;

        public FlxText(float X, float Y, float Width, String Text = null, SpriteFont Font = null)
            : base(X, Y)
        {
            makeGraphic(Width, 1, FlxColor.BLACK);
            if (Text == null)
                _textField = "";
            else
                _textField = Text;

            if(Font == null)
                _font = FlxG.defaultFont;
            else
                _font = Font;

            _regen = true;
            allowCollisions = NONE;
            calcFrame();
            _scale = 0;
            //_numLines = 0;
        }

        public override void update()
        {
            base.update();
        }

        public override void draw()
        {
            //base.draw(spriteBatch);
            if (visible)
            {
                Vector2 pos = new Vector2(x, y);
                Vector2 orig = new Vector2(0,0);
                Single rot = 0;
                FlxG.spriteBatch.DrawString(

                    FlxG.defaultFont,
                    _textField,
                    pos,
                    (_color * alpha),
                    rot,
                    new Vector2(),
                    _scale,
                    SpriteEffects.None,
                    0

                    );
            }
        }

        public override void destroy()
        {
            _textField = null;
            base.destroy();
        }

        public FlxText setFormat(Color Color, float Scale = 1.0f, SpriteFont Font = null)
        {
            if (Font == null)
                _font = FlxG.defaultFont;
            else
                _font = Font;

            _scale = Scale;

            _regen = true;
            calcFrame();
            return this;
        }

        public String text
        {
            get { return _textField.ToString(); }
            set { _textField = value; }
        }

        public SpriteFont font
        {
            get { return _font; }
            set { _font = value; }
        }



    }
}
