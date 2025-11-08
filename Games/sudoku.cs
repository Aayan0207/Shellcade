using System;
using System.Globalization;
using System.Reflection.Metadata;

namespace ArcadeProject.Games
{
    public static class Sudoku
    {
        public static void Run()
        {
            // Console.CursorVisible = false;
            const string BOLD = "\x1b[1m";
            const string RESET = "\x1b[0m";
            Console.Clear();
            Console.ResetColor();
            int mode = 1;
            bool flag = false;
            string[] details = ["Easy", "Medium", "Hard", "Extreme"];
            ConsoleColor[] colors = [ConsoleColor.Green, ConsoleColor.Yellow, ConsoleColor.Red, ConsoleColor.DarkMagenta];
            int[,] board, solved_board;
            while (!flag)
            {
                Console.Clear();
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("   _____           __      __        \n  / ___/__  ______/ /___  / /____  __\n  \\__ \\/ / / / __  / __ \\/ //_/ / / /\n ___/ / /_/ / /_/ / /_/ / ,< / /_/ / \n/____/\\__,_/\\__,_/\\____/_/|_|\\__,_/  \n                                     \n");
                Console.ResetColor();
                Console.WriteLine("Test");
                for (int i = 0; i < details.Length; i++)
                {
                    Console.ForegroundColor = colors[i];
                    if (mode - 1 == i)
                    {
                        Console.WriteLine($"◉{BOLD} {details[i]}{RESET}");
                    }
                    else
                    {
                        Console.WriteLine($"◎{BOLD} {details[i]}{RESET}");
                    }
                    Console.ResetColor();
                }
                ConsoleKey key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.UpArrow && mode > 1)
                {
                    mode -= 1;
                }
                else if (key == ConsoleKey.DownArrow && mode < 4)
                {
                    mode += 1;
                }
                else if (key == ConsoleKey.Enter)
                {
                    flag = true;
                }
            }
            do
            {
                Console.Clear();
                Console.ResetColor();
                // Console.CursorVisible = true;
                board = Board(mode);
                solved_board = SolvedBoard((int[,])board.Clone());
                // Console.SetCursorPosition(0, 0);
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("   _____           __      __        \n  / ___/__  ______/ /___  / /____  __\n  \\__ \\/ / / / __  / __ \\/ //_/ / / /\n ___/ / /_/ / /_/ / /_/ / ,< / /_/ / \n/____/\\__,_/\\__,_/\\____/_/|_|\\__,_/  \n                                     \n");
                Console.ResetColor();
                Console.WriteLine("___________________________\n");
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        if (board[i, j] == 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write(" _ ");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write($" {board[i, j]} ");
                            Console.ResetColor();
                        }
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
                Console.WriteLine("___________________________");
                // for (int i = 0; i < 9; i++)
                // {
                //     for (int j = 0; j < 9; j++)
                //     {
                //         Console.Write($" {solved_board[i, j]} ");
                //     }
                //     Console.WriteLine();
                // }
                break; //temporary
            } while (board != solved_board);
        }
        public static int[,] Board(int mode)
        {
            Random random = new Random();
            int x = random.Next(9), y = random.Next(9), val = random.Next(9);
            int[,] board = new int[9, 9];
            board[x, y] = val;
            board = SolvedBoard(board);
            int removed = 0;
            int[][] cells = [[0, 0], [0, 1], [0, 2], [0, 3], [0, 4], [0, 5], [0, 6], [0, 7], [0, 8], [1, 0], [1, 1], [1, 2], [1, 3], [1, 4], [1, 5], [1, 6], [1, 7], [1, 8], [2, 0], [2, 1], [2, 2], [2, 3], [2, 4], [2, 5], [2, 6], [2, 7], [2, 8], [3, 0], [3, 1], [3, 2], [3, 3], [3, 4], [3, 5], [3, 6], [3, 7], [3, 8], [4, 0], [4, 1], [4, 2], [4, 3], [4, 4], [4, 5], [4, 6], [4, 7], [4, 8], [5, 0], [5, 1], [5, 2], [5, 3], [5, 4], [5, 5], [5, 6], [5, 7], [5, 8], [6, 0], [6, 1], [6, 2], [6, 3], [6, 4], [6, 5], [6, 6], [6, 7], [6, 8], [7, 0], [7, 1], [7, 2], [7, 3], [7, 4], [7, 5], [7, 6], [7, 7], [7, 8], [8, 0], [8, 1], [8, 2], [8, 3], [8, 4], [8, 5], [8, 6], [8, 7], [8, 8]];
            switch (mode)
            {
                case 1:
                    removed = 45;
                    break;
                case 2:
                    removed = 49;
                    break;
                case 3:
                    removed = 53;
                    break;
                case 4:
                    removed = 59;
                    break;
            }
            for (int i = 0; i < cells.Length; i++)
            {
                val = random.Next(i, cells.Length);
                (cells[i], cells[val]) = (cells[val], cells[i]);
            }
            for (int i = 0; i < removed; i++)
            {
                board[cells[i][0], cells[i][1]] = 0;
            }
            return board;
        }

