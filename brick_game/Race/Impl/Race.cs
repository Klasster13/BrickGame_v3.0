using Common.Enums;
using Common.Interfaces;
using Common.Models;
using Common.Options;
using Race.Options;
using Race.Models;

using System.Diagnostics;

namespace Race.Impl;

public class Race : IModel
{
    private Action?[][] Actions { get; init; }
    private Car MyCar { get; set; }
    private List<Car> EnemiesCars { get; init; }
    private int Score { get; set; }
    private int HighScore { get; set; }
    private int Level { get; set; }
    private int Speed { get; set; }
    private Common.Enums.GameState State { get; set; }
    private int SpaceBetweenCars { get; set; }
    private int FenceOffset { get; set; }
    private Stopwatch? Timer { get; set; }
    private long Timeout { get; set; }


    public Race()
    {
        Actions = CreateActions();
        MyCar = new Car();
        EnemiesCars = [new Car(isEnemy: true)];
        Score = 0;
        HighScore = 0;
        Level = 1;
        Speed = 1;
        State = GameState.Start;
        SpaceBetweenCars = 0;
        FenceOffset = 0;
        UpdateTimeout();
    }


    private Action?[][] CreateActions() =>
    [
        // START
        [Start, null, null, null, null, null, null, null, null],
        // MOVE LEFT/RIGHT
        [Reset, Pause, Exit, MoveLeft, MoveRight, SpeedUp, null, null, null],
        // SHIFT UP CAR
        [Shift, Shift, Shift, Shift, Shift, Shift, Shift, Shift, Shift],
        // PAUSE
        [Continue, Continue, Continue, Continue, Continue, Continue, Continue, Continue, null],
        // GAMEOVER
        [Reset, GameOver, GameOver, GameOver, GameOver, GameOver, GameOver, GameOver, GameOver],
        // GAMEOVER
        [Exit, Exit, Exit, Exit, Exit, Exit, Exit, Exit, Exit],
    ];


    public State UpdateCurrentState()
    {
        var info = new State(
            Field: new bool[CommonOptions.Height][],
            Next: new bool[CommonOptions.NextSize][],
            Score: Score,
            HighScore: HighScore,
            Level: Level,
            Speed: Speed,
            Pause: State == GameState.Pause
            );

        for (var y = 0; y < CommonOptions.Height; y++)
        {
            info.Field[y] = new bool[CommonOptions.Width];
            var fenceRow = (y + FenceOffset) % (RaceOptions.FenceLength + RaceOptions.FenceSpace);
            var isFenceRow = fenceRow < RaceOptions.FenceLength;

            for (var x = 0; x < CommonOptions.Width; x++)
            {
                var isFenceCell = x == 0 || x == CommonOptions.Width - 1;
                info.Field[y][x] = isFenceRow && isFenceCell;
            }
        }

        for (var y = 0; y < CommonOptions.NextSize; y++)
        {
            info.Next[y] = new bool[CommonOptions.NextSize];
            for (var x = 0; x < CommonOptions.NextSize; x++)
            {
                info.Next[y][x] = false;
            }
        }

        // draw my car
        MyCar.Points.ForEach(p => info.Field[p.Y][p.X] = true);

        // draw enemies cars
        foreach (var car in EnemiesCars)
        {
            foreach (var (Y, X) in car.Points)
            {
                if (Y < 0 || Y >= CommonOptions.Height)
                {
                    continue;
                }
                info.Field[Y][X] = true;
            }
        }

        return info;
    }


    // TODO ADD HOLD 
    public void UserInput(UserAction action, bool hold)
    {
        CheckTimer();
        Actions[(int)State][(int)action]?.Invoke();
    }


    private void CheckTimer()
    {
        if (Timer?.ElapsedMilliseconds >= Timeout && State != GameState.GameOver)
        {
            State = GameState.Shifting;
        }
    }


    private void Start()
    {
        Timer = Stopwatch.StartNew();
        State = GameState.Moving;
    }


    private void Shift()
    {
        int dy = 1;
        foreach (var car in EnemiesCars)
        {
            foreach (var (Y, X) in car.Points)
            {
                if (CheckCollideWithMyCar(Y + dy, X))
                {
                    State = GameState.GameOver;
                }
            }

            for (var i = 0; i < car.Points.Count; i++)
            {
                car.Points[i] = car.Points[i] with { Y = car.Points[i].Y + dy };
            }
            if (State == GameState.GameOver)
            {
                return;
            }
        }
        CountScore();
        SpawnEnemyCar();
        FenceOffset = (FenceOffset + 3) % (RaceOptions.FenceLength + RaceOptions.FenceSpace);
        State = GameState.Moving;
        Timer?.Restart();
    }


    private void SpawnEnemyCar()
    {
        if (EnemiesCars.Last().Points.TrueForAll(p => p.Y > 0))
        {
            SpaceBetweenCars++;
        }

        if (SpaceBetweenCars >= Car.Body.Length + RaceOptions.SpaceBetweenCars)
        {
            EnemiesCars.Add(new Car(isEnemy: true));
            SpaceBetweenCars = 0;
        }
    }

    private void MoveLeft() => Move(-3);
    private void MoveRight() => Move(3);
    private void Move(int dx)
    {
        var points = MyCar.Points;
        var count = MyCar.Points.Count;

        for (var i = 0; i < count; i++)
        {
            var newX = points[i].X + dx;
            if (newX < 0 || newX >= CommonOptions.Width)
            {
                return;
            }
            else if (CheckCollideWithEnemiesCars(points[i].Y, newX))
            {
                State = GameState.GameOver;
                break;
            }

        }
        for (var i = 0; i < count; i++)
        {
            points[i] = points[i] with { X = points[i].X + dx };
        }
    }

    // TODO MB CHANGE TO TIMER DECREASING
    private void SpeedUp() => Shift();


    private void GameOver()
    {
        Timer?.Reset();
        Level = -1;
    }

    private void Pause()
    {
        State = GameState.Pause;
        Timer?.Stop();
    }

    private void Continue()
    {
        State = GameState.Moving;
        Timer?.Start();
    }

    private void Exit()
    {
        Timer?.Reset();
        Level = -2;
    }


    private bool CheckCollideWithEnemiesCars(int y, int x)
    {
        foreach (var car in EnemiesCars)
        {
            if (car.Points.Any(p => p.Y == y) && car.Points.Any(p => p.X == x))
            {
                return true;
            }
        }
        return false;
    }


    private bool CheckCollideWithMyCar(int y, int x)
    {
        return MyCar.Points.Any(p => p.Y == y) && MyCar.Points.Any(p => p.X == x);
    }


    private void CountScore()
    {
        Score += EnemiesCars.RemoveAll(car => car.Points.TrueForAll(p => p.Y >= CommonOptions.Height));
        UpdateHighScore();
        UpdateLevel();
        UpdateTimeout();
    }


    private void UpdateHighScore() => HighScore = Math.Max(HighScore, Score);


    private void UpdateLevel()
    {
        if (Score < Level * RaceOptions.PointsPerLevel)
        {
            return;
        }

        Level = Math.Min(Level + 1, RaceOptions.MaxLevelNumber);
        Speed = Level;
    }


    private void UpdateTimeout() => Timeout = RaceOptions.BaseTimeout - (Speed * RaceOptions.SpeedUpPerLevel);

    private void Reset()
    {
        MyCar = new Car();
        EnemiesCars.Clear();
        EnemiesCars.Add(new Car(isEnemy: true));
        Score = 0;
        Level = 1;
        Speed = 1;
        State = GameState.Start;
        SpaceBetweenCars = 0;
        FenceOffset = 0;
    }
}
