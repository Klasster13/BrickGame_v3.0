namespace Common.Interfaces;

// TODO REMOVE ASYNC
public interface IView
{
    Task StartAsync();
    Task StopAsync();
}
