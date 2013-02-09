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
            using (var gameRunner = new FlxGameRunner(640, 480, new Sandbox(), 2.0f))
            {
                gameRunner.Run();
            }
        }
    }
}

