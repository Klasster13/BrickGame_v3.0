namespace WebApp.Models.Responses;

public class GameState
{
    /// <summary>
    /// Матрица, описывающая состояние игрового поля
    /// </summary>
    public bool[][] Field { get; set; } = null!;

    /// <summary>
    /// Поле доп. информации
    /// </summary>
    public bool[][] Next { get; set; } = null!;

    /// <summary>
    /// Текущее количество очков
    /// </summary>
    public int Score { get; set; }

    /// <summary>
    /// Максимальное количество очков
    /// </summary>
    public int HighScore { get; set; }

    /// <summary>
    /// Текущий уровень
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Текущая скорость
    /// </summary>
    public int Speed { get; set; }

    /// <summary>
    /// Состояние паузы
    /// </summary>
    public bool Pause { get; set; }


    public GameState(Common.Models.State state)
    {
        Field = state.Field;
        Next = state.Next;
        Score = state.Score;
        HighScore = state.HighScore;
        Level = state.Level;
        Speed = state.Speed;
        Pause = state.Pause;
    }
}
