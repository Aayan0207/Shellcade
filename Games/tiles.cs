using System;

namespace ArcadeProject.Games
{
    public static class Tiles
    {
        const char tile = '◼';
        public static void Run()
        {
            Console.Clear();
            Console.ResetColor();
            Console.CursorVisible = false;
            int[,] board = new int[4, 4];
            Random_Tile(board);
            Random_Tile(board);
            while (!Terminal(board))
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  _______ __         \n /_  __(_) /__  _____\n  / / / / / _ \\/ ___/\n / / / / /  __(__  ) \n/_/ /_/_/\\___/____/  \n                     \n");
                Console.ResetColor();
                Display(board);
                int[,] copy = Copy(board);
                Console.WriteLine("Enter your next move (Arrow Keys)..");
                ConsoleKey key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.RightArrow:
                        Shift(board, 'R');
                        break;
                    case ConsoleKey.LeftArrow:
                        Shift(board, 'L');
                        break;
                    case ConsoleKey.UpArrow:
                        Shift(board, 'U');
                        break;
                    case ConsoleKey.DownArrow:
                        Shift(board, 'D');
                        break;
                    default:
                        continue;
                }
                if (!Boards_Equals(board, copy))
                {
                    Random_Tile(board);
                }
            }
        }

        public static bool Boards_Equals(int[,] board, int[,] copy)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (board[i, j] != copy[i, j])
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public static int[,] Copy(int[,] board)
        {
            int[,] copy = new int[4, 4];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    copy[i, j] = board[i, j];
                }
            }
            return copy;
        }
        public static void Random_Tile(int[,] board)
        {
            var possibles = new List<(int x, int y)>();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (board[i, j] == 0)
                    {
                        possibles.Add((i, j));
                    }
                }
            }

            if (possibles.Count == 0)
            {
                return;
            }
            int random_index = Random.Shared.Next(possibles.Count);
            int roll = Random.Shared.NextDouble() < 0.9 ? 2 : 4;
            board[possibles[random_index].x, possibles[random_index].y] = roll;
        }
        public static void Display(int[,] board)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    switch (board[i, j])
                    {
                        case 0:
                            Console.ForegroundColor = ConsoleColor.Black;
                            break;
                        case 2:
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                        case 4:
                            Console.ForegroundColor = ConsoleColor.Blue;
                            break;
                        case 8:
                            Console.ForegroundColor = ConsoleColor.Green;
                            break;
                        case 16:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            break;
                        case 32:
                            Console.ForegroundColor = ConsoleColor.Red;
                            break;
                        case 64:
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            break;
                        case 128:
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            break;
                        case 256:
                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                            break;
                        case 512:
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            break;
                        case 1024:
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            break;
                        case 2048:
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            break;

                    }
                    //White -> 2, Blue -> 4, Green -> 8, Yellow -> 16, Red -> 32, Magenta -> 64, Cyan -> 128, Grey -> 256, DarkBlue -> 512, DarkGreen -> 1024, DarkGrey -> 2048
                    Console.Write($"{tile} ");
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
        }


        public static void Shift_Zeroes(int[,] board)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int pass = 0; pass < 3; pass++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (board[i, j] == 0 && board[i, j + 1] != 0)
                        {
                            (board[i, j], board[i, j + 1]) = (board[i, j + 1], board[i, j]);
                        }
                    }
                }
            }
        }

        public static void Merge(int[,] board)
        {

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j] == board[i, j + 1] && board[i, j] != 0)
                    {
                        board[i, j] *= 2;
                        board[i, j + 1] = 0;
                        j++;
                    }
                }
            }
        }

        public static void Reverse(int[,] board)
        {
            for (int i = 0; i < 4; i++)
            {
                int L = 0, R = 3;
                while (L < R)
                {
                    (board[i, L], board[i, R]) = (board[i, R], board[i, L]);
                    L++;
                    R--;
                }
            }
        }

        public static void Transpose(int[,] board)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = i + 1; j < 4; j++)
                {
                    (board[i, j], board[j, i]) = (board[j, i], board[i, j]);
                }
            }
        }
        public static void Shift(int[,] board, char move)
        {
            switch (move)
            {
                case 'R':
                    Reverse(board);
                    Shift_Zeroes(board);
                    Merge(board);
                    Shift_Zeroes(board);
                    Reverse(board);
                    break;
                case 'L':
                    Shift_Zeroes(board);
                    Merge(board);
                    Shift_Zeroes(board);
                    break;
                case 'U':
                    Transpose(board);
                    Shift_Zeroes(board);
                    Merge(board);
                    Shift_Zeroes(board);
                    Transpose(board);
                    break;
                case 'D':
                    Transpose(board);
                    Reverse(board);
                    Shift_Zeroes(board);
                    Merge(board);
                    Shift_Zeroes(board);
                    Reverse(board);
                    Transpose(board);
                    break;
            }
        }

        public static bool Terminal(int[,] board)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (board[i, j] == 0)
                    {
                        return false;
                    }
                    if (j < 3 && board[i, j] == board[i, j + 1])
                    {
                        return false;
                    }
                    if (i < 3 && board[i, j] == board[i + 1, j])
                    {
                        return false;
                    }
                    if (board[i, j] == 2048)
                    {
                        Console.WriteLine("You win!");
                        return true;
                    }
                }
            }
            return true;
        }
    }
}