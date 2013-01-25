using System;
using fliXNA_xbox;

namespace flxSharp
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (var flxGame = new FlxGame(new Sandbox()))
            {
                flxGame.Run();
            }
        }
    }
#endif
}

