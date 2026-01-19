using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Data.Common;

namespace ArcadeProject.Games
{
    public static class Minesweeper
    {
        const int ROWS = 20;
        const int COLUMNS = 20;
        const int MINES = (int)(ROWS * COLUMNS * 0.15);
        public static void Run()
        {
            Console.Clear();
            Console.ResetColor();
            Console.CursorVisible = true;
            Cell[,] board = Initialize();
            bool first = true;
            while (!Terminal(board))
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("    __  ____                                                   \n   /  |/  (_)___  ___  ______      _____  ___  ____  ___  _____\n  / /|_/ / / __ \\/ _ \\/ ___/ | /| / / _ \\/ _ \\/ __ \\/ _ \\/ ___/\n / /  / / / / / /  __(__  )| |/ |/ /  __/  __/ /_/ /  __/ /    \n/_/  /_/_/_/ /_/\\___/____/ |__/|__/\\___/\\___/ .___/\\___/_/     \n                                           /_/                 \n");
                Console.ResetColor();
                Display(board);
                int row, column;
                Console.Write("Enter row (1-10): ");
                row = int.Parse(Console.ReadLine()) - 1;
                if (row < 0 || row >= ROWS)
                {
                    continue;
                }
                Console.Write("Enter column (1-10): ");
                column = int.Parse(Console.ReadLine()) - 1;
                if (column < 0 || column >= ROWS)
                {
                    continue;
                }
                if (!first && board[row, column].IsRevealed)
                {
                    continue;
                }
                char action = 'Z';
                if (!first)
                {
                    Console.Write("Click (Z) or Flag (X)? ");
                    action = char.ToUpper(Console.ReadLine()[0]);
                    if (action != 'Z' && action != 'X')
                    {
                        continue;
                    }
                }
                if (first)
                {
                    board = SetMines(board, row, column);
                    first = false;
                }
                board = Result(board, row, column, action);
            }
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("    __  ____                                                   \n   /  |/  (_)___  ___  ______      _____  ___  ____  ___  _____\n  / /|_/ / / __ \\/ _ \\/ ___/ | /| / / _ \\/ _ \\/ __ \\/ _ \\/ ___/\n / /  / / / / / /  __(__  )| |/ |/ /  __/  __/ /_/ /  __/ /    \n/_/  /_/_/_/ /_/\\___/____/ |__/|__/\\___/\\___/ .___/\\___/_/     \n                                           /_/                 \n");
            Console.ResetColor();
            Display(board, true);
            if (HitMine(board))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Sorry. You lost!");
                Console.ResetColor();
            }
            else if (Won(board))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Congratulations! You won!");
                Console.ResetColor();
            }
        }

        public static bool HitMine(Cell[,] board)
        {
            for (int i = 0; i < ROWS; i++)
                for (int j = 0; j < COLUMNS; j++)
                    if (board[i, j].IsRevealed && board[i, j].IsMine)
                        return true;
            return false;
        }
        public static bool Won(Cell[,] board)
        {
            int safe = 0;

            for (int i = 0; i < ROWS; i++)
            {
                for (int j = 0; j < COLUMNS; j++)
                {
                    Cell cell = board[i, j];
                    if (cell.IsRevealed && !cell.IsMine)
                    {
                        safe++;
                    }
                }
            }
            return safe == ROWS * COLUMNS - MINES;

        }
        public static Cell[,] Initialize()
        {
            Cell[,] board = new Cell[ROWS, COLUMNS];
            for (int i = 0; i < ROWS; i++)
            {
                for (int j = 0; j < COLUMNS; j++)
                {
                    board[i, j] = new Cell(new int[] { i, j }, false, false, false);
                }
            }
            return board;
        }
        public static Cell[,] Result(Cell[,] board, int row, int column, char action)
        {
            switch (action)
            {
                case 'X':
                    board[row, column].IsFlagged = !board[row, column].IsFlagged;
                    break;
                case 'Z':
                    if (board[row, column].IsFlagged)
                    {
                        break;
                    }
                    if (board[row, column].IsMine)
                    {
                        board[row, column].IsRevealed = true;
                    }
                    else
                    {
                        RevealCell(board, row, column);
                    }
                    break;
            }
            return board;
        }

        public static int AdjacentMines(Cell[,] board, int row, int column)
        {
            int mines = 0;
            foreach (Cell neighbor in Neighbors(board, row, column))
            {
                if (neighbor.IsMine)
                {
                    mines++;
                }
            }
            return mines;
        }
        public static Cell[] Neighbors(Cell[,] board, int row, int column)
        {
            int[,] directions = { { -1, -1 }, { -1, 0 }, { -1, 1 }, { 0, -1 }, { 0, 1 }, { 1, -1 }, { 1, 0 }, { 1, 1 } };
            List<Cell> neighbors = new List<Cell>();
            for (int i = 0; i < 8; i++)
            {
                int new_row = row + directions[i, 0];
                int new_column = column + directions[i, 1];
                if (!(new_row >= 0 && new_row < ROWS) || !(new_column >= 0 && new_column < COLUMNS))
                {
                    continue;
                }
                neighbors.Add(board[new_row, new_column]);
            }
            return neighbors.ToArray();
        }
        public static void RevealCell(Cell[,] board, int row, int column)
        {
            Cell cell = board[row, column];
            if (cell.IsRevealed || cell.IsFlagged)
            {
                return;
            }
            cell.IsRevealed = true;
            if (AdjacentMines(board, row, column) == 0)
            {
                foreach (Cell neighbor in Neighbors(board, row, column))
                {
                    RevealCell(board, neighbor.Location[0], neighbor.Location[1]);
                }
            }
        }
        public static Cell[,] SetMines(Cell[,] board, int row, int column)
        {
            int mines_placed = 0;
            while (mines_placed < MINES)
            {
                int random_row = Random.Shared.Next(ROWS);
                int random_column = Random.Shared.Next(COLUMNS);
                if ((random_row, random_column) == (row, column))
                {
                    continue;
                }
                if (board[random_row, random_column].IsMine)
                {
                    continue;
                }
                board[random_row, random_column].IsMine = true;
                mines_placed++;
            }
            return board;
        }
        public static bool Terminal(Cell[,] board)
        {
            int non_mine = 0;
            for (int i = 0; i < ROWS; i++)
            {
                for (int j = 0; j < COLUMNS; j++)
                {
                    Cell cell = board[i, j];
                    if (cell.IsRevealed)
                    {
                        if (cell.IsMine)
                        {
                            return true;
                        }
                        else
                        {
                            non_mine++;
                        }
                    }
                }
            }
            return non_mine == ROWS * COLUMNS - MINES;

        }
        public static void Display(Cell[,] board, bool showMines = false)
        {
            Console.Write("  \t");
            for (int i = 0; i < COLUMNS; i++)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write($"{i + 1} ".PadRight(3));
                Console.ResetColor();
            }
            Console.WriteLine();
            for (int i = 0; i < ROWS; i++)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write($"{i + 1}\t");
                Console.ResetColor();
                for (int j = 0; j < COLUMNS; j++)
                {
                    Cell cell = board[i, j];
                    if (showMines)
                    {
                        cell.IsRevealed = true;
                    }
                    if (cell.IsRevealed)
                    {
                        int count = AdjacentMines(board, cell.Location[0], cell.Location[1]);
                        if (cell.IsMine)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("◼  ".PadRight(3));
                            Console.ResetColor();
                        }
                        else if (count > 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write($"{count} ".PadRight(3));
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("◼ ".PadRight(3));
                            Console.ResetColor();
                        }
                    }
                    else if (cell.IsFlagged)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("⚑ ".PadRight(3));
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("◼ ".PadRight(3));
                        Console.ResetColor();
                    }
                }
                Console.WriteLine();
            }
        }
    }

    public class Cell
    {
        public bool IsMine { get; set; }
        public bool IsRevealed { get; set; }
        public bool IsFlagged { get; set; }
        public int[] Location { get; set; }

        public Cell(int[] location, bool isFlagged, bool isRevealed, bool isMine)
        {
            Location = location;
            IsFlagged = isFlagged;
            IsRevealed = isRevealed;
            IsMine = isMine;
        }
    }
}