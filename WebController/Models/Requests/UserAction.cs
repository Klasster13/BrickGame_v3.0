namespace WebController.Models.Requests;

public class UserAction
{
    /// <summary>
    /// Идентификатор действия
    /// </summary>
    public int ActionId { get; set; }

    /// <summary>
    /// Флаг, отвечающий за зажатие кнопки
    /// </summary>
    public bool Hold { get; set; }
}
