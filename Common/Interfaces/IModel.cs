using Common.Enums;
using Common.Models;

namespace Common.Interfaces;

public interface IModel
{
    void UserInput(UserAction action, bool hold);
    State UpdateCurrentState();
}
