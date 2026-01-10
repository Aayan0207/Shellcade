using System;

namespace ArcadeProject.Utility
{
    public static class Menu
    {
        public static int Run()
        {
            Console.CursorVisible = false;
            string[] menuItems = ["Sudoku", "Nim", "Hangman", "Mastermind", "TicTacToe", "Minesweeper", "Buckshot Roulette"];
            ConsoleColor[] colors = [ConsoleColor.Blue, ConsoleColor.Yellow, ConsoleColor.Green, ConsoleColor.Cyan, ConsoleColor.DarkGreen, ConsoleColor.Red, ConsoleColor.Magenta];
            int choice = 1;
            bool flag = false;
            while (!flag)
            {
                Console.Clear();
                Console.ResetColor();
                Console.WriteLine("   _____ __         __________          __   \n  / ___// /_  ___  / / / ____/___ _____/ /__ \n  \\__ \\/ __ \\/ _ \\/ / / /   / __ `/ __  / _ \\\n ___/ / / / /  __/ / / /___/ /_/ / /_/ /  __/\n/____/_/ /_/\\___/_/_/\\____/\\__,_/\\__,_/\\___/ \n                                             \n");
                for (int item = 0; item < menuItems.Length; item++)
                {
                    Console.ForegroundColor = colors[item];
                    if (item == choice - 1)
                    {
                        Console.WriteLine($"◉ {menuItems[item]}");
                    }
                    else
                    {
                        Console.WriteLine($"◎ {menuItems[item]}");
                    }
                }
                ConsoleKey key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.UpArrow && choice > 1)
                {
                    choice -= 1;
                }
                else if (key == ConsoleKey.DownArrow && choice < menuItems.Length)
                {
                    choice += 1;
                }
                else if (key == ConsoleKey.Enter && choice != 0)
                {
                    flag = true;
                }
            }
            return choice;
        }
    }
}