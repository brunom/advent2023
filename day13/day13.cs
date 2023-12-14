using System.IO;
using System.Linq;
using Xunit;

public class day13
{
    long Line(string pattern, int rows, int cols, int stride_rows, int stride_cols)
    {
        long result = 0;
        for (int row = 1; row < rows; ++row)
        {
            for (int curr = 0; curr < row && curr < rows - row; ++curr)
            {
                for (int col = 0; col < cols; ++col)
                {
                    if (pattern[(row - curr - 1) * stride_rows + col * stride_cols] !=
                        pattern[(row + curr) * stride_rows + col * stride_cols])
                    {
                        goto next_row;
                    }
                }
            }
            result += row;
        next_row: { }
        }
        return result;
    }
    long Line(string pattern)
    {
        int cols = pattern.IndexOf('\n');
        int rows = pattern.Length / (cols + "\n".Length);
        long line_rows = Line(pattern, rows, cols, cols + "\n".Length, 1);
        long line_cols = Line(pattern, cols, rows, 1, cols + "\n".Length);
        return 100 * line_rows + line_cols;
    }
    long LinesFile(string filename)
    {
        return
            File.ReadAllText(filename)
            .Split("\n\n")
            .Select(x => x + "\n")
            .Select(Line)
            .Sum();
    }

    [Fact]
    public void Example()
    {
        Assert.Equal(405, LinesFile("example.txt"));
    }
    [Fact]
    public void Input()
    {
        Assert.Equal(33122, LinesFile("input.txt")); //16784 low
    }
}
