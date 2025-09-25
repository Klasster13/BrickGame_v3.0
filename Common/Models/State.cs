namespace Common.Models;

public readonly record struct State(
    bool[][] Field,
    bool[][] Next,
    int Score,
    int HighScore,
    int Level,
    int Speed,
    bool Pause);