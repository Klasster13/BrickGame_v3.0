namespace WebApp.Models.Responses;

public class GameInfo(int id, string name)
{
    /// <summary>
    /// Идентификатор игры
    /// </summary>
    public int Id { get; set; } = id;

    /// <summary>
    /// Название игры
    /// </summary>
    public string Name { get; set; } = name;
}
