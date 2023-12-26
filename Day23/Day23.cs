using System.Collections.Immutable;
using System.Diagnostics;
using Xunit;


public class Day23
{
    [Theory]
    [InlineData("example.txt")]
    [InlineData("input.txt")]
    public void Test_single_entry_slopes(string filename)
    {
        string[] lines = File.ReadAllLines(filename);
        Assert.All(lines.SelectMany((line, row) => line.Select((ch, col) => (row, col, ch))), x =>
        {
            if (x.ch == '<' || x.ch == '>')
            {
                Assert.Equal('#', lines[x.row - 1][x.col]);
                Assert.Equal('#', lines[x.row + 1][x.col]);
            }
            else if (x.ch == '^' || x.ch == 'v')
            {
                Assert.Equal('#', lines[x.row][x.col - 1]);
                Assert.Equal('#', lines[x.row][x.col + 1]);
            }
        });
    }

    [Theory]
    [InlineData("example.txt")]
    public void Test_Reduce(string filename)
    {
        string[] lines = File.ReadAllLines(filename);
        int rows = lines.Length;
        int cols = lines[0].Length;
        var component = new int[rows, cols];
        int last_component = 0;
        void Dump()
        {

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    if (lines[row][col] == '#')
                        Console.Write(' ');
                    else if (lines[row][col] == '.' && component[row, col] != 0)
                        Console.Write((char)('a' + component[row, col]));
                    else
                        Console.Write(lines[row][col]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
        void walk((int row, int col) prev, (int row, int col) curr)
        {
            while (true)
            {
                var next = new[]
                {
                    (row:curr.row - 1, col: curr.col),
                    (row:curr.row + 1, col: curr.col),
                    (row:curr.row , col: curr.col- 1),
                    (row:curr.row , col: curr.col+ 1),
                }
                    .Where(x => x.row != 0)
                    .Where(x => x.row + 1 != rows)
                    .Where(x => x != prev)
                    .Where(x => !"#".Contains(lines[x.row][x.col]))
                    .ToList();
                if (next.Count > 1)
                    break;
                Trace.Assert(component[curr.row, curr.col] == 0);
                component[curr.row, curr.col] = last_component;
                if (next.Count == 0)
                    break;
                prev = curr;
                curr = next.Single();
            }
        }
        for (int row = 0; row < rows; row++)
        {
            if (row == 0)
                continue;
            if (row == rows - 1)
                continue;
            for (int col = 0; col < cols; col++)
            {
                if (component[row, col] != 0)
                    continue;
                if ("#".Contains(lines[row][col]))
                    continue;
                var next = new[]
                {
                    (row:row - 1, col: col),
                    (row:row + 1, col: col),
                    (row:row , col: col- 1),
                    (row:row , col: col+ 1),
                }
                    .Where(x => x.row != 0)
                    .Where(x => x.row + 1 != rows)
                    .Where(x => !"#".Contains(lines[x.row][x.col]))
                    .ToList();
                if (next.Count > 2)
                    continue;
                last_component += 1;
                component[row, col] = last_component;
                foreach (var x in next)
                {
                    walk((row, col), x);
                }
            }
        }
        Dump();
    }

    public readonly record struct Graph(Dictionary<(int irow, int icol), Dictionary<(int irow, int icol), int>> v);

    public static Graph LoadGraph(string filename)
    {
        var paths =
            File.ReadLines(filename)
            .SelectMany((row, irow) => row.Select((ch, icol) => (irow, icol, ch)))
            .Where(x => x.ch != '#')
            .Select(x => (x.irow, x.icol))
            .ToImmutableHashSet();
        return new(
            paths
            .Select(x => (
                x.irow,
                x.icol,
                edges: new (int irow, int icol)[]
                {
                    (x.irow-1, x.icol),
                    (x.irow+1, x.icol),
                    (x.irow, x.icol-1),
                    (x.irow, x.icol+1),
                }))
            .ToDictionary(
                x => (x.irow, x.icol),
                x =>
                    x.edges
                    .Where(e => paths.Contains((e.irow, e.icol)))
                    .ToDictionary(
                        x => (x.irow, x.icol),
                        _ => 1)));
    }
    public static void Reduce(Graph graph)
    {
        foreach (var x in graph.v)
        {
            if (x.Value.Count == 2)
            {
                var n1 = x.Value.First();
                var n2 = x.Value.Last();

                graph.v.Remove(x.Key);
                graph.v[n1.Key].Remove(x.Key);
                graph.v[n2.Key].Remove(x.Key);
                graph.v[n1.Key].Add(n2.Key, n1.Value + n2.Value);
                graph.v[n2.Key].Add(n1.Key, n1.Value + n2.Value);
            }
        }
    }
    public static int? Longest(Graph graph)
    {
        int target_irow = graph.v.Max(x => x.Key.irow);
        HashSet<(int irow, int icol)> visited = new();
        int? impl((int irow, int icol) curr)
        {
            if (visited.Contains(curr))
                return null;
            if (curr.irow == target_irow)
                return 0;

            visited.Add(curr);
            int? result = null;
            foreach (var e in graph.v[curr])
            {
                //var rec = impl(e.Key);
                //result = new[] { result, rec + e.Value }.Max();

                var rec = impl(e.Key);
                if (rec.HasValue)
                {
                    if (result.HasValue)
                    {
                        result = Math.Max(result.Value, rec.Value + e.Value);
                    }
                    else
                    {
                        result = rec.Value + e.Value;
                    }
                }
            }
            visited.Remove(curr);
            return result;
        }
        return impl((0, 1));
    }

    [Fact] public void Test_part1_example() => Assert.Equal(94, Longest("example.txt", part: 1));
    [Fact] public void Test_part1_input() => Assert.Equal(2414, Longest("input.txt", part: 1));
    [Fact] public void Test_part2_example() => Assert.Equal(154, Longest("example.txt", part: 2));
    [Fact]
    public void Test_part2_reduced_example()
    {
        var g = LoadGraph("example.txt");
        Reduce(g);
        Assert.Equal(154, Longest(g));
    }
    [Fact]
    public void Test_part2_reduced_input()
    {
        var g = LoadGraph("input.txt");
        Reduce(g);
        Assert.Equal(6598, Longest(g));
    }

    public int Longest(string filename, int part)
    {
        string[] lines = File.ReadAllLines(filename);
        int nrows = lines.Length;
        int ncols = lines[0].Length;
        HashSet<(int irow, int icol)> visited = new();
        int impl((int irow, int icol) prev, (int irow, int icol) curr)
        {
            if (visited.Contains(curr))
                return 0;
            if (curr.irow < 0)
                return 0;
            if (nrows <= curr.irow)
                return 0;
            if (curr.icol < 0)
                return 0;
            if (ncols <= curr.icol)
                return 0;
            if (lines[curr.irow][curr.icol] == '#')
                return 0;
            if (part == 1)
            {
                if (lines[curr.irow][curr.icol] == '>' && prev.icol + 1 != curr.icol)
                    return 0;
                if (lines[curr.irow][curr.icol] == 'v' && prev.irow + 1 != curr.irow)
                    return 0;
            }
            if (curr == (nrows - 1, ncols - 2))
                return 1;

            visited.Add(curr);
            int result = 0;
            result = Math.Max(result, impl(curr, (curr.irow + 1, curr.icol)));
            result = Math.Max(result, impl(curr, (curr.irow - 1, curr.icol)));
            result = Math.Max(result, impl(curr, (curr.irow, curr.icol + 1)));
            result = Math.Max(result, impl(curr, (curr.irow, curr.icol - 1)));
            visited.Remove(curr);
            if (result == 0)
                return 0;
            return result + 1;
        }
        TaskCompletionSource<int> source = new();
        new Thread(() => source.SetResult(impl((0, 1), (1, 1))), 111222333).Start();
        return source.Task.Result;
    }
}