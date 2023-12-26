using System.Diagnostics;
using Xunit;

Console.WriteLine(new Day23().Longest("input.txt", part: 2));

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

    [Fact] public void Test_part1_example() => Assert.Equal(94, Longest("example.txt", part: 1));
    [Fact] public void Test_part1_input() => Assert.Equal(2414, Longest("input.txt", part: 1));
    [Fact] public void Test_part2_example() => Assert.Equal(154, Longest("example.txt", part: 2));
    [Fact] public void Test_part2_input() => Assert.Equal(0000, Longest("input.txt", part: 2));
    public long Longest(string filename, int part)
    {
        string[] lines = File.ReadAllLines(filename);
        int rows = lines.Length;
        int cols = lines[0].Length;
        HashSet<(int row, int col)> visited = new();
        long impl((int row, int col) prev, (int row, int col) curr)
        {
            if (visited.Contains(curr))
                return 0;
            if (curr.row < 0)
                return 0;
            if (rows <= curr.row)
                return 0;
            if (curr.col < 0)
                return 0;
            if (cols <= curr.col)
                return 0;
            if (lines[curr.row][curr.col] == '#')
                return 0;
            if (part == 1)
            {
                if (lines[curr.row][curr.col] == '>' && prev.col + 1 != curr.col)
                    return 0;
                if (lines[curr.row][curr.col] == 'v' && prev.row + 1 != curr.row)
                    return 0;
            }
            if (curr == (rows - 1, cols - 2))
                return 1;

            visited.Add(curr);
            long result = 0;
            result = Math.Max(result, impl(curr, (curr.row + 1, curr.col)));
            result = Math.Max(result, impl(curr, (curr.row - 1, curr.col)));
            result = Math.Max(result, impl(curr, (curr.row, curr.col + 1)));
            result = Math.Max(result, impl(curr, (curr.row, curr.col - 1)));
            visited.Remove(curr);
            if (result == 0)
                return 0;
            return result + 1;
        }
        TaskCompletionSource<long> source = new();
        new Thread(() => source.SetResult(impl((0, 1), (1, 1))), 111222333).Start();
        return source.Task.Result;
    }
}