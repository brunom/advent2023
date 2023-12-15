string text = File.ReadAllText("input.txt");
//string text = File.ReadAllText("example.txt");
int ncols = text.IndexOf('\n');
int nrows = text.Length / (ncols + 1); // + 1 is \n
char get(int row, int col)
{
    if (row < 0 || col < 0 || nrows <= row || ncols <= col)
        return '.';
    return text[(ncols + 1) * row + col]; // + 1 is \n
}

int number(int row, int col) // 0 when not a number
{
    if (!char.IsNumber(get(row, col)))
        return 0;
    while (char.IsNumber(get(row, col - 1)))
    {
        col--;
    }
    int result = 0;
    while (true)
    {
        char ch = get(row, col);
        if (!char.IsNumber(ch))
            break;
        result = result * 10 + (ch - '0');
        col++;
    }
    return result;
}

int sum = 0;
for (int row = 0; row < nrows; row++)
{
    for (int col = 0; col < ncols; col++)
    {
        if (get(row, col) != '*')
            continue;
        int nparts = 0;
        int ratio = 1;

        int acc(int number)
        {
            if (number != 0)
            {
                nparts++;
                ratio *= number;
            }
            return number;
        }

        acc(number(row, col - 1));
        acc(number(row, col + 1));
        if (acc(number(row - 1, col)) == 0)
        {
            acc(number(row - 1, col - 1));
            acc(number(row - 1, col + 1));
        }
        if (acc(number(row + 1, col)) == 0)
        {
            acc(number(row + 1, col - 1));
            acc(number(row + 1, col + 1));
        }

        if (nparts == 2)
        {
            sum += ratio;
        }
    }
}
Console.WriteLine(sum);
