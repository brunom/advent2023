using Xunit;

public class day14
{
    int TiltedLoad(string file)
    {
        var lines = File.ReadAllLines(file);
        int rows = lines.Length;
        int cols = lines[0].Length;
        int load = 0;
        for (int col = 0; col < cols; col++)
        {
            int nrounded = 0;
            for (int row = rows; row > 0; row--)
            {
                char ch = lines[row - 1][col];
                if (ch == '#')
                {
                    load += (rows - row) * (rows - row + 1) / 2;
                    load -= (rows - row - nrounded) * (rows - row + 1 - nrounded) / 2;
                    nrounded = 0;
                }
                else if (ch == 'O')
                {
                    nrounded++;
                }
            }
            load += (rows) * (rows + 1) / 2;
            load -= (rows - nrounded) * (rows + 1 - nrounded) / 2;
        }
        return load;
    }
    int TiltThenLoad(string file)
    {
        var platform = File.ReadAllBytes(file);
        int cols = platform.Select((v, i) => (v, i)).Where(p => p.v == '\n').Select(p => p.i).First();
        int rows = platform.Length / (cols + "\n".Length);
        TiltNorth(platform, rows, cols, cols + "\n".Length, 1);
        return Load(platform, rows, cols, cols + "\n".Length, 1);
    }
    unsafe public static int CycleThenLoad(string file, int cycles)
    {
        var platform = File.ReadAllBytes(file);
        int cols = platform.Select((v, i) => (v, i)).Where(p => p.v == '\n').Select(p => p.i).First();
        int rows = platform.Length / (cols + "\n".Length);
        Dictionary<string, int> seen = new();
        List<int> loads = new();
        for (int cycle = 0; cycle < cycles; ++cycle)
        {
            Cycle(platform, rows, cols, cols + "\n".Length, 1);
            string str;
            fixed (byte* ptr = &platform[0])
            {
                str = new((sbyte*)ptr);
            }
            if (!seen.TryAdd(str, cycle))
            {
                var prev = seen[str];
                return loads[prev + (cycles - cycle - 1) % (cycle - prev)];
            }
            loads.Add(Load(platform, rows, cols, cols + "\n".Length, 1));
        }
        return loads.Last();
    }

    static void TiltNorth(byte[] platform, int rows, int cols, int stride_rows, int stride_cols, int origin = 0)
    {
        for (int col = 0; col < cols; col++)
        {
            int nrounded = 0;
            for (int row = rows; row > 0; row--)
            {
                ref byte ch = ref platform[(row - 1) * stride_rows + col * stride_cols + origin];
                if (ch == '#')
                {
                    for (int i = 0; i < nrounded; i++)
                    {
                        platform[(row + i) * stride_rows + col * stride_cols + origin] = (byte)'O';
                    }
                    nrounded = 0;
                }
                else if (ch == 'O')
                {
                    nrounded++;
                    ch = (byte)'.';
                }
            }
            for (int i = 0; i < nrounded; i++)
            {
                platform[i * stride_rows + col * stride_cols + origin] = (byte)'O';
            }
        }
    }
    static void Cycle(byte[] platform, int rows, int cols, int stride_rows, int stride_cols)
    {
        TiltNorth(platform, rows, cols, stride_rows, stride_cols);
        TiltNorth(platform, cols, rows, stride_cols, stride_rows);
        TiltNorth(platform, rows, cols, -stride_rows, -stride_cols, platform.Length - 2);
        TiltNorth(platform, cols, rows, -stride_cols, -stride_rows, platform.Length - 2);
    }
    static int Load(byte[] platform, int rows, int cols, int stride_rows, int stride_cols)
    {
        int result = 0;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (platform[row * stride_rows + col * stride_cols] == 'O')
                {
                    result += rows - row;
                }
            }
        }
        return result;
    }
    [Fact]
    public void TiltedLoad_example() =>
        Assert.Equal(136, TiltedLoad("example.txt"));
    [Fact]
    public void TiltedLoad_input() =>
        Assert.Equal(103333, TiltedLoad("input.txt"));

    [Fact]
    public void TiltThenLoad_example() =>
        Assert.Equal(136, TiltThenLoad("example.txt"));

    [Fact]
    public void TiltThenLoad_input() =>
        Assert.Equal(103333, TiltThenLoad("input.txt"));
    [Fact]
    public void CycleThenLoad_example() =>
        Assert.Equal(64, CycleThenLoad("example.txt", 1000000000));
    [Fact]
    public void CycleThenLoad_input() =>
        Assert.Equal(97241, CycleThenLoad("input.txt", 1000000000));
}
