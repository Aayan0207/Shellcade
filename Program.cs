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
//Mastermind, Sudoku, TicTacToe, Buckshot Roulette, 2048, Pong, Dino Runner
