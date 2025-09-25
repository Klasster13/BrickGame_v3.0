namespace WebController.Models.Responses;

public class ErrorMessage(string message)
{
    /// <summary>
    /// Сообщение об ошибке
    /// </summary>
    public string Message { get; init; } = message;
}
