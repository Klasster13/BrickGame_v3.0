using Common.Interfaces;

namespace Selector;

public class Selector
{
    private IModel? _model;
    private IView? _view;


    public async Task StartSelector()
    {
        _model = SelectGame();
        _view = SelectView();


        if (_model is not null && _view is not null)
        {
            Console.Clear();
            await StartAsync();
        }
    }

    private async Task StartAsync() => await _view!.StartAsync();

    private IModel? SelectGame()
    {
        Console.Clear();
        Console.WriteLine("Select game:");
        Console.WriteLine("1) Tetris");
        Console.WriteLine("2) Snake");
        Console.WriteLine("3) Race");


        var key = Console.ReadKey();
        return key.Key switch
        {
            ConsoleKey.D1 => null,
            ConsoleKey.D2 => null,
            ConsoleKey.D3 => new Race.Impl.Race(),
            _ => null
        };
    }

    private IView? SelectView()
    {
        Console.Clear();
        Console.WriteLine("Select view:");
        Console.WriteLine("1) Console");
        Console.WriteLine("2) Desktop");
        Console.WriteLine("3) Web");


        var key = Console.ReadKey();
        return key.Key switch
        {
            ConsoleKey.D1 => new ConsoleView.ConsoleView(_model!),
            ConsoleKey.D2 => null,
            ConsoleKey.D3 => new WebApp.WebApp(_model!),
            _ => null
        };
    }
}
