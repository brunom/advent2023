var lines = File.ReadLines("input.txt");
Console.WriteLine(lines.Select(x => x.Split().Select(x => long.Parse(x))).Select(Extrapolate).Sum());

static long Extrapolate(IEnumerable<long> numbers)
{
    var arr = numbers.ToArray();
    int len = arr.Length;
    long result = 0;
    while (len > 0)
    {
        for (int i = 0; i < len - 1; i++)
        {
            arr[i] = arr[i + 1] - arr[i];
        }
        result += arr[len - 1];
        len--;
    }
    return result;
}
