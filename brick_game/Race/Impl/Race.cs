using Race.Enums;
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
    private State State { get; set; }
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
        State = State.Start;
        SpaceBetweenCars = 0;
        FenceOffset = 0;
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


    public GameInfo UpdateCurrentState()
    {
        var info = new GameInfo(
            Field: new bool[Options.Height][],
            Next: new bool[Options.NextSize][],
            Score: Score,
            HighScore: HighScore,
            Level: Level,
            Speed: Speed,
            Pause: State == State.Pause,
            State: State // DELETE
            );

        for (int y = 0; y < Options.Height; y++)
        {
            info.Field[y] = new bool[Options.Width];
            var fenceRow = (y + FenceOffset) % (Options.FenceLength + Options.FenceSpace);
            var isFenceRow = fenceRow < Options.FenceLength;

            for (int x = 0; x < Options.Width; x++)
            {
                var isFenceCell = x == 0 || x == Options.Width - 1;
                info.Field[y][x] = isFenceRow && isFenceCell;
            }
        }

        for (int y = 0; y < Options.NextSize; y++)
        {
            info.Next[y] = new bool[Options.NextSize];
            for (int x = 0; x < Options.NextSize; x++)
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
                if (Y < 0 || Y >= Options.Height)
                {
                    continue;
                }
                info.Field[Y][X] = true;
            }
        }

        return info;
    }


    public void UserInput(UserAction action, bool hold)
    {
        CheckTimer();
        Actions[(int)State][(int)action]?.Invoke();
    }


    private void CheckTimer()
    {
        if (Timer?.ElapsedMilliseconds >= Timeout && State != State.GameOver)
        {
            State = State.Shifting;
        }
    }


    private void Start()
    {
        Timer = Stopwatch.StartNew();
        State = State.Moving;
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
                    State = State.GameOver;
                    //return;
                }
            }

            for (int i = 0; i < car.Points.Count; i++)
            {
                car.Points[i] = car.Points[i] with { Y = car.Points[i].Y + dy };
            }
            if (State == State.GameOver)
            {
                return;
            }
        }
        CountScore();
        SpawnEnemyCar();
        FenceOffset = (FenceOffset + 3) % (Options.FenceLength + Options.FenceSpace);
        State = State.Moving;
        Timer?.Restart();
    }


    private void SpawnEnemyCar()
    {
        if (EnemiesCars.Last().Points.TrueForAll(p => p.Y > 0))
        {
            SpaceBetweenCars++;
        }

        if (SpaceBetweenCars >= Car.Body.Length + Options.SpaceBetweenCars)
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
        int count = MyCar.Points.Count;

        for (int i = 0; i < count; i++)
        {
            int newX = points[i].X + dx;
            if (newX < 0 || newX >= Options.Width)
            {
                return;
            }
            else if (CheckCollideWithEnemiesCars(points[i].Y, newX))
            {
                State = State.GameOver;
                //return;
            }

        }
        for (int i = 0; i < count; i++)
        {
            points[i] = points[i] with { X = points[i].X + dx };
        }
        if (State == State.GameOver)
        {
            return;
        }
    }


    private void SpeedUp() { }


    private void GameOver()
    {
        Timer?.Reset();
        Level = -1;
    }


    // ADD TIMER
    private void Pause()
    {
        State = State.Pause;
        Timer?.Stop();
    }


    private void Continue()
    {
        State = State.Moving;
        Timer?.Start();
    }


    // TODO
    private void Exit() { }


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
        if (MyCar.Points.Any(p => p.Y == y) && MyCar.Points.Any(p => p.X == x))
        {
            return true;
        }

        return false;
    }


    private void CountScore()
    {
        Score += EnemiesCars.RemoveAll(car => car.Points.TrueForAll(p => p.Y >= Options.Height));
        UpdateHighScore();
        UpdateLevel();
    }


    private void UpdateHighScore() => HighScore = Math.Max(HighScore, Score);


    private void UpdateLevel()
    {
        if (Score < Level * Options.PointsPerLevel)
        {
            return;
        }

        Level = Math.Min(Level + 1, Options.MaxLevelNumber);
        Speed = Level;
    }


    private void UpdateTimeout() => Timeout = Options.BaseTimeout - (Speed * Options.SpeedUpPerLevel);

    private void Reset()
    {
        MyCar = new Car();
        EnemiesCars.Clear();
        EnemiesCars.Add(new Car(isEnemy: true));
        Score = 0;
        Level = 1;
        Speed = 1;
        State = State.Start;
        SpaceBetweenCars = 0;
        FenceOffset = 0;
    }
}
