<Query Kind="Statements" />

var lines = File.ReadLines("input.txt");
var e = lines.GetEnumerator();
e.MoveNext();
var numbers = e.Current.Split()
	.Skip(1) // "seeds:"
	.Select(x => long.Parse(x));

e.MoveNext(); // \n

while (e.MoveNext()) // ".* map:"
{
	var mapped = new List<long>();
	while (true)
	{
		e.MoveNext();
		if (string.IsNullOrWhiteSpace(e.Current))
			break;
		var arr = e.Current.Split();
		var dest = long.Parse(arr[0]);
		var src = long.Parse(arr[1]);
		var length = long.Parse(arr[2]);
		var same = new List<long>();
		foreach (var n in numbers)
		{
			if (src <= n && n < src + length)
			{
				mapped.Add(dest + n - src);
			}
			else
			{
				same.Add(n);
			}
		}
		numbers = same;
	}
	numbers = numbers.Concat(mapped);
}
numbers.Min().Dump();