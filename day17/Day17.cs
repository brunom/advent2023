using System.Collections.Immutable;
using System.Diagnostics;
using System.Xml.Linq;
using Xunit;
using static Day17;

public class Day17
{
    public readonly record struct Map(ImmutableDictionary<(int x, int y), int> HeatLoss);

    public readonly record struct Solution(int TotalHeatLoss, ImmutableStack<(int x, int y)> Path);
    public enum Dir { N, S, W, E };
    public readonly record struct Solutions(ImmutableDictionary<(int x, int y, Dir), Lazy<Solution?>> Dic);
    static (int x, int y) Move(int x, int y, Dir dir)
    {
        return dir switch
        {
            Dir.N => (x, y - 1),
            Dir.S => (x, y + 1),
            Dir.W => (x - 1, y),
            Dir.E => (x + 1, y),
            _ => throw new NotImplementedException(),
        };
    }
    static readonly ImmutableArray<Dir> NS = [Dir.N, Dir.S,];
    static readonly ImmutableArray<Dir> WE = [Dir.W, Dir.E,];
    static IEnumerable<Dir> Turn(Dir dir)
    {
        return dir switch
        {
            Dir.N => WE,
            Dir.S => WE,
            Dir.W => NS,
            Dir.E => NS,
            _ => throw new NotImplementedException(),
        };
    }
    static Solutions Solve(Map map)
    {
        ImmutableDictionary<(int x, int y, Dir), Lazy<Solution?>>? dic = null;
        dic = ImmutableDictionary.CreateRange(
            from xy in map.HeatLoss
            from d in Enum.GetValues<Dir>()
            select new KeyValuePair<(int x, int y, Dir), Lazy<Solution?>>((xy.Key.x, xy.Key.y, d), new Lazy<Solution?>(() =>
            {
                if (!map.HeatLoss.ContainsKey(Move(xy.Key.x, xy.Key.y, Dir.S)) &&
                    !map.HeatLoss.ContainsKey(Move(xy.Key.x, xy.Key.y, Dir.E)))
                    return new(0, ImmutableStack.Create<(int, int)>());
                var x0y0 = (xy.Key.x, xy.Key.y);
                var x1y1 = Move(x0y0.x, x0y0.y, d);
                var x2y2 = Move(x1y1.x, x1y1.y, d);
                var x3y3 = Move(x2y2.x, x2y2.y, d);
                return (
                    from xy in new[] { x1y1, x2y2, x3y3, }
                    where map.HeatLoss.ContainsKey(xy)
                    from d1 in Turn(d)
                    let succ = dic![(xy.x, xy.y, d1)].Value
                    where succ != null
                    select succ)
                    .MinBy(a => a.Value.TotalHeatLoss);
            }, isThreadSafe: false)));
        return new(dic);
    }
    static int MinTotalHeatLoss(Solutions ss, int x = 0, int y = 0)
    {
        return (
            from d in Enum.GetValues<Dir>()
            let s = ss.Dic[(x, y, d)].Value
            where s != null
            select s.Value.TotalHeatLoss)
            .Min();
    }
    static Map Load(string path)
    {
        return new(ImmutableDictionary.CreateRange(
            File.ReadLines(path)
            .SelectMany((line, y) => line.Select((ch, x) => new KeyValuePair<(int, int), int>((x, y), ch - '0')))));
    }
    [Fact] public void Test_part1_example() => Assert.Equal(5, MinTotalHeatLoss(Solve(Load("example.txt"))));
}
