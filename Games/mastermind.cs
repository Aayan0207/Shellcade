using System;
using System.Text.RegularExpressions;

namespace ArcadeProject.Games
{
    public class Mastermind
    {
        static readonly ConsoleColor[] COLORS = [ConsoleColor.Blue, ConsoleColor.Green, ConsoleColor.Yellow, ConsoleColor.Red, ConsoleColor.White, ConsoleColor.Magenta];
        const char PEG = '◉';
        public static void Run()
        {
            Console.Clear();
            Console.ResetColor();
            Console.CursorVisible = true;
            ConsoleColor[] board_colors = Set();
            string board_string = "";
            foreach (ConsoleColor color in board_colors)
            {
                char letter = ' ';
                switch (color)
                {
                    case ConsoleColor.Blue:
                        letter = 'B';
                        break;
                    case ConsoleColor.Green:
                        letter = 'G';
                        break;
                    case ConsoleColor.Yellow:
                        letter = 'Y';
                        break;
                    case ConsoleColor.Red:
                        letter = 'R';
                        break;
                    case ConsoleColor.White:
                        letter = 'W';
                        break;
                    case ConsoleColor.Magenta:
                        letter = 'M';
                        break;
                }
                board_string += letter;
            }
            string[] guesses = new string[10];
            string[] responses = new string[10];
            int tries = 0;
            while (tries < 10)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("    __  ___           __                      _           __\n   /  |/  /___ ______/ /____  _________ ___  (_)___  ____/ /\n  / /|_/ / __ `/ ___/ __/ _ \\/ ___/ __ `__ \\/ / __ \\/ __  / \n / /  / / /_/ (__  ) /_/  __/ /  / / / / / / / / / / /_/ /  \n/_/  /_/\\__,_/____/\\__/\\___/_/  /_/ /_/ /_/_/_/ /_/\\__,_/   \n                                                            \n");
                Console.ResetColor();
                Console.CursorVisible = false;
                for (int j = 0; j < tries; j++)
                {
                    foreach (char pin in guesses[j])
                    {
                        switch (pin)
                        {
                            case 'B':
                                Console.ForegroundColor = ConsoleColor.Blue;
                                Console.Write(PEG);
                                break;
                            case 'G':
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write(PEG);
                                break;
                            case 'W':
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.Write(PEG);
                                break;
                            case 'Y':
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.Write(PEG);
                                break;
                            case 'M':
                                Console.ForegroundColor = ConsoleColor.Magenta;
                                Console.Write(PEG);
                                break;
                            case 'R':
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write(PEG);
                                break;
                        }
                        Console.Write("\t");
                        Console.ResetColor();
                    }
                    foreach (char response in responses[j])
                    {
                        switch (response)
                        {
                            case 'R':
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("●\t");
                                break;
                            case 'W':
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.Write("●\t");
                                break;
                        }
                        Console.ResetColor();
                    }
                    Console.WriteLine();
                }
                Console.WriteLine("Available Pins: ");
                foreach (ConsoleColor color in COLORS)
                {
                    Console.ForegroundColor = color;
                    Console.Write($"{PEG}\t");
                    Console.ResetColor();
                }
                Console.WriteLine();
                foreach (ConsoleColor color in COLORS)
                {
                    Console.ForegroundColor = color;
                    char letter = ' ';
                    switch (color)
                    {
                        case ConsoleColor.Blue:
                            letter = 'B';
                            break;
                        case ConsoleColor.Green:
                            letter = 'G';
                            break;
                        case ConsoleColor.Yellow:
                            letter = 'Y';
                            break;
                        case ConsoleColor.Red:
                            letter = 'R';
                            break;
                        case ConsoleColor.White:
                            letter = 'W';
                            break;
                        case ConsoleColor.Magenta:
                            letter = 'M';
                            break;
                    }
                    Console.Write($"{letter}\t");
                    Console.ResetColor();
                }
                Console.WriteLine();
                Console.CursorVisible = true;
                Console.Write("Enter your guess: ");
                string input = Console.ReadLine();
                if (Regex.IsMatch(input, @"^[RBGYWM]{4}$", RegexOptions.IgnoreCase))
                {
                    input = input.ToUpper();
                    guesses[tries] = input;
                    responses[tries] = Response(input, board_string);
                    tries += 1;
                }
                else
                {
                    continue;
                }
                if (guesses[tries - 1].ToUpper() == board_string.ToUpper())
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Congratulations! You Won!");
                    Console.ResetColor();
                    break;
                }
            }
            if (tries >= 10)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Sorry, you lost. The pattern was: ");
                Console.ResetColor();
                foreach (ConsoleColor color in board_colors)
                {
                    Console.ForegroundColor = color;
                    Console.Write($"{PEG} ");
                    Console.ResetColor();
                }
            }
        }

        public static string Response(string input, string board)
        {
            string response = "";
            bool[] input_used = new bool[4];
            bool[] board_used = new bool[4];
            for (int i = 0; i < 4; i++)
            {
                if (input[i] == board[i])
                {
                    response += "R";
                    input_used[i] = true;
                    board_used[i] = true;
                }
            }
            for (int i = 0; i < 4; i++)
            {
                if (input_used[i]) continue;
                for (int j = 0; j < 4; j++)
                {
                    if (!board_used[j] && input[i] == board[j])
                    {
                        response += "W";
                        board_used[j] = true;
                        break;
                    }
                }
            }
            return response;
        }
        public static ConsoleColor[] Set()
        {
            ConsoleColor[] set = new ConsoleColor[4];
            Random random = new Random();
            for (int i = 0; i < 4; i++)
            {
                int num = random.Next(COLORS.Length);
                set[i] = COLORS[num];
            }
            return set;
        }
    }
}