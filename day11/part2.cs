var lines = File.ReadAllLines("input.txt");
var rows = lines.Length;
var cols = lines[0].Length;

var galaxies = (
	from row in Enumerable.Range(0, rows)
	from col in Enumerable.Range(0, cols)
	where lines[row][col] == '#'
	select (row, col))
	.ToList();

var busy_rows = new HashSet<int>(galaxies.Select(g => g.row));
var busy_cols = new HashSet<int>(galaxies.Select(g => g.col));

long distance((int row, int col) lhs, (int row, int col) rhs)
{
	int minrow = Math.Min(lhs.row, rhs.row);
	int maxrow = Math.Max(lhs.row, rhs.row);
	int mincol = Math.Min(lhs.col, rhs.col);
	int maxcol = Math.Max(lhs.col, rhs.col);
	return 0
		+ maxrow - minrow
		+ maxcol - mincol
		+ Enumerable.Range(minrow, maxrow - minrow).Where(x => !busy_rows.Contains(x)).Count() * (1000000L - 1)
		+ Enumerable.Range(mincol, maxcol - mincol).Where(x => !busy_cols.Contains(x)).Count() * (1000000L - 1)
		;
}

Console.WriteLine((
	from g1 in galaxies
	from g2 in galaxies
	where g1.CompareTo(g2) < 0
	select distance(g1, g2))
	.Sum());
