var rows = new SortedDictionary<int, int>();
var cols = new SortedDictionary<int, int>();
int ngalaxies = 0;
long factor = 1000000;

long DistSum(IEnumerable<KeyValuePair<int, int>> e)
{
	long result = 0;
	int prev_galaxies = 0;
	int next_galaxies = ngalaxies;
	int lastgalaxy = 0;
	foreach (var kv in e)
	{
		result += prev_galaxies * next_galaxies * (1 + Math.Max(0, kv.Key - lastgalaxy - 1) * factor);
		prev_galaxies += kv.Value;
		next_galaxies -= kv.Value;
		lastgalaxy = kv.Key;
	}
	return result;
}

using (var file = File.OpenText("input.txt"))
{
	int row = 0;
	int col = 0;
	int row_galaxies = 0;
	while (true)
	{
		col += 1;
		switch (file.Read())
		{
			case '.':
				break;
			case '#':
				ngalaxies += 1;
				row_galaxies += 1;
				cols.TryGetValue(col - 1, out int old);
				cols[col - 1] = old + 1;
				break;
			case '\n':
				if (row_galaxies > 0)
				{
					rows.Add(row, row_galaxies);
					row_galaxies = 0;
				}
				row += 1;
				col = 0;
				break;
			case -1:
				Trace.Assert(row_galaxies == 0);
				goto eof;
			default:
				Trace.Assert(false);
				break;
		}
	}
eof: { }
	Console.WriteLine(DistSum(rows) + DistSum(cols));
}
