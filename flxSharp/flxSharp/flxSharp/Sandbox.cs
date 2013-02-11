using Microsoft.Xna.Framework;
using flxSharp.flxSharp;
using Microsoft.Xna.Framework.Graphics;

namespace flxSharp
{
    public class Sandbox : FlxState
    {
        private FlxSprite defaultLogo;
        private FlxSprite block;
        private FlxSprite block2;
        private FlxSprite block3;
        private FlxSprite block4;

        private FlxSprite background;
        private FlxSprite megaman;

        public override void create()
        {
            base.create();

            //FlxG.addCamera(new FlxCamera(50, 50, 500, 500));

            //defaultLogo = new FlxSprite(FlxG.width / 2, FlxG.height / 2);
            ////defaultLogo.scaling = new FlxPoint(10, 10);
            //this.add(defaultLogo);

            //block = new FlxSprite(0, 0);
            //block.makeGraphic(100, 100, FlxColor.CYAN);
            //this.add(block);

            //block2 = new FlxSprite(100, 0);
            //block2.makeGraphic(100, 100, FlxColor.GREEN);
            //this.add(block2);

            //block3 = new FlxSprite(0, 100);
            //block3.makeGraphic(100, 100, FlxColor.INDIGO);
            //block3.drawLine(0, 0, 99, 99, FlxColor.BLACK, 1);
            //this.add(block3);

            //block4 = new FlxSprite(100, 100);
            //block4.makeGraphic(100, 100, FlxColor.PINK);
            //this.add(block4);

            background = new FlxSprite(0, 0, FlxS.ContentManager.Load<Texture2D>("bg"));
            this.add(background);

            megaman = new FlxSprite(50, 150);
            //megaman.loadGraphic(FlxS.ContentManager.Load<Texture2D>("megaman_run_test"), true, false, 49, 49);
            megaman.loadGraphic(FlxS.ContentManager.Load<Texture2D>("megaman_run"), true, false, 49, 49);
            megaman.addAnimation("run", new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 }, 12);
            megaman.play("run", true);
            this.add(megaman);

            //FlxG.playMusic(FlxS.ContentManager.Load<SoundEffect>("applause"));
            //FlxG.play(FlxS.ContentManager.Load<SoundEffect>("background")); // change content processor to soundeffect

            FlxG.camera.Flash(Color.Black, 2);
        }

        public override void update()
        {
            base.update();

            //block4.Angle += 0.1f;
        }
    }
}
