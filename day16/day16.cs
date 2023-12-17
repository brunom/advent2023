using Xunit;

public class day16
{
    long Energized(string filename)
    {
        var lines = File.ReadAllLines(filename);
        var rows = lines.Length;
        var cols = lines[0].Length;
        HashSet<(int row, int col, char dir)> set = new();
        List<(int row, int col, char dir)> list = new();
        list.Add((0, 0, 'E'));
        for (int i = 0; i < list.Count; i++)
        {
            var t = list[i];
            if (!set.Add(t))
                continue;
            void move(char newdir)
            {
                if (newdir == 'N' && 0 < t.row)
                    list.Add((t.row - 1, t.col, newdir));
                else if (newdir == 'S' && t.row + 1 < rows)
                    list.Add((t.row + 1, t.col, newdir));
                else if (newdir == 'W' && 0 < t.col)
                    list.Add((t.row, t.col - 1, newdir));
                else if (newdir == 'E' && t.col + 1 < cols)
                    list.Add((t.row, t.col + 1, newdir));
            }

            char ch = lines[t.row][t.col];
            if (ch == '.')
                move(t.dir);

            else if (ch == '|' && t.dir == 'N')
                move(t.dir);
            else if (ch == '|' && t.dir == 'S')
                move(t.dir);
            else if (ch == '-' && t.dir == 'W')
                move(t.dir);
            else if (ch == '-' && t.dir == 'E')
                move(t.dir);

            else if (ch == '/' && t.dir == 'N')
                move('E');
            else if (ch == '/' && t.dir == 'S')
                move('W');
            else if (ch == '/' && t.dir == 'W')
                move('S');
            else if (ch == '/' && t.dir == 'E')
                move('N');

            else if (ch == '\\' && t.dir == 'N')
                move('W');
            else if (ch == '\\' && t.dir == 'S')
                move('E');
            else if (ch == '\\' && t.dir == 'W')
                move('N');
            else if (ch == '\\' && t.dir == 'E')
                move('S');

            else if (ch == '|' && t.dir == 'W')
            {
                move('N');
                move('S');
            }
            else if (ch == '|' && t.dir == 'E')
            {
                move('N');
                move('S');
            }
            else if (ch == '-' && t.dir == 'N')
            {
                move('W');
                move('E');
            }
            else if (ch == '-' && t.dir == 'S')
            {
                move('W');
                move('E');
            }
            else throw new NotImplementedException();
        }
        return
            list
            .Select(t => (t.row, t.col))
            .Distinct()
            .Count();
    }
    [Fact] public void Test_part1_example() => Assert.Equal(46, Energized("example.txt"));
    [Fact] public void Test_part1_input() => Assert.Equal(7951, Energized("input.txt"));
}
