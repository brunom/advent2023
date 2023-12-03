
var input = (""
    + "Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green\n"
    + "Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue\n"
    + "Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red\n"
    + "Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red\n"
    + "Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green\n"
    ).Split('\n').AsEnumerable();

Directory.SetCurrentDirectory(@"c:\tools\advent2023\02");
input = File.ReadLines("input.txt");

int sum = 0;
foreach (string line in input)
{
    if (line.Split(':') is [string game_n, string sets])
    {
        int game = int.Parse(game_n.Split(' ')[1]);
        int red = 0;
        int green = 0;
        int blue = 0;

        foreach (var set in sets.Split(';'))
        {
            foreach (var cube in set.Split(','))
            {
                if (cube.Split(' ', StringSplitOptions.RemoveEmptyEntries) is [string number, string color])
                {
                    ref int acc =
                        ref color == "red" ? ref red :
                        ref color == "green" ? ref green :
                        ref blue;
                    acc = Math.Max(acc, int.Parse(number));
                }
            }
        }
        sum += red * green * blue;
    }
}
Console.WriteLine(sum);
