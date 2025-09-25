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


    private void GeneratePoints(bool isEnemy = false)
    {
        var rnd = new Random();
        var dx = rnd.NextDouble() >= 0.5 ? 0 : 3;

        for (var i = 0; i < Body.Length; i++)
        {
            for (var j = 0; j < Body[i].Length; j++)
            {
                if (Body[i][j] == false) { continue; }

                var y = isEnemy ? 0 - (Body.Length - i) : Common.Options.CommonOptions.Height - (Body.Length - i);
                var x = Common.Options.CommonOptions.Width / 2 - (Body[i].Length - j) + dx;
                Points.Add((y, x));
            }
        }
    }
}