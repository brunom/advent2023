using System.Diagnostics;
using Xunit;

public class day16
{
    long Energized(string filename)
    {
        var lines = File.ReadAllLines(filename);
        var rows = lines.Length;
        var cols = lines[0].Length;

        List<(int row, int col, char dir)> list = new();
        list.Add((0, 0, 'E'));
        var set = list.ToHashSet();

        for (int i = 0; i < list.Count; i++)
        {
            var t = list[i];

            void add(int row, int col, char dir)
            {
                if (set.Add((row, col, dir)))
                    list.Add((row, col, dir));
            }

            void move(char newdir)
            {
                switch (newdir)
                {
                    case 'N' when 0 < t.row: add(t.row - 1, t.col, newdir); break;
                    case 'S' when t.row + 1 < rows: add(t.row + 1, t.col, newdir); break;
                    case 'W' when 0 < t.col: add(t.row, t.col - 1, newdir); break;
                    case 'E' when t.col + 1 < cols: add(t.row, t.col + 1, newdir); break;
                }
            }

            switch (lines[t.row][t.col], t.dir)
            {
                case ('.', _): move(t.dir); break;
                case ('|', 'N'): move(t.dir); break;
                case ('|', 'S'): move(t.dir); break;
                case ('-', 'W'): move(t.dir); break;
                case ('-', 'E'): move(t.dir); break;
                case ('/', 'N'): move('E'); break;
                case ('/', 'S'): move('W'); break;
                case ('/', 'W'): move('S'); break;
                case ('/', 'E'): move('N'); break;
                case ('\\', 'N'): move('W'); break;
                case ('\\', 'S'): move('E'); break;
                case ('\\', 'W'): move('N'); break;
                case ('\\', 'E'): move('S'); break;
                case ('|', 'W'): move('N'); move('S'); break;
                case ('|', 'E'): move('N'); move('S'); break;
                case ('-', 'N'): move('W'); move('E'); break;
                case ('-', 'S'): move('W'); move('E'); break;
                default: throw new NotImplementedException();
            }
        }
        Trace.Assert(list.Count == set.Count);
        return
            list
            .Select(t => (t.row, t.col))
            .Distinct()
            .Count();
    }
    [Fact] public void Test_part1_example() => Assert.Equal(46, Energized("example.txt"));
    [Fact] public void Test_part1_input() => Assert.Equal(7951, Energized("input.txt"));
}
