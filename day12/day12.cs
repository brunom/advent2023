using System.Collections.Immutable;
using Xunit;

Console.WriteLine(
    //new[] { ".???#??.?##?#??? 1,1,7" }
    //new[] { ".???#?? 1,1" }
    new[] { "?.#.? 1,1" }
    //new[] { ".??...? 2,1" }
    .Select(day12.Parse)
    .Select(x => (rec: day12.ArrangementsRec(x), tab: day12.ArrangementsTable(x)))
    .Select((p, i) => (p.rec, p.tab, i))
    .Where(x => x.rec != x.tab)
    .FirstOrDefault());
struct EquatableMemory<T> : IEquatable<EquatableMemory<T>>
{
    EquatableMemory(ReadOnlyMemory<T> m)
    {
        this.m = m;
    }
    public readonly ReadOnlyMemory<T> m;

    public static implicit operator ReadOnlyMemory<T>(EquatableMemory<T> d) => d.m;
    public static implicit operator EquatableMemory<T>(ReadOnlyMemory<T> d) => new(d);
    public static implicit operator EquatableMemory<T>(Memory<T> d) => new(d);

    override public bool Equals(object? obj)
    {
        if (obj is not EquatableMemory<T> other)
            return false;
        return Equals(other);
    }

    public bool Equals(EquatableMemory<T> other)
    {
        return MemoryExtensions.SequenceEqual(this.m.Span, other.m.Span);
    }

    override public int GetHashCode()
    {
        var code = new HashCode();
        foreach (var e in m.Span)
        {
            code.Add(e);
        }
        return code.ToHashCode();
    }
}

public class day12
{
    public static (string springs, ImmutableArray<int> groups) Parse(string s)
    {
        if (s.Split() is not [string springs, string sgroups])
            throw new NotImplementedException();

        var groups =
            sgroups
            .Split(',')
            .Select(s => int.Parse(s))
            .ToImmutableArray();
        return (springs, groups);
    }
    public static (string springs, ImmutableArray<int> groups) Unfold(int unfolding, (string springs, ImmutableArray<int> groups) sg)
    {
        return (
            string.Join('?', Enumerable.Repeat(sg.springs, unfolding)),
            Enumerable.Repeat(sg.groups, unfolding)
                .SelectMany(x => x)
                .ToImmutableArray());
    }
    public static long ArrangementsTable((
        string springs,
        ImmutableArray<int> groups) sg)
    {
        var arr = new long[sg.springs.Length, sg.groups.Length];
        int damaged = 0;
        for (int s = 0; s < sg.springs.Length; s++)
        {
            if (sg.springs[s] == '.')
                damaged = 0;
            else
                damaged++;

            for (int g = 0; g < sg.groups.Length; g++)
            {
                long prev;
                if (s == 0)
                    prev = 0;
                else if (sg.springs[s] == '#' && g + 1 == sg.groups.Length)
                    prev = 0;
                else
                    prev = arr[s - 1, g];

                long curr;
                if (damaged < sg.groups[g])
                    curr = 0;
                else if (sg.groups[g] <= s && sg.springs[s - sg.groups[g]] == '#')
                    curr = 0;
                else if (s + 1 < sg.springs.Length && sg.springs[s + 1] == '#')
                    curr = 0;
                else if (g == 0)
                    curr = 1;
                else if (1 + sg.groups[g] <= s)
                    curr = arr[s - sg.groups[g] - 1, g - 1];
                else
                    curr = 0;

                arr[s, g] = prev + curr;
            }
        }
        return arr[sg.springs.Length - 1, sg.groups.Length - 1];
    }

    public static long ArrangementsRec((
        string springs,
        ImmutableArray<int> groups) sg) => ArrangementsRecImpl(sg.springs.AsMemory(), sg.groups.AsMemory(), new());
    static long ArrangementsRecImpl(
        ReadOnlyMemory<char> springs,
        ReadOnlyMemory<int> groups,
        Dictionary<(EquatableMemory<char> springs, EquatableMemory<int> groups), long> cache)
    {
        if (groups.Length == 0)
        {
            if (MemoryExtensions.Contains(springs.Span, '#'))
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        // cache can also use ReadOnlyMemory, with lower hit rate.
        if (cache.TryGetValue((springs, groups), out long sum))
        {
            return sum;
        }

        int mid = groups.Length / 2;
        for (int i = 0; i + groups.Span[mid] <= springs.Length; ++i)
        {
            if (MemoryExtensions.Contains(springs[i..(i + groups.Span[mid])].Span, '.'))
                continue;

            int prev;
            if (i == 0)
            {
                prev = 0;
            }
            else
            {
                prev = i - 1;
                if (springs.Span[prev] == '#')
                    continue;
            }

            int next;
            if (i + groups.Span[mid] == springs.Length)
            {
                next = springs.Length;
            }
            else
            {
                next = i + groups.Span[mid] + 1;
                if (springs.Span[i + groups.Span[mid]] == '#')
                    continue;
            }

            var lhs = ArrangementsRecImpl(springs[..prev], groups[..mid], cache);
            var rhs = ArrangementsRecImpl(springs[next..], groups[(mid + 1)..], cache);

            sum += lhs * rhs;
        }

        cache.Add((springs, groups), sum);
        return sum;
    }

    static long Arrangements((string springs, ImmutableArray<int> groups) sg) => ArrangementsTable(sg);

    [Fact] public void Test_Arrangements_Line1() => Assert.Equal(1, Arrangements(Parse("???.### 1,1,3")));
    [Fact] public void Test_Arrangements_Line2() => Assert.Equal(4, Arrangements(Parse(".??..??...?##. 1,1,3")));
    [Fact] public void Test_Arrangements_Line3() => Assert.Equal(1, Arrangements(Parse("?#?#?#?#?#?#?#? 1,3,1,6")));
    [Fact] public void Test_Arrangements_Line4() => Assert.Equal(1, Arrangements(Parse("????.#...#... 4,1,1")));
    [Fact] public void Test_Arrangements_Line5() => Assert.Equal(4, Arrangements(Parse("????.######..#####. 1,6,5")));
    [Fact] public void Test_Arrangements_Line6() => Assert.Equal(10, Arrangements(Parse("?###???????? 3,2,1")));
    [Fact] public void Test_part1_example() => Assert.Equal(21, File.ReadLines("example.txt").Sum(x => Arrangements(Parse(x))));
    [Fact] public void Test_part1_input() => Assert.Equal(7084, File.ReadLines("input.txt").Sum(x => Arrangements(Parse(x))));
    [Fact] public void Test_part2_example() => Assert.Equal(525152, File.ReadLines("example.txt").Sum(x => Arrangements(Unfold(5, Parse(x)))));
    [Fact] public void Test_part2_input() => Assert.Equal(8414003326821, File.ReadLines("input.txt").Sum(x => Arrangements(Unfold(5, Parse(x)))));
}
