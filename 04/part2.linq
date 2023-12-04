<Query Kind="Statements" />

Directory.SetCurrentDirectory(@"C:\tools\advent2023\04");
var lines = File.ReadAllLines("input.txt");
var copies = new int[lines.Length];
long sum = 0;
for (int iline = 0; iline < lines.Length; ++iline)
{
	var match = Regex.Match(lines[iline], @"^Card\s+(?<card>\d+):\s+((?<winning>\d+)\s+)+\|(\s+(?<have>\d+))+$");
	var card = match.Groups["card"].Value;
	var winning = match.Groups["winning"].Captures.Cast<Capture>().Select(x => x.Value);
	var have = match.Groups["have"].Captures.Cast<Capture>().Select(x => x.Value);
	int count = winning.Intersect(have).Count();

	for (int iline1 = iline + 1; iline1 < lines.Length && iline1 < iline + 1 + count; iline1++)
	{
		copies[iline1] += (copies[iline] + 1);
	}
	sum += copies[iline] + 1;
}
sum.Dump();