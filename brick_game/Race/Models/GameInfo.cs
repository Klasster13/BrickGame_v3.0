using Race.Enums;

namespace Race.Models;

public readonly record struct GameInfo(
    bool[][] Field,
    bool[][] Next,
    int Score,
    int HighScore,
    int Level,
    int Speed,
    bool Pause,
    State State); // DELETE