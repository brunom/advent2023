using Xunit;

public readonly record struct Coordinate(int X, int Y, int Z);
public readonly record struct Brick(Coordinate loend, Coordinate hiend)
{
    IEnumerable<Coordinate> Coordinates()
    {
        for (int x = loend.X; x <= hiend.X; ++x)
        {
            for (int y = loend.Y; y <= hiend.Y; ++y)
            {
                for (int z = loend.Z; z <= hiend.Z; ++z)
                {
                    yield return new(x, y, z);
                }
            }
        }
    }
}
public class SettledBrick
{
    public Brick Brick;
    public HashSet<SettledBrick> Bellow = new();
    public HashSet<SettledBrick> Above = new();
}
public class Day22
{
    int Safe(string path)
    {
        Dictionary<(int X, int Y), SettledBrick> topBrick = new();
        List<SettledBrick> settledBricks = new();

        var fallingBricks =
            File.ReadLines(path)
            .Select(x => x.Split([',', '~']).Select(int.Parse).ToList())
            .Select(a => new Brick(new(a[0], a[1], a[2]), new(a[3], a[4], a[5])))
            .OrderBy(x => x.loend.Z);
        foreach (var brick in fallingBricks)
        {
            int Z = 1;
            for (int x = brick.loend.X; x <= brick.hiend.X; ++x)
            {
                for (int y = brick.loend.Y; y <= brick.hiend.Y; ++y)
                {
                    if (topBrick.TryGetValue((x, y), out SettledBrick? bellow))
                    {
                        Z = Math.Max(Z, bellow.Brick.hiend.Z + 1);
                    }
                }
            }
            SettledBrick settled = new()
            {
                Brick = new(
                    brick.loend with { Z = Z },
                    brick.hiend with { Z = Z + brick.hiend.Z - brick.loend.Z }
                ),
            };

            for (int x = brick.loend.X; x <= brick.hiend.X; ++x)
            {
                for (int y = brick.loend.Y; y <= brick.hiend.Y; ++y)
                {
                    if (topBrick.TryGetValue((x, y), out SettledBrick? bellow))
                    {
                        if (Z == bellow.Brick.hiend.Z + 1)
                        {
                            bellow.Above.Add(settled);
                            settled.Bellow.Add(bellow);
                        }
                    }
                    topBrick[(x, y)] = settled;
                }
            }

            settledBricks.Add(settled);
        }
        return settledBricks.Count(b => b.Above.All(a => a.Bellow.Count > 1));
    }
    [Fact] public void Test_part1_example() => Assert.Equal(5, Safe("example.txt"));
    [Fact] public void Test_part1_input() => Assert.Equal(430, Safe("input.txt"));
}