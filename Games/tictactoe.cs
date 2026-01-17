using System;
using System.IO.Pipelines;
using System.Reflection.Metadata;

namespace ArcadeProject.Games
{
    public static class TicTacToe
    {
        const char X = 'X';
        const char O = 'O';
        public static void Run()
        {
            Console.Clear();
            Console.ResetColor();
            char[,] board = { { ' ', ' ', ' ' }, { ' ', ' ', ' ' }, { ' ', ' ', ' ' } };
            Console.CursorVisible = true;
            Console.Write("Play as X or O? ");
            char player = char.ToUpper(Console.ReadLine()[0]);
            if (player == O)
            {
                int[] random_move = RandomAIMove();
                board = Result(board, random_move[0], random_move[1]);
            }
            while (!Terminal(board))
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("  _______     ______          ______         \n /_  __(_)___/_  __/___ _____/_  __/___  ___ \n  / / / / ___// / / __ `/ ___// / / __ \\/ _ \\\n / / / / /__ / / / /_/ / /__ / / / /_/ /  __/\n/_/ /_/\\___//_/  \\__,_/\\___//_/  \\____/\\___/ \n                                             \n");
                Console.ResetColor();
                Display(board);
                if (Player(board) == player)
                {
                    int row, column;
                    Console.Write("Enter row: ");
                    row = int.Parse(Console.ReadLine());
                    Console.Write("Enter column: ");
                    column = int.Parse(Console.ReadLine());
                    if (ValidMove(board, row, column))
                    {
                        board = Result(board, row, column);
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    int[] ai_move = Minimax(board);
                    board = Result(board, ai_move[0], ai_move[1]);
                }
            }
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  _______     ______          ______         \n /_  __(_)___/_  __/___ _____/_  __/___  ___ \n  / / / / ___// / / __ `/ ___// / / __ \\/ _ \\\n / / / / /__ / / / /_/ / /__ / / / /_/ /  __/\n/_/ /_/\\___//_/  \\__,_/\\___//_/  \\____/\\___/ \n                                             \n");
            Console.ResetColor();
            Display(board);
            if (Winner(board) != 0)
            {
                if (Winner(board) == player)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Congratulations! You won!");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Sorry! AI wins!");
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("It's a draw.");
            }
            Console.ResetColor();
        }

        public static int[] Minimax(char[,] board)
        {
            if (Terminal(board))
            {
                return [];
            }
            return Player(board) == X ? MaxValue(board)[..2] : MinValue(board);
        }

        public static int[] MaxValue(char[,] board)
        {
            if (Terminal(board))
            {
                return [0, 0, Winner(board) == X ? 1 : Winner(board) == O ? -1 : 0];
            }
            int[] move = new int[2];
            int check = -2;
            int[,] moves = Moves(board);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (moves[i, j] == 1)
                    {
                        int minimum = MinValue(Result(board, i, j))[2];
                        if (minimum > check)
                        {
                            move = [i, j];
                            check = minimum;
                        }
                    }
                }
            }
            return [move[0], move[1], check];
        }

        public static int[] MinValue(char[,] board)
        {
            if (Terminal(board))
            {
                return [0, 0, Winner(board) == X ? 1 : Winner(board) == O ? -1 : 0];
            }
            int[] move = new int[2];
            int check = 2;
            int[,] moves = Moves(board);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (moves[i, j] == 1)
                    {
                        int maximum = MaxValue(Result(board, i, j))[2];
                        if (maximum < check)
                        {
                            move = [i, j];
                            check = maximum;
                        }
                    }
                }
            }
            return [move[0], move[1], check];
        }

        public static int Winner(char[,] board)
        {
            if ((board[0, 0] == X && board[1, 0] == X && board[2, 0] == X) || (board[0, 0] == O && board[1, 0] == O && board[2, 0] == O))
            {
                return board[0, 0];
            }
            else if ((board[0, 1] == X && board[1, 1] == X && board[2, 1] == X) || (board[0, 1] == O && board[1, 1] == O && board[2, 1] == O))
            {
                return board[0, 1];
            }
            else if ((board[0, 2] == X && board[1, 2] == X && board[2, 2] == X) || (board[0, 2] == O && board[1, 2] == O && board[2, 2] == O))
            {
                return board[0, 2];
            }
            else if ((board[0, 0] == X && board[0, 1] == X && board[0, 2] == X) || (board[0, 0] == O && board[0, 1] == O && board[0, 2] == O))
            {
                return board[0, 0];
            }
            else if ((board[1, 0] == X && board[1, 1] == X && board[1, 2] == X) || (board[1, 0] == O && board[1, 1] == O && board[1, 2] == O))
            {
                return board[1, 0];
            }
            else if ((board[2, 0] == X && board[2, 1] == X && board[2, 2] == X) || (board[2, 0] == O && board[2, 1] == O && board[2, 2] == O))
            {
                return board[2, 0];
            }
            else if ((board[0, 0] == X && board[1, 1] == X && board[2, 2] == X) || (board[0, 0] == O && board[1, 1] == O && board[2, 2] == O))
            {
                return board[0, 0];
            }
            else if ((board[0, 2] == X && board[1, 1] == X && board[2, 0] == X) || (board[0, 2] == O && board[1, 1] == O && board[2, 0] == O))
            {
                return board[0, 2];
            }
            else
            {
                return 0;
            }
        }
        public static int[,] Moves(char[,] board)
        {
            int[,] moves = new int[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j] == ' ')
                    {
                        moves[i, j] = 1;
                    }
                }
            }
            return moves;
        }
        public static bool Terminal(char[,] board)
        {
            if (Winner(board) != 0)
            {
                return true;
            }
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j] == ' ')
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public static bool ValidMove(char[,] board, int row, int column)
        {
            if (row < 0 || row > 2 || column < 0 || column > 2)
            {
                return false;
            }
            if (board[row, column] == ' ')
            {
                return true;
            }
            return false;
        }
        public static char Player(char[,] board)
        {
            int counter = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j] == ' ')
                    {
                        counter++;
                    }
                }
            }
            return (9 - counter) % 2 == 0 ? X : O;
        }
        public static int[] RandomAIMove()
        {
            return [Random.Shared.Next(3), Random.Shared.Next(3)];
        }

        public static char[,] Result(char[,] board, int row, int column)
        {
            char[,] copy = (char[,])board.Clone();
            copy[row, column] = Player(board);
            return copy;
        }
        public static void Display(char[,] board)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    switch (board[i, j])
                    {
                        case ' ':
                            Console.Write("  ");
                            break;
                        case X:
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write("X ");
                            Console.ResetColor();
                            break;
                        case O:
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.Write("O ");
                            Console.ResetColor();
                            break;
                    }
                }
                Console.WriteLine();
            }
        }
    }
}