        public static int[,] SolvedBoard(int[,] board)
        {
            List<(int, int)> unknowns = Unknowns(board);
            if (unknowns.Count == 0)
            {
                return board;
            }
            foreach ((int, int) unknown in unknowns)
            {
                List<int> moves = Possibles(board, unknown.Item1, unknown.Item2);
                foreach (int move in moves)
                {
                    board[unknown.Item1, unknown.Item2] = move;
                    if (SolvedBoard(board) != null)
                    {
                        return board;
                    }
                    board[unknown.Item1, unknown.Item2] = 0;
                }
                return null;
            }
            return null;
        }
        public static List<(int, int)> Unknowns(int[,] board)
        {
            List<(int, int)> unknowns = new List<(int, int)>();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (board[i, j] == 0)
                    {
                        unknowns.Add((i, j));
                    }
                }
            }
            return unknowns;
        }

        public static List<int> Possibles(int[,] board, int x, int y)
        {
            HashSet<int> possibles = [1, 2, 3, 4, 5, 6, 7, 8, 9];
            HashSet<int> row = [];
            for (int i = 0; i < 9; i++)
            {
                if (board[x, i] != 0)
                {
                    row.Add(board[x, i]);
                }
            }
            HashSet<int> column = [];
            for (int i = 0; i < 9; i++)
            {
                if (board[i, y] != 0)
                {
                    column.Add(board[i, y]);
                }
            }
            HashSet<int> box = [];
            List<(int, int)> grid1 = [(0, 0), (0, 1), (0, 2), (1, 0), (1, 1), (1, 2), (2, 0), (2, 1), (2, 2)];
            List<(int, int)> grid2 = [(0, 3), (0, 4), (0, 5), (1, 3), (1, 4), (1, 5), (2, 3), (2, 4), (2, 5)];
            List<(int, int)> grid3 = [(0, 6), (0, 7), (0, 8), (1, 6), (1, 7), (1, 8), (2, 6), (2, 7), (2, 8)];
            List<(int, int)> grid4 = [(3, 0), (3, 1), (3, 2), (4, 0), (4, 1), (4, 2), (5, 0), (5, 1), (5, 2)];
            List<(int, int)> grid5 = [(3, 3), (3, 4), (3, 5), (4, 3), (4, 4), (4, 5), (5, 3), (5, 4), (5, 5)];
            List<(int, int)> grid6 = [(3, 6), (3, 7), (3, 8), (4, 6), (4, 7), (4, 8), (5, 6), (5, 7), (5, 8)];
            List<(int, int)> grid7 = [(6, 0), (6, 1), (6, 2), (7, 0), (7, 1), (7, 2), (8, 0), (8, 1), (8, 2)];
            List<(int, int)> grid8 = [(6, 3), (6, 4), (6, 5), (7, 3), (7, 4), (7, 5), (8, 3), (8, 4), (8, 5)];
            List<(int, int)> grid9 = [(6, 6), (6, 7), (6, 8), (7, 6), (7, 7), (7, 8), (8, 6), (8, 7), (8, 8)];
            List<(int, int)>[] grids = [grid1, grid2, grid3, grid4, grid5, grid6, grid7, grid8, grid9];
            foreach (List<(int, int)> grid in grids)
            {
                if (grid.Contains((x, y)))
                {
                    foreach ((int, int) cell in grid)
                    {
                        box.Add(board[cell.Item1, cell.Item2]);
                    }
                }
            }
            possibles.ExceptWith(row);
            possibles.ExceptWith(column);
            possibles.ExceptWith(box);
            return possibles.ToList();
        }
    }
}