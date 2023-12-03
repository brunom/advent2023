string text = File.ReadAllText("input.txt");
//string text = File.ReadAllText("example.txt");
int ncols = text.IndexOf('\n');
int nrows = text.Length / (ncols + 1); // + 1 is \n
int sum = 0;
char get(int row, int col)
{
    if (row < 0 || col < 0 || nrows <= row || ncols <= col)
        return '.';
    return text[(ncols + 1) * row + col]; // + 1 is \n
}
bool symbol(char ch) => ch != '.' && !char.IsNumber(ch);

int number = 0;
bool adjacent = false;
for (int row = 0; row < nrows; row++)
{
    Trace.Assert(number == 0);
    for (int col = 0; col < ncols + 1; col++) // + 1 to flush number
    {
        char ch = get(row, col);
        if (char.IsNumber(ch))
        {
            number = number * 10 + ch - '0';
            adjacent = adjacent
                || symbol(get(row - 1, col - 1))
                || symbol(get(row - 1, col + 0))
                || symbol(get(row - 1, col + 1))
                || symbol(get(row + 0, col - 1))
                || symbol(get(row + 0, col + 1))
                || symbol(get(row + 1, col - 1))
                || symbol(get(row + 1, col + 0))
                || symbol(get(row + 1, col + 1))
                ;
        }
        else
        {
            if (adjacent)
            {
                sum += number;
            }

            number = 0;
            adjacent = false;
        }
    }
}
Trace.Assert(number == 0);
Console.WriteLine(sum);