using System;

namespace ArcadeProject.Utility
{
    public static class Menu
    {
        public static void Run()
        {
            string[] menuItems = ["Pong", "Dino Runner", "Hangman"];
            int choice = 0;
            bool flag = false;
            while (!flag)
            {
                Console.Clear();
                Console.WriteLine("Welcome to the Arcade");
                for (int item = 0; item < menuItems.Length; item++)
                {
                    if (item == choice)
                    {
                        Console.WriteLine($"◉ {menuItems[item]}");
                    }
                    else
                    {
                        Console.WriteLine($"{menuItems[item]}");
                    }
                }
                ConsoleKey key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.UpArrow && choice > 0)
                {
                    choice -= 1;
                }
                else if (key == ConsoleKey.DownArrow && choice < menuItems.Length-1)
                {
                    choice += 1;
                }
                else if (key == ConsoleKey.Enter && choice != 0)
                {
                    flag = true;
                }
            }
        }
    }
}