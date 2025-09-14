using Race.Enums;
using Race.Models;

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
        [Reset, Pause, Exit, MoveLeft, MoveRight, Shift, null, null, null],
        // SHIFT UP CAR
        [Shift, Shift, Shift, Shift, Shift, Shift, Shift, Shift, null],
        // PAUSE
        [Continue, Continue, Continue, Continue, Continue, Continue, Continue, Continue, Continue],
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


    public void UserInput(UserAction action, bool hold) => GetAction(State, action)?.Invoke();
    private Action? GetAction(State state, UserAction action) => Actions[(int)state][(int)action];
    private void Start() => State = State.Moving;

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
    private void GameOver() { }


    // ADD TIMER
    private void Pause() => State = State.Pause;
    private void Continue() => State = State.Moving;




    private void Exit() { }


    //private bool CheckTimer()
    //{

    //}

    // TODO FIX COLLISION
    //private bool CheckCollideWithEnemiesCars(int y, int x) => EnemiesCars.Any(car => car.Points.Contains((y, x)));
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
    // TODO FIX COLLISION
    //private bool CheckCollideWithMyCar(int y, int x) => MyCar.Points.Contains((y, x));
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
        if (Score > HighScore)
        {
            HighScore = Score;
        }
    }

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
