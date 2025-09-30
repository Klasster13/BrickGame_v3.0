using Common.Interfaces;
using Common.Enums;
using Common.Models;
using Common.Options;

namespace ConsoleView;

public class ConsoleView(IModel model) : IView
{
    private readonly IModel _model = model;

    public async Task StartAsync()
    {
        PrintGame(_model.UpdateCurrentState());

        while (true)
        {
            var keyTask = GetAction();
            var delayTask = Task.Delay(100);

            var completedTask = await Task.WhenAny(keyTask, delayTask);

            var userAction = completedTask == keyTask
                ? keyTask.Result
                : UserAction.Err;

            if (userAction == UserAction.Terminate)
            {
                break;
            }
            _model.UserInput(userAction, false);

            var data = _model.UpdateCurrentState();
            PrintGame(data);
        }

        Console.Clear();
        Console.WriteLine("Game terminated");
        Console.ReadKey();
    }

    public async Task StopAsync() => await Task.CompletedTask;

    private async static Task<UserAction> GetAction()
    {
        if (!Console.KeyAvailable)
        {
            await Task.Delay(100);
            return UserAction.Err;
        }

        //ConsoleKeyInfo key = default;

        //while (Console.KeyAvailable)
        //{
        var key = Console.ReadKey(true);
        //}

        return key.Key switch
        {
            ConsoleKey.Enter => UserAction.Start,
            ConsoleKey.P => UserAction.Pause,
            ConsoleKey.Escape => UserAction.Terminate,
            ConsoleKey.LeftArrow => UserAction.Left,
            ConsoleKey.RightArrow => UserAction.Right,
            ConsoleKey.UpArrow => UserAction.Up,
            ConsoleKey.DownArrow => UserAction.Down,
            ConsoleKey.Spacebar => UserAction.Action,
            _ => UserAction.Err
        };
    }


    private static void PrintGame(State gameInfo)
    {
        //Console.Clear();
        Console.SetCursorPosition(0, 0);
        var i = 0;
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

        Console.SetCursorPosition(CommonOptions.Width * 2 + 10, 0);
        Console.WriteLine($"Score: {gameInfo.Score}");
        Console.SetCursorPosition(CommonOptions.Width * 2 + 10, 2);
        Console.WriteLine($"HighScore: {gameInfo.HighScore}");
        Console.SetCursorPosition(CommonOptions.Width * 2 + 10, 4);
        Console.WriteLine($"Level: {gameInfo.Level}");
        Console.SetCursorPosition(CommonOptions.Width * 2 + 10, 6);
        Console.WriteLine($"Speed: {gameInfo.Speed}");
        Console.SetCursorPosition(CommonOptions.Width * 2 + 10, 8);
        Console.WriteLine($"Pause: {gameInfo.Pause}");
    }
}
