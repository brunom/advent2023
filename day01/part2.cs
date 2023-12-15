var lines = File.ReadLines("input.txt");
int sum = 0;
foreach (var line in lines)
{
	int? first = null;
	int? last = null;
	for (int i = 0; i < line.Length; ++i)
	{
		char ch = line[i];
		if (line.Substring(i).StartsWith("one")) ch = '1';
		if (line.Substring(i).StartsWith("two")) ch = '2';
		if (line.Substring(i).StartsWith("three")) ch = '3';
		if (line.Substring(i).StartsWith("four")) ch = '4';
		if (line.Substring(i).StartsWith("five")) ch = '5';
		if (line.Substring(i).StartsWith("six")) ch = '6';
		if (line.Substring(i).StartsWith("seven")) ch = '7';
		if (line.Substring(i).StartsWith("eight")) ch = '8';
		if (line.Substring(i).StartsWith("nine")) ch = '9';
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