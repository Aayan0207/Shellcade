using System;

namespace ArcadeProject.Utility
{
    public static class Menu
    {
        public static void Run()
        {
            string[] menuItems = ["1. Pong", "2. Dino Runner", "3. Hangman"];
            int choice;
            Console.WriteLine("Welcome to the Arcade");
            foreach (string item in menuItems)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine("Enter choice: ");
            choice = int.Parse(Console.ReadLine());
        }
    }
}