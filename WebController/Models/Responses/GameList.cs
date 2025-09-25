namespace WebController.Models.Responses;

public class GameList(GameInfo[] games)
{
    /// <summary>
    /// Список игр
    /// </summary>
    public GameInfo[] Games { get; set; } = games;
}
