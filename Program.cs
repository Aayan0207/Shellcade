using System;
using ArcadeProject.Utility;
using ArcadeProject.Games;

try
{
    int game = Menu.Run();
    switch (game)
    {
        case 1:
            Console.WriteLine("Sudoku");
            break;
        case 2:
            Console.WriteLine("Dino Runner");
            break;
        case 3:
            Hangman.Run();
            break;
    }
}
finally
{
    Cleanup.Run();
}
//Pong, Dino Runner, Minesweeper, Crossword, Hangman, Sudoku
