using Race;

namespace ConsoleView;

public class View
{
    public static void Start()
    {
        IModel race = new Race.Impl.Race();

        PrintGame(race.UpdateCurrentState());

        while (true)
        {
            var userAction = GetAction();

            if (userAction == Race.Enums.UserAction.Terminate)
            {
                break;
            }
            race.UserInput(userAction, false);
            var data = race.UpdateCurrentState();

            PrintGame(data);
            var delayTask = Task.Delay(100);
            delayTask.Wait();
        }

        Console.Clear();
        Console.WriteLine("Game terminated");
        Console.ReadKey();
    }



    public static Race.Enums.UserAction GetAction()
    {
        if (Console.KeyAvailable)
        {
            var key = Console.ReadKey(true);


            return key.Key switch
            {
                ConsoleKey.Enter => Race.Enums.UserAction.Start,
                ConsoleKey.P => Race.Enums.UserAction.Pause,
                ConsoleKey.Escape => Race.Enums.UserAction.Terminate,
                ConsoleKey.LeftArrow => Race.Enums.UserAction.Left,
                ConsoleKey.RightArrow => Race.Enums.UserAction.Right,
                ConsoleKey.UpArrow => Race.Enums.UserAction.Up,
                ConsoleKey.DownArrow => Race.Enums.UserAction.Down,
                ConsoleKey.Spacebar => Race.Enums.UserAction.Action,
                _ => Race.Enums.UserAction.Err
            };
        }

        return Race.Enums.UserAction.Err;
    }


    public static void PrintGame(Race.Models.GameInfo gameInfo)
    {
        Console.Clear();
        int i = 0;
        foreach (var row in gameInfo.Field)
        {
            Console.Write($"{i}{(i < 10 ? " " : "")}");
            foreach (var cell in row)
            {
                Console.Write(cell ? "[]" : "  ");
            }
            Console.WriteLine();
            i++;
        }
        Console.Write("  0 1 2 3 4 5 6 7 8 9");

        Console.SetCursorPosition(Options.Width * 2 + 10, 0);
        Console.WriteLine($"Score: {gameInfo.Score}");
        Console.SetCursorPosition(Options.Width * 2 + 10, 2);
        Console.WriteLine($"HighScore: {gameInfo.HighScore}");
        Console.SetCursorPosition(Options.Width * 2 + 10, 4);
        Console.WriteLine($"Level: {gameInfo.Level}");
        Console.SetCursorPosition(Options.Width * 2 + 10, 6);
        Console.WriteLine($"Speed: {gameInfo.Speed}");
        Console.SetCursorPosition(Options.Width * 2 + 10, 8);
        Console.WriteLine($"Pause: {gameInfo.Pause}");
        //DEBUG
        Console.SetCursorPosition(Options.Width * 2 + 10, 10);
        Console.WriteLine($"State: {gameInfo.State}");
    }
}
