<Query Kind="Statements" />

var lines = File.ReadLines("input.txt");
var instructions = lines.First();
var nodes = lines
	.Skip(2)
	.ToDictionary(x => x.Substring(0, 3), x => new { L = x.Substring(7, 3), R = x.Substring(12, 3), });
long steps = 0;
string curr = "AAA";
while (true)
{
	foreach (var instruction in instructions)
	{
		if (curr == "ZZZ")
			goto found;
		var node = nodes[curr];
		if (instruction == 'L')
			curr = node.L;
		else
			curr = node.R;
		steps++;
	}
}
found: { }
steps.Dump();