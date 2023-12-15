var lines = File.ReadLines("input.txt");
int sum = 0;
foreach (var line in lines)
{
	int? first = null;
	int? last = null;
	foreach (var ch in line)
	{
		if ('0' <= ch && ch <= '9')
		{
			if (first == null)
				first = ch - '0';
			last = ch - '0';
		}
	}
	sum += first.Value * 10 + last.Value;
}
sum.Dump();
