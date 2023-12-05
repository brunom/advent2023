<Query Kind="Statements" />

Directory.SetCurrentDirectory(@"C:\tools\advent2023\05");

var lines = File.ReadLines("example.txt");
var e = lines.GetEnumerator();
e.MoveNext();
var narr = e.Current.Split();
List<long> numbers = new List<long>();
for (int i = 1; i + 1 < narr.Length; i += 2)
{
	numbers.AddRange(Enumerable.Range(int.Parse(narr[i]), int.Parse(narr[i+1])).Select(x => (long)x));
}
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
		numbers.AddRange(mapped);
	}
numbers.Min().Dump();