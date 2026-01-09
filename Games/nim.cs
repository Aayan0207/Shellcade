using System;
using System.Diagnostics.CodeAnalysis;
namespace ArcadeProject.Games
{
    public static class Nim
    {
        public static void Run()
        {
            Console.Clear();
            Console.ResetColor();
            string[] piles = ["⬬", "⬬⬬⬬", "⬬⬬⬬⬬⬬", "⬬⬬⬬⬬⬬⬬⬬"];
            int turn = 0; //0->Player, 1->Computer
            while (true)
            {
                Console.Clear();
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("    _   ___         \n   / | / (_)___ ___ \n  /  |/ / / __ `__ \\\n / /|  / / / / / / /\n/_/ |_/_/_/ /_/ /_/ \n                    \n");
                Console.ResetColor();
                int count = 0;
                for (int i = 0; i < piles.Length; i++)
                {
                    for (int j = piles.Length; j >= i; j--)
                    {
                        Console.Write(" ");
                    }
                    for (int j = 0; j < piles[i].Length; j++)
                    {
                        Console.Write($"{piles[i][j]} ");
                    }
                    Console.WriteLine();
                    count += piles[i].Length;
                }
                if (count == 0)
                {
                    switch (turn)
                    {
                        case 0:
                            Console.WriteLine("AI Wins!");
                            break;
                        case 1:
                            Console.WriteLine("You Win!");
                            break;
                    }
                    break;
                }
                if (turn == 0)
                {
                    Console.Write("Choose pile to draw from (1-4): ");
                    int pile = int.Parse(Console.ReadLine());
                    if (piles[pile - 1].Length == 0 || pile < 0 || pile > 4)
                    {
                        continue;
                    }
                    else
                    {
                        Console.Write("Draw how many stones? ");
                        int draw = int.Parse(Console.ReadLine());
                        if (draw > piles[pile - 1].Length)
                        {
                            continue;
                        }
                        else
                        {
                            piles[pile - 1] = piles[pile - 1][..^draw];
                        }
                    }
                    turn = 1;
                }
                else
                {
                    int nim_sum = piles[0].Length;
                    for (int i = 1; i < piles.Length; i++)
                    {
                        nim_sum ^= piles[i].Length;
                    }
                    if (nim_sum == 0)
                    {
                        for (int i = 0; i < piles.Length; i++)
                        {
                            if (piles[i].Length > 0)
                            {
                                piles[i] = "";
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < piles.Length; i++)
                        {
                            int target = piles[i].Length ^ nim_sum;
                            if (target < piles[i].Length)
                            {
                                piles[i] = piles[i][..target];
                                break;
                            }
                        }
                    }
                    turn = 0;
                }
            }
        }
    }
}