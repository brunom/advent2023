<Query Kind="Program" />

struct Range
{
	public long Start;
	public long End;
};

void Main()
{
	Directory.SetCurrentDirectory(@"C:\tools\advent2023\05");

	var lines = File.ReadLines("input.txt");
	var e = lines.GetEnumerator();
	e.MoveNext();
	var narr = e.Current.Split();
	var numbers = new List<Range>();
	for (int i = 1; i + 1 < narr.Length; i += 2)
	{
		long start = long.Parse(narr[i]);
		long length = long.Parse(narr[i + 1]);
		numbers.Add(new Range { Start = start, End = start + length, });
	}
	e.MoveNext(); // \n

	while (e.MoveNext()) // ".* map:"
	{
		var mapped = new List<Range>();
		while (true)
		{
			e.MoveNext();
			if (string.IsNullOrWhiteSpace(e.Current))
				break;
			var arr = e.Current.Split();
			var dest_start = long.Parse(arr[0]);
			var src_start = long.Parse(arr[1]);
			var length = long.Parse(arr[2]);
			var src_end = src_start + length;
			var bump = dest_start - src_start;

			var same = new List<Range>();
			foreach (var n in numbers)
			{
				if (n.Start < src_start)
				{
					same.Add(new Range { Start = n.Start, End = Math.Min(n.End, src_start), });
				}
				if (src_start < n.End && n.Start < src_end)
				{
					mapped.Add(new Range { Start = Math.Max(n.Start, src_start) + bump, End = Math.Min(n.End, src_end) + bump, });
				}
				if (src_end < n.End)
				{
					same.Add(new Range { Start = Math.Max(n.Start, src_end), End = n.End, });
				}
			}
			numbers = same;
		}
		numbers.AddRange(mapped);
	}
	numbers.Select(x => x.Start).Min().Dump();
}
