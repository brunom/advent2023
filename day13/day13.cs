using System.IO;
using System.Linq;
using Xunit;

public class day13
{
    long Line(string pattern, int rows, int cols, int stride_rows, int stride_cols, int smudges)
    {
        long result = 0;
        for (int row = 1; row < rows; ++row)
        {
            int smudges1 = smudges;
            for (int curr = 0; curr < row && curr < rows - row; ++curr)
            {
                for (int col = 0; col < cols; ++col)
                {
                    if (pattern[(row - curr - 1) * stride_rows + col * stride_cols] !=
                        pattern[(row + curr) * stride_rows + col * stride_cols])
                    {
                        if (smudges1 > 0)
                        {
                            smudges1 -= 1;
                        }
                        else
                        {
                            goto next_row;
                        }
                    }
                }
            }
            if (smudges1 > 0)
                goto next_row;
            result += row;
        next_row: { }
        }
        return result;
    }
    long Line(string pattern, int smudges)
    {
        int cols = pattern.IndexOf('\n');
        int rows = pattern.Length / (cols + "\n".Length);
        long line_rows = Line(pattern, rows, cols, cols + "\n".Length, 1, smudges);
        long line_cols = Line(pattern, cols, rows, 1, cols + "\n".Length, smudges);
        return 100 * line_rows + line_cols;
    }
    long LinesFile(string filename, int smudges = 0)
    {
        return
            File.ReadAllText(filename)
            .Split("\n\n")
            .Select(x => x + "\n")
            .Select(x => Line(x, smudges))
            .Sum();
    }

    [Fact]
    public void part1_Example()
    {
        Assert.Equal(405, LinesFile("example.txt"));
    }
    [Fact]
    public void part2_Example()
    {
        Assert.Equal(400, LinesFile("example.txt", smudges: 1));
    }
    [Fact]
    public void part1_Input()
    {
        Assert.Equal(33122, LinesFile("input.txt"));
    }
    [Fact]
    public void part2_Input()
    {
        Assert.Equal(32312, LinesFile("input.txt", smudges: 1));
    }
}
