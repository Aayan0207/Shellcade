using System;

//Item usage, ai turn
namespace ArcadeProject.Games
{
    public class BuckshotRoulette
    {
        public static Item[] ITEMS = [
            new Item("Magnifying Glass", "See current shell", 40, 2),
            new Item("Drumstick", "+1 HP", 40, 2),
            new Item("Extractor", "Remove current shell", 25, 1),
            new Item("Cage", "Opponent skips a turn", 25, 1),
            new Item("Exploding Barrel", "Shell does double damage", 15, 1),
            new Item("Convertor", "Convert Shell Type", 15, 1),
            new Item("Balaclava", "Steal an item from opponent", 8, 1),
            new Item("Potion", "50% -1 HP/ 50% +2 HP", 2, 1),
            new Item("NULL", "NULL", 0, 3),
        ];
        public static bool CAGED = false;
        public static bool DOUBLE_DMG = false;
        public const int SHELL_COUNT = 6;
        public const int HEALTH = 5;
        public static int TURN;
        public const int MAX_ITEMS = 3;
        public static int ROUND;
        public static int[] SHELLS;
        public static void Run()
        {
            Console.Clear();
            Console.ResetColor();
            Queue();
            Player player = new Player(HEALTH, []);
            Player ai = new Player(HEALTH, []);
            TURN = Random.Shared.NextDouble() < 0.5 ? 1 : 0; //1 -> Player, 0 -> AI
            ROUND = 1;
            player.Inventory = [ITEMS[^1], ITEMS[^1], ITEMS[^1]];
            ai.Inventory = [ITEMS[^1], ITEMS[^1], ITEMS[^1]];
            while (player.Health > 0 && ai.Health > 0)
            {
                Console.Clear();
                Console.ResetColor();
                Console.WriteLine("    ____             __        __          __  ____              __     __  __ \n   / __ )__  _______/ /_______/ /_  ____  / /_/ __ \\____  __  __/ /__  / /_/ /_\n  / __  / / / / ___/ //_/ ___/ __ \\/ __ \\/ __/ /_/ / __ \\/ / / / / _ \\/ __/ __/\n / /_/ / /_/ / /__/ ,< (__  ) / / / /_/ / /_/ _, _/ /_/ / /_/ / /  __/ /_/ /_  \n/_____/\\__,_/\\___/_/|_/____/_/ /_/\\____/\\__/_/ |_|\\____/\\__,_/_/\\___/\\__/\\__/  \n                                                                               \n      \n  ___ \n / _ \\\n/  __/\n\\___/ \n      \n");
                Display(player, ai);
                if (SHELLS.Length == 0)
                {
                    ROUND++;
                    Items(player);
                    Items(ai);
                    Queue();
                }
                if (TURN == 1)
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
                            Display(player, ai, true);
                            break;
                        case 'X':
                            Console.Write("Fire at yourself (Z) or fire at opponent (X): ");
                            key = Console.ReadKey().KeyChar;
                            if (key != 'Z' || key != 'X')
                            {
                                continue;
                            }
                            int shell = SHELLS[0];
                            SHELLS = SHELLS[1..];
                            Fire(player, ai, shell, key == 'Z' ? true : false);
                            break;
                    }
                }
                else
                {
                    //AI
                }
            }
        }

        public static void Fire(Player player, Player ai, int shell, bool own)
        {
            Display(player, ai, false, true, shell);
            if (TURN == 1)
            {
                if (shell == 1)
                {
                    if (DOUBLE_DMG)
                    {
                        if (own)
                        {
                            player.Health -= 2;
                        }
                        else
                        {
                            ai.Health -= 2;
                        }
                        DOUBLE_DMG = false;
                    }
                    else
                    {
                        if (own)
                        {
                            player.Health -= 1;
                        }
                        else
                        {
                            ai.Health -= 1;
                        }
                    }
                }
            }
            else
            {
                if (shell == 1)
                {
                    if (DOUBLE_DMG)
                    {
                        if (own)
                        {
                            ai.Health -= 2;
                        }
                        else
                        {
                            player.Health -= 2;
                        }
                        DOUBLE_DMG = false;
                    }
                    else
                    {
                        if (own)
                        {
                            ai.Health -= 1;
                        }
                        else
                        {
                            player.Health -= 1;
                        }
                    }
                }
            }
            if (CAGED)
            {
                CAGED = false;
            }
            else
            {
                TURN = ~TURN;
            }

        }
        public static void UseItem(Item item, Player player, Player ai)
        {
            switch (item.Name.ToLower())
            {
                case "magnifying glass":
                    Console.WriteLine($"Current shell: {(SHELLS[0] == 1 ? "Live" : "Blank")}");
                    break;
                case "drumstick":
                    if (TURN == 1)
                    {
                        player.Health = (player.Health + 1) % 5;
                    }
                    else
                    {
                        ai.Health = (ai.Health + 1) % 5;
                    }
                    break;
                case "extractor":
                    SHELLS = SHELLS[1..];
                    break;
                case "cage":
                    CAGED = true; //Work
                    break;
                case "exploding barrel":
                    DOUBLE_DMG = true; //Work
                    break;
                case "convertor":
                    SHELLS[0] = ~SHELLS[0];
                    break;
                case "balaclava":
                    if (TURN == 1)
                    {
                        if (player.ItemCount(ITEMS[^1]) == 0 || ai.ItemCount(ITEMS[^1]) == 3)
                        {
                            return;
                        }
                        else
                        {
                            Console.WriteLine("Choose item to steal.");
                            int counter = 1;
                            for (int i = 0; i < MAX_ITEMS; i++)
                            {
                                if (ai.Inventory[i].Name == ITEMS[^1].Name)
                                {
                                    continue;
                                }
                                Console.WriteLine($"{counter}. {ai.Inventory[i].Name}");
                                counter++;
                            }
                            Console.Write("Enter corresponding number key: ");
                            char key = Console.ReadKey().KeyChar;
                            if (!char.IsDigit(key))
                            {
                                return;
                            }
                            int num = key - '0';
                            if (num > 3 || num < 1)
                            {
                                return;
                            }
                            Item popped = ai.PopItem(num - 1);
                            player.AddItem(popped);
                        }
                    }
                    else
                    {
                        //AI steal
                    }
                    break;
                case "potion":
                    if (TURN == 1)
                    {
                        player.Health = Random.Shared.NextDouble() > 0.5 ? (player.Health + 1) % 5 : 0;
                    }
                    else
                    {
                        ai.Health = Random.Shared.NextDouble() > 0.5 ? (ai.Health + 1) % 5 : 0;
                    }
                    break;
            }
        }
        public static void Display(Player player, Player ai, bool inventory = false, bool firing = false, int shell = -1)
        {
            if (inventory)
            {

            }
        }

        public static void Items(Player player)
        {
            int spaces = player.ItemCount(ITEMS[^1]);
            switch (spaces)
            {
                case 0:
                    break;
                case 1:
                    AddItem(player);
                    break;
                default:
                    int val = Random.Shared.Next(1, 3);
                    for (int i = 0; i < val; i++)
                    {
                        AddItem(player);
                    }
                    break;
            }
            return;
        }

        public static void AddItem(Player player)
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
                    if (player.Allowed(item))
                    {
                        player.AddItem(item);
                    }
                    else
                    {
                        AddItem(player);
                    }
                }
            }

        }
        public static void Queue()
        {

            int live = Random.Shared.Next(2, 4);
            for (int i = 0; i < live; i++)
            {
                SHELLS[i] = 1;
            }
            for (int i = live; i < SHELL_COUNT; i++)
            {
                SHELLS[i] = 0;
            }
            Random.Shared.Shuffle(SHELLS);
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

        public int ItemCount(Item item)
        {
            int count = 0;
            foreach (Item i in Inventory)
            {
                if (i.Name == item.Name)
                {
                    count++;
                }
            }
            return count;
        }
    }
}