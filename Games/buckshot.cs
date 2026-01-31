using System;
using System.Linq;
using System.Threading;
//firing animation
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
        public static int TIME = 750;
        public static int ANIMATION = 2;

        public static string[] ANIMATIONS_LIVE_UP = [
            "|            🟥          |\n|                        |\n|                        |\n",
            "|                        |\n|            🟥          |\n|                        |\n",
            "|                        |\n|                        |\n|            🟥          |\n"
            ];
        public static string[] ANIMATIONS_LIVE_DOWN = [
           "|                        |\n|                        |\n|            🟥          |\n",
            "|                        |\n|            🟥          |\n|                        |\n",
            "|            🟥          |\n|                        |\n|                        |\n",
            ];
        public static string[] ANIMATIONS_BLANK_UP = [
            "|            🟦          |\n|                        |\n|                        |\n",
            "|                        |\n|            🟦          |\n|                        |\n",
            "|                        |\n|                        |\n|            🟦          |\n"
            ];
        public static string[] ANIMATIONS_BLANK_DOWN = [
            "|                        |\n|                        |\n|            🟦          |\n",
            "|                        |\n|            🟦          |\n|                        |\n",
            "|            🟦          |\n|                        |\n|                        |\n",
            ];
        public static int CURRENT = -1;
        public static bool CAGED = false;
        public static bool DOUBLE_DMG = false;
        public const int SHELL_COUNT = 6;
        public const int HEALTH = 5;
        public static int TURN;
        public const int MAX_ITEMS = 3;
        public static int ROUND = 0;
        public static int[] SHELLS = new int[SHELL_COUNT];

        public static bool start = true;
        public static void Run()
        {
            Console.Clear();
            Console.ResetColor();
            Queue();
            Player player = new Player(HEALTH, [ITEMS[^1], ITEMS[^1], ITEMS[^1]]);
            Player ai = new Player(HEALTH, [ITEMS[^1], ITEMS[^1], ITEMS[^1]]);
            TURN = Random.Shared.NextDouble() < 0.5 ? 1 : 0; //1 -> Player, 0 -> AI
            while (player.Health > 0 && ai.Health > 0)
            {
                if (SHELLS.Length == 0 || start)
                {
                    start = false;
                    ROUND++;
                    Items(player);
                    Items(ai);
                    Queue();
                    continue;
                }
                Display(player, ai);
                if (TURN == 1)
                {
                    Console.WriteLine("Inventory (Z) or Fire (X)");
                    Console.Write("Enter action: ");
                    char key = char.ToUpper(Console.ReadKey().KeyChar);
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
                            Console.Write("\nFire at yourself (Z) or Fire at opponent (X): ");
                            key = char.ToUpper(Console.ReadKey().KeyChar);
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
                    if (ai.ItemCount(ITEMS[0]) > 0 && LiveProbability >= 0.5 && CURRENT == -1) //Magnifying Glass
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
            Display(player, ai);
            if (player.Health == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Sorry. You lost. AI Wins!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Congratulations! You Win!");
            }
            Console.ResetColor();
        }
        public static void Fire(Player player, Player ai, bool own)
        {
            int shell = SHELLS[0];
            SHELLS = SHELLS[1..];
            bool ShootUp = (TURN == 1 && own == false) || (TURN == 0 && own == true);
            for (int frame = 2; frame >= 0; frame--)
            {
                ANIMATION = frame;
                Display(player, ai, false, true, own, ShootUp, shell);
                Thread.Sleep(TIME / 3);
            }
            ANIMATION = 2;
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
            int id = 0;
            if (TURN == 1)
            {
                for (int i = 0; i < MAX_ITEMS; i++)
                {
                    if (player.Inventory[i].Name == item.Name)
                    {
                        id = i;
                        break;
                    }
                }
                player.PopItem(id);
            }
            else
            {
                for (int i = 0; i < MAX_ITEMS; i++)
                {
                    if (ai.Inventory[i].Name == item.Name)
                    {
                        id = i;
                        break;
                    }
                }
                ai.PopItem(id);
            }
            switch (item.Name.ToLower())
            {
                case "magnifying glass":
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
        public static void Display(Player player, Player ai, bool inventory = false, bool firing = false, bool own = false, bool up = false, int shell = -1)
        {
            Console.Clear();
            Console.ResetColor();
            Console.WriteLine("    ____             __        __          __  ____              __     __  __     \n   / __ )__  _______/ /_______/ /_  ____  / /_/ __ \\____  __  __/ /__  / /_/ /____ \n  / __  / / / / ___/ //_/ ___/ __ \\/ __ \\/ __/ /_/ / __ \\/ / / / / _ \\/ __/ __/ _ \\\n / /_/ / /_/ / /__/ ,< (__  ) / / / /_/ / /_/ _, _/ /_/ / /_/ / /  __/ /_/ /_/  __/\n/_____/\\__,_/\\___/_/|_/____/_/ /_/\\____/\\__/_/ |_|\\____/\\__,_/_/\\___/\\__/\\__/\\___/ \n                                                                                   \n");
            Console.WriteLine($"         Round {ROUND}                 ");
            Console.WriteLine("__________________________");
            Console.Write("|       ");
            for (int i = 0; i < ai.Health; i++)
            {
                Console.Write("🟩");
            }
            for (int i = 0; i < HEALTH - ai.Health; i++)
            {
                Console.Write("⬛");
            }
            Console.WriteLine("       |");
            Console.WriteLine("|           🤖           |");
            Console.Write("|     ");
            foreach (Item item in ai.Inventory)
            {
                switch (item.Name.ToLower())
                {
                    case "magnifying glass":
                        Console.Write("🔎");
                        break;
                    case "drumstick":
                        Console.Write("🍗");
                        break;
                    case "potion":
                        Console.Write("🧪");
                        break;
                    case "extractor":
                        Console.Write("🛠️");
                        break;
                    case "convertor":
                        Console.Write("🔄");
                        break;
                    case "exploding barrel":
                        Console.Write("🧨");
                        break;
                    case "cage":
                        Console.Write("⛓️");
                        break;
                    case "balaclava":
                        Console.Write("🥷");
                        break;
                    default:
                        Console.Write(" ");
                        break;
                }
                Console.Write($"    ");
            }
            Console.WriteLine("  |");
            Console.WriteLine("__________________________");
            if (firing && up)
            {
                if (shell == 1)
                {
                    Console.Write(ANIMATIONS_LIVE_UP[ANIMATION]);
                }
                else
                {
                    Console.Write(ANIMATIONS_BLANK_UP[ANIMATION]);
                }
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    Console.WriteLine("|                        |");
                }
            }
            Console.WriteLine("|           ||           |");
            Console.WriteLine("|           ||           |");
            Console.WriteLine("|       ====||====       |");
            Console.WriteLine("|           ||           |");
            Console.WriteLine("|           ||           |");
            if (firing && !up)
            {
                if (shell == 1)
                {
                    Console.Write(ANIMATIONS_LIVE_DOWN[ANIMATION]);
                }
                else
                {
                    Console.Write(ANIMATIONS_BLANK_DOWN[ANIMATION]);
                }
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    Console.WriteLine("|                        |");
                }
            }
            Console.WriteLine("__________________________");
            Console.Write("|     ");
            foreach (Item item in player.Inventory)
            {
                switch (item.Name.ToLower())
                {
                    case "magnifying glass":
                        Console.Write("🔎");
                        break;
                    case "drumstick":
                        Console.Write("🍗");
                        break;
                    case "potion":
                        Console.Write("🧪");
                        break;
                    case "extractor":
                        Console.Write("🛠️");
                        break;
                    case "convertor":
                        Console.Write("🔄");
                        break;
                    case "exploding barrel":
                        Console.Write("🧨");
                        break;
                    case "cage":
                        Console.Write("⛓️");
                        break;
                    case "balaclava":
                        Console.Write("🥷");
                        break;
                    default:
                        Console.Write(" ");
                        break;
                }
                Console.Write($"    ");
            }
            Console.WriteLine("  |");
            Console.WriteLine("|          You           |");
            Console.Write("|       ");
            for (int i = 0; i < player.Health; i++)
            {
                Console.Write("🟩");
            }
            for (int i = 0; i < HEALTH - player.Health; i++)
            {
                Console.Write("⬛");
            }
            Console.WriteLine("       |");
            Console.WriteLine("__________________________");
            Console.WriteLine($"Current shells: 💥 {SHELLS.Count(x => x == 1)} 💨 {SHELLS.Count(x => x == 0)}");
            if (TURN == 1 && CURRENT != -1)
            {
                Console.WriteLine($"Current shell: {(CURRENT == 1 ? "💥" : "💨")}");
            }
            if (inventory)
            {
                bool[] allowed = [false, false, false];
                for (int i = 0; i < MAX_ITEMS; i++)
                {
                    if (player.Inventory[i].Name == ITEMS[^1].Name)
                    {
                        continue;
                    }
                    allowed[i] = true;
                    Console.Write($"{i + 1}. ");
                    switch (player.Inventory[i].Name.ToLower())
                    {
                        case "magnifying glass":
                            Console.Write("🔎");
                            break;
                        case "drumstick":
                            Console.Write("🍗");
                            break;
                        case "potion":
                            Console.Write("🧪");
                            break;
                        case "extractor":
                            Console.Write("🛠️");
                            break;
                        case "convertor":
                            Console.Write("🔄");
                            break;
                        case "exploding barrel":
                            Console.Write("🧨");
                            break;
                        case "cage":
                            Console.Write("⛓️");
                            break;
                        case "balaclava":
                            Console.Write("🥷");
                            break;
                        default:
                            Console.Write("—");
                            break;
                    }
                    Console.WriteLine($"  ({player.Inventory[i].Description})");
                }
                Console.Write("Choose item to use (Enter corresponding number) or any other key to cancel: ");
                char key = Console.ReadKey().KeyChar;
                if (!char.IsDigit(key))
                {
                    return;
                }
                int num = key - '0';
                if (num < 1 || num > MAX_ITEMS || !allowed[num - 1])
                {
                    return;
                }
                UseItem(player.Inventory[num - 1], player, ai);
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
            SHELLS = new int[SHELL_COUNT];
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