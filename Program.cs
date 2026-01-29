using System;
using ArcadeProject.Utility;
using ArcadeProject.Games;

try
{
    int game = Menu.Run();
    switch (game)
    {
        case 1:
            Sudoku.Run();
            break;
        case 2:
            Nim.Run();
            break;
        case 3:
            Hangman.Run();
            break;
        case 4:
            Mastermind.Run();
            break;
        case 5:
            Tiles.Run();
            break;
        case 6:
            TicTacToe.Run();
            break;
        case 7:
            Minesweeper.Run();
            break;
        case 8:
            BuckshotRoulette.Run();
            break;
    }
}
finally
{
    Cleanup.Run();
}
