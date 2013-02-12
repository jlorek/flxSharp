using flxSharp.flxSharp.System;

namespace flxSharp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (var gameRunner = new FlxGameRunner(320, 240, new Sandbox(), 3.0f))
            {
                gameRunner.Run();
            }
        }
    }
}

