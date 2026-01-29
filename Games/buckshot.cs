using System;

namespace ArcadeProject.Games
{
    public class BuckshotRoulette
    {
        Item[] ITEMS = [
            new Item("Magnifying Glass", "See current shell", 40, 2),
            new Item("Drumstick", "+1 HP", 40, 2),
            new Item("Extractor", "Remove current shell", 25, 1),
            new Item("Cage", "Opponent skips a turn", 25, 1),
            new Item("Exploding Barrel", "Shell does double damage", 15, 1),
            new Item("Convertor", "Convert Shell Type", 15, 1),
            new Item("Balaclava", "Steal an item from opponent", 8, 1),
            new Item("Potion", "50% -1HP/ 50% +2HP", 2, 1),
        ];

        const int SHELLS = 6;
        const int HEALTH = 5;
        int TURN;
        const int MAX_ITEMS = 3;
        public static void Run()
        {
            Console.Clear();
            Console.ResetColor();
            int[] shells = Queue();
            Player player = Player(HEALTH, []);
            Player ai = Player(HEALTH, []);
            TURN = Random.Shared.NextDouble() < 0.5 ? 1 : 0; //1 -> Player, 0 -> AI
            int round = 1;
            player.Inventory = new Item[];
            ai.Inventory = new Item[];
            while (player.Health > 0 && ai.Health > 0)
            {
                Console.Clear();
                Console.ResetColor();
                Display(player, ai, shells);
                if (shells.Length == 0)
                {
                    round++;
                    player.Inventory = Items(player.Inventory);
                    ai.Inventory = Items(ai.Inventory);
                }
                if (turn == 1)
                {
                    Console.WriteLine("Inventory (Z) or Fire (X)");
                    Console.Write("Enter action: ");
                    char key = Console.ReadKey().KeyChar;
                    if (key != 'Z' || key != 'X')
                    {
                        continue;
                    }
                    switch (key)
                    {
                        case 'Z':
                            Console.Clear();
                            Console.ResetColor();
                            Display(player, ai, shells, 1);
                            break;
                        case 'X':
                            Console.Write("Fire at yourself (Z) or fire at opponent (X): ");
                            key = Console.ReadKey().KeyChar;
                            if (key != 'Z' || key != 'X')
                            {
                                continue;
                            }
                            int shell = shells[0];
                            shells = shells[1..];
                            Fire(player, ai, shell);
                            break;
                    }
                }
                else
                {
                    //AI
                }
            }
        }


        public static Item[] Items(Player player)
        {
            int spaces = MAX_ITEMS - player.Inventory.Length;
            switch (spaces)
            {
                case 0:
                    break;
                case 1:
                    player.Inventory = AddItem(player.Inventory);
                    break;
                default:
                    int val = Random.Shared.Next(1, 3);
                    for (int i = 0; i < val; i++)
                    {
                        player.Inventory = AddItem(player.Inventory);
                    }
                    break;
            }
            return player.Inventory;
        }

        public static Item[] AddItem(Item[] inventory)
        {
            int totalWeight = 0, running = 0;
            foreach (Item item in ITEMS)
            {
                totalWeight += item.Roll;
            }
            int roll = Random.Shared.Next(1, totalWeight + 1);
            foreach (Item item in ITEMS)
            {
                running += item.Roll;
                if (roll <= running)
                {
                    if (Allowed(item, inventory))
                    {
                        inventory = inventory.Append(item).ToArray();
                        return inventory;
                    }
                    else
                    {
                        return AddItem(inventory);
                    }
                }
            }

        }
        public static int[] Queue()
        {
            int[] shells = new int[SHELLS];
            int live = Random.Shared.Next(2, 4);
            for (int i = 0; i < live; i++)
            {
                shells[i] = 1;
            }
            for (int i = live; i < SHELLS; i++)
            {
                shells[i] = 0;
            }
            Random.Shared.Shuffle(shells);
            return shells;
        }
    }

    public class Item
    {
        public string Name { get; set; }

        public string Description { get; set; }
        public int Roll { get; set; }
        public int Max { get; set; }

        public Item(string name, string description, int roll, int max)
        {
            Name = name;
            Description = description;
            Roll = roll;
            Max = max;
        }

    }
    public class Player
    {
        public int Health { get; set; }
        public Item[] Inventory { get; set; }

        public Player(int health, Item[] inventory)
        {
            Health = health;
            Inventory = inventory;
        }
    }
}