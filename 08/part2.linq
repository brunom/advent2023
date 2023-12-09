<Query Kind="Statements" />

Directory.SetCurrentDirectory(@"C:\tools\advent2023\08");
var lines = File.ReadLines("input.txt");
string instructions = lines.First();
var nodes = lines
	.Skip(2)
	.ToDictionary(x => x.Substring(0, 3), x => new { L = x.Substring(7, 3), R = x.Substring(12, 3), });
long steps = 0;
var curr = new HashSet<string>(nodes.Select(x => x.Key).Where(x => x.Last() == 'A'));
while (true)
{
	foreach (var instruction in instructions)
	{
		if (curr.All(x => x.Last() == 'Z'))
			goto found;
		curr = new HashSet<string>(curr.Select(x => nodes[x]).Select(x => instruction == 'L' ? x.L : x.R));
		steps++;
	}
}
found: { }
steps.Dump();