using System;
using System.Linq;

//display
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

        public static int CURRENT = -1;
        public static bool CAGED = false;
        public static bool DOUBLE_DMG = false;
        public const int SHELL_COUNT = 6;
        public const int HEALTH = 5;
        public static int TURN;
        public const int MAX_ITEMS = 3;
        public static int ROUND;
        public static int[] SHELLS = new int[SHELL_COUNT];
        public static void Run()
        {
            Console.Clear();
            Console.ResetColor();
            Queue();
            Player player = new Player(HEALTH, [ITEMS[^1], ITEMS[^1], ITEMS[^1]]);
            Player ai = new Player(HEALTH, [ITEMS[^1], ITEMS[^1], ITEMS[^1]]);
            TURN = Random.Shared.NextDouble() < 0.5 ? 1 : 0; //1 -> Player, 0 -> AI
            ROUND = 1;
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
                    continue;
                }
                if (TURN == 1)
                {
                    Console.WriteLine("Inventory (Z) or Fire (X)");
                    Console.Write("Enter action: ");
                    char key = Console.ReadKey().KeyChar;
                    if (key != 'Z' && key != 'X')
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
                            if (key != 'Z' && key != 'X')
                            {
                                continue;
                            }
                            Fire(player, ai, key == 'Z' ? true : false);
                            break;
                    }
                }
                else
                {
                    int RemainingShells = SHELLS.Length;
                    int LiveShells = SHELLS.Count(x => x == 1);
                    double LiveProbability = (double)LiveShells / RemainingShells;
                    if (CURRENT == 1)
                    {
                        LiveProbability = 1;
                    }
                    else if (CURRENT == 0)
                    {
                        LiveProbability = 0;
                    }
                    int priority;
                    if (ai.Health == 1)
                    {
                        priority = -1; //Survive
                    }
                    else if (player.Health == 1)
                    {
                        priority = 1; //Attack
                    }
                    else
                    {
                        if (LiveProbability >= 0.6)
                        {
                            priority = 1; //Attack
                        }
                        else
                        {
                            priority = -1; //Survive
                        }
                    }
                    if (ai.ItemCount(ITEMS[0]) > 0 && LiveProbability >= 0.33 && CURRENT == -1) //Magnifying Glass
                    {
                        UseItem(ITEMS[0], player, ai);
                    }
                    if (ai.ItemCount(ITEMS[3]) > 0 && LiveProbability >= 0.5) //Cage
                    {
                        UseItem(ITEMS[3], player, ai);
                    }
                    if (ai.ItemCount(ITEMS[1]) > 0 && ai.Health <= HEALTH - 1) //Drumstick
                    {
                        UseItem(ITEMS[1], player, ai);
                    }
                    if (ai.ItemCount(ITEMS[6]) > 0 && (player.ItemCount(ITEMS[2]) > 0 || player.ItemCount(ITEMS[5]) > 0 || player.ItemCount(ITEMS[^1]) > 0) && ai.ItemCount(ITEMS[^1]) > 0) //Balaclava
                    {
                        UseItem(ITEMS[6], player, ai); //AI steal needs to be set up
                    }
                    if (priority == 1)
                    {
                        if (ai.ItemCount(ITEMS[5]) > 0 && (CURRENT == 0 || LiveProbability <= 0.45)) //Extractor
                        {
                            UseItem(ITEMS[5], player, ai);
                        }
                        if (ai.ItemCount(ITEMS[4]) > 0 && LiveProbability >= 0.45)  //Exploding Barrel
                        {
                            UseItem(ITEMS[4], player, ai);
                        }
                        if (ai.ItemCount(ITEMS[7]) > 0 && ai.Health == 1) //Potion
                        {
                            UseItem(ITEMS[7], player, ai);
                        }
                        Fire(player, ai, false);
                    }
                    else
                    {
                        if (ai.ItemCount(ITEMS[5]) > 0 && (CURRENT == 1 || LiveProbability >= 0.6)) //Extractor
                        {
                            UseItem(ITEMS[5], player, ai);
                        }
                        if (ai.ItemCount(ITEMS[7]) > 0 && ai.Health <= 2) //Potion
                        {
                            UseItem(ITEMS[7], player, ai);
                        }
                        Fire(player, ai, true);
                    }
                }
            }
        }
        public static void Fire(Player player, Player ai, bool own)
        {
            int shell = SHELLS[0];
            SHELLS = SHELLS[1..];
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
                TURN = TURN == 1 ? 0 : 1;
            }
            CURRENT = -1;
            player.Health = Math.Max(player.Health, 0);
            ai.Health = Math.Max(ai.Health, 0);
        }
        public static void UseItem(Item item, Player player, Player ai)
        {
            switch (item.Name.ToLower())
            {
                case "magnifying glass":
                    Console.WriteLine($"Current shell: {(SHELLS[0] == 1 ? "Live" : "Blank")}");
                    CURRENT = SHELLS[0] == 1 ? 1 : 0;
                    break;
                case "drumstick":
                    if (TURN == 1)
                    {
                        player.Health = Math.Min(player.Health + 1, HEALTH);
                    }
                    else
                    {
                        ai.Health = Math.Min(ai.Health + 1, HEALTH);
                    }
                    break;
                case "extractor":
                    SHELLS = SHELLS[1..];
                    CURRENT = -1;
                    break;
                case "cage":
                    CAGED = true;
                    break;
                case "exploding barrel":
                    DOUBLE_DMG = true;
                    break;
                case "convertor":
                    SHELLS[0] = SHELLS[0] == 1 ? 0 : 1;
                    CURRENT = SHELLS[0];
                    break;
                case "balaclava":
                    if (TURN == 1)
                    {
                        if (player.ItemCount(ITEMS[^1]) == 0 || ai.ItemCount(ITEMS[^1]) == MAX_ITEMS)
                        {
                            return;
                        }
                        else
                        {
                            Console.WriteLine("Choose item to steal.");
                            bool[] allowed = [false, false, false];
                            for (int i = 0; i < MAX_ITEMS; i++)
                            {
                                if (ai.Inventory[i].Name == ITEMS[^1].Name)
                                {
                                    continue;
                                }
                                allowed[i] = true;
                                Console.WriteLine($"{i + 1}. {ai.Inventory[i].Name}");
                            }
                            Console.Write("Enter corresponding number key: ");
                            char key = Console.ReadKey().KeyChar;
                            if (!char.IsDigit(key))
                            {
                                return;
                            }
                            int num = key - '0';
                            if (num > 3 || num < 1 || !allowed[num - 1])
                            {
                                return;
                            }
                            Item popped = ai.PopItem(num - 1);
                            player.InventoryAdd(popped);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < MAX_ITEMS; i++)
                        {
                            if (player.Inventory[i].Name == ITEMS[2].Name || player.Inventory[i].Name == ITEMS[5].Name || player.Inventory[i].Name == ITEMS[6].Name)
                            {
                                Item popped = player.PopItem(i);
                                ai.InventoryAdd(popped);
                                break;
                            }
                        }
                    }
                    break;
                case "potion":
                    if (TURN == 1)
                    {
                        player.Health = Random.Shared.NextDouble() > 0.5 ? Math.Min(player.Health + 2, HEALTH) : Math.Max(player.Health - 1, 0);
                    }
                    else
                    {
                        ai.Health = Random.Shared.NextDouble() > 0.5 ? Math.Min(ai.Health + 2, HEALTH) : Math.Max(ai.Health - 1, 0);
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
            if (player.ItemCount(ITEMS[^1]) == 0)
            {
                return;
            }
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
                        player.InventoryAdd(item);
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
            CURRENT = -1;
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

        public bool Allowed(Item item)
        {
            return !(ItemCount(item) == item.Max);
        }
        public Item PopItem(int id)
        {
            for (int i = 0; i < BuckshotRoulette.MAX_ITEMS; i++)
            {
                if (i == id)
                {
                    Item popped = Inventory[i];
                    Inventory[i] = BuckshotRoulette.ITEMS[^1];
                    return popped;
                }
            }
            return BuckshotRoulette.ITEMS[^1];
        }
        public void InventoryAdd(Item item)
        {
            if (ItemCount(item) == item.Max)
            {
                return;
            }
            for (int i = 0; i < BuckshotRoulette.MAX_ITEMS; i++)
            {
                if (Inventory[i].Name == BuckshotRoulette.ITEMS[^1].Name)
                {
                    Inventory[i] = item;
                    break;
                }
            }
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