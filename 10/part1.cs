var sketch = File.ReadAllLines("input.txt");
int rows = sketch.Length;
int cols = sketch[0].Length;

var coordinates = Enumerable.Range(0, rows).SelectMany(row => Enumerable.Range(0, cols).Select(col => (row, col)));
HashSet<(int row, int col)> visited = new();
HashSet<(int row, int col)> wavefront = new(coordinates.Where(coord => sketch[coord.row][coord.col] == 'S'));
for (int dist = 0; ; dist++)
{
    foreach (var coord in wavefront)
    {
        visited.Add(coord);
    }

    wavefront = new(
        from coord in wavefront
        from rct in new (int row, int col, string tiles0, string tiles1)[] {
            (coord.row + 1, coord.col, "S|7F", "|LJ"),
            (coord.row - 1, coord.col, "S|LJ", "|7F"),
            (coord.row, coord.col + 1, "S-LF", "-J7"),
            (coord.row, coord.col - 1, "S-J7", "-LF"),
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
