using fliXNA_xbox;

namespace flxSharp
{
    public class Sandbox : FlxState
    {
        public override void create()
        {
            base.create();

            var defaultLogo = new FlxSprite(FlxG.width / 2, FlxG.height / 2);
            this.add(defaultLogo);
        }
    }
}
