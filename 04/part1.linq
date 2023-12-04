<Query Kind="Statements" />

Directory.SetCurrentDirectory(@"C:\tools\advent2023\04");
long sum = 0;
foreach (var line in File.ReadLines("input.txt"))
{
	var match = Regex.Match(line, @"^Card\s+(?<card>\d+):\s+((?<winning>\d+)\s+)+\|(\s+(?<have>\d+))+$");
	var card = match.Groups["card"].Value;
	var winning = match.Groups["winning"].Captures.Cast<Capture>().Select(x => x.Value);
	var have = match.Groups["have"].Captures.Cast<Capture>().Select(x => x.Value);
	int count = winning.Intersect(have).Count();
	sum += ((1 << count) >> 1);	
}
sum.Dump();