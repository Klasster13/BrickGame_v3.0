using Race.Enums;
using Race.Models;

namespace Race;

public interface IModel
{
    void UserInput(UserAction action, bool hold);
    GameInfo UpdateCurrentState();
}
