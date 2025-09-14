namespace Race.Models;

internal class Car
{
    public static readonly bool[][] Body =
    [
        [false, true, false],
        [true, true, true],
        [false, true, false],
        [true, false, true]
    ];

    public List<(int Y, int X)> Points { get; set; }

    public Car(bool isEnemy = false)
    {
        Points = [];
        GeneratePoints(isEnemy);
    }


    public void GeneratePoints(bool isEnemy = false)
    {
        var rnd = new Random();
        var dx = rnd.NextDouble() >= 0.5 ? 0 : 3;

        for (int i = 0; i < Body.Length; i++)
        {
            for (int j = 0; j < Body[i].Length; j++)
            {
                if (Body[i][j] == false) { continue; }

                int y = isEnemy ? 0 - (Body.Length - i) : Options.Height - (Body.Length - i);
                int x = Options.Width / 2 - (Body[i].Length - j) + dx;
                Points.Add((y, x));
            }
        }
    }
}