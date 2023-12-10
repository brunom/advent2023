var sketch = File.ReadLines("input.txt").Select(x => x.ToArray()).ToArray();
int rows = sketch.Length;
int cols = sketch[0].Length;

var coordinates = Enumerable.Range(0, rows).SelectMany(row => Enumerable.Range(0, cols).Select(col => (row, col)));
var starting = coordinates.Where(coord => sketch[coord.row][coord.col] == 'S').Single();
sketch[starting.row][starting.col] =
    "|-F7LJ"
    .AsEnumerable()
    .Where(ch => !"|LJ".Contains(ch) || (starting.row != 0 && "|F7".Contains(sketch[starting.row - 1][starting.col])))
    .Where(ch => !"|F7".Contains(ch) || (starting.row + 1 != rows && "|LJ".Contains(sketch[starting.row + 1][starting.col])))
    .Where(ch => !"-7J".Contains(ch) || (starting.col != 0 && "-FL".Contains(sketch[starting.row][starting.col - 1])))
    .Where(ch => !"-FL".Contains(ch) || (starting.col + 1 != cols && "-7J".Contains(sketch[starting.row][starting.col + 1])))
    .Single();

SortedSet<(int row, int col)> visited = new();
HashSet<(int row, int col)> wavefront = new();
wavefront.Add(starting);
for (int dist = 0; ; dist++)
{
    foreach (var coord in wavefront)
    {
        visited.Add(coord);
    }

    wavefront = new(
        from coord in wavefront
        from rct in new (int row, int col, string tiles0, string tiles1)[] {
            (coord.row + 1, coord.col, "|7F", "|LJ"),
            (coord.row - 1, coord.col, "|LJ", "|7F"),
            (coord.row, coord.col + 1, "-LF", "-J7"),
            (coord.row, coord.col - 1, "-J7", "-LF"),
        }
        where !visited.Contains((rct.row, rct.col))
        where 0 <= rct.row
        where rct.row < rows
        where 0 <= rct.col
        where rct.col < cols
        where rct.tiles0.Contains(sketch[coord.row][coord.col])
        where rct.tiles1.Contains(sketch[rct.row][rct.col])
        select (rct.row, rct.col));

    if (!wavefront.Any())
    {
        Console.WriteLine(dist); // !15
        break;
    }
}

int count = 0;
for (int row = 0; row < rows; row++)
{
    inout inout = inout.outside;
    for (int col = 0; col < cols; col++)
    {
        if (false) { }
        else if (inout == inout.outside)
        {
            if (visited.Contains((row, col)))
            {
                if ("|".Contains(sketch[row][col]))
                {
                    inout = inout.middle;
                }
                else if ("F".Contains(sketch[row][col]))
                {
                    inout = inout.crawl;
                }
                else if ("L".Contains(sketch[row][col]))
                {
                    inout = inout.fly;
                }
                else throw null;
            }
        }
        else if (!visited.Contains((row, col)))
        {
            count++;
            inout = inout.middle;
        }
        else if (inout == inout.middle)
        {
            if (false) { }
            else if ("|7J".Contains(sketch[row][col]))
            {
                inout = inout.outside;
            }
            else if ("F".Contains(sketch[row][col]))
            {
                inout = inout.fly;
            }
            else if ("L".Contains(sketch[row][col]))
            {
                inout = inout.crawl;
            }
            else throw null;
        }
        else if (inout == inout.fly)
        {
            if (false) { }
            else if ("7".Contains(sketch[row][col]))
            {
                inout = inout.middle;
            }
            else if ("|J".Contains(sketch[row][col]))
            {
                inout = inout.outside;
            }
        }
        else if (inout == inout.crawl)
        {
            if (false) { }
            else if ("J".Contains(sketch[row][col]))
            {
                inout = inout.middle;
            }
            else if ("|7".Contains(sketch[row][col]))
            {
                inout = inout.outside;
            }
        }
        else throw null;
    }
    if (inout != inout.outside)
        throw null;
}

Console.WriteLine(
    count
);

enum inout
{
    outside,
    fly,
    middle,
    crawl,
};
