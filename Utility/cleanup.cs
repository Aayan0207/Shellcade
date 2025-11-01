using System;

namespace ArcadeProject.Utility
{
    public static class Cleanup
    {
        public static void Run()
        {
            Console.CursorVisible = true;
            Console.ResetColor();
        }
    }
}