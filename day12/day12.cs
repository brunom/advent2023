using FsCheck;
using FsCheck.Xunit;
using Microsoft.FSharp.Collections;
using System.Collections.Immutable;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Xunit;

Console.WriteLine(

    //new[] { ".???#??.?##?#??? 1,1,7" }
    //new[] { ".???#?? 1,1" }
    //new[] { "?.#.? 1,1" }

    //new[] { "#.? 1" }
    //new[] { "#.??#? 1,2" }
    new[] { "## 1" }

    //File.ReadLines("input.txt")
    .Select(line => (line, sg: day12.Parse(line)))

    //new[] { ("#", Enumerable.Empty<int>().ToImmutableArray()), }

    .Select(x => (x.line, rec: day12.ArrangementsRec(x.sg), tab: day12.ArrangementsTable(x.sg)))
    //.Where(x => x.rec != x.tab)
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
    public readonly record struct Puzzle(string springs, ImmutableArray<int> damaged_groups);

    public static Puzzle Parse(string s)
    {
        if (s.Split() is not [string springs, string sgroups])
            throw new NotImplementedException();

        var groups =
            sgroups
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => int.Parse(s))
            .ToImmutableArray();
        return new(springs, groups);
    }
    public static Puzzle Unfold(int unfolding, Puzzle sg)
    {
        return new(
            string.Join('?', Enumerable.Repeat(sg.springs, unfolding)),
            Enumerable.Repeat(sg.damaged_groups, unfolding)
                .SelectMany(x => x)
                .ToImmutableArray());
    }
    public static long ArrangementsTable(Puzzle sg)
    {
        if (sg.springs.Length == 0 && sg.damaged_groups.Length == 0)
            return 1;
        if (sg.springs.Length != 0 && sg.damaged_groups.Length == 0)
            return MemoryExtensions.Contains(sg.springs, '#') ? 0 : 1;
        if (sg.springs.Length == 0 && sg.damaged_groups.Length != 0)
            return 0;

        var arr = new long[sg.springs.Length, sg.damaged_groups.Length];
        int max_damaged = 0;
        int? first_damaged = null;
        for (int s = 0; s < sg.springs.Length; s++)
        {
            if (sg.springs[s] == '.')
                max_damaged = 0;
            else
                max_damaged++;

            if (!first_damaged.HasValue && sg.springs[s] == '#')
                first_damaged = s;

            for (int g = 0; g < sg.damaged_groups.Length; g++)
            {
                long prev;
                if (s == 0)
                    prev = 0;
                else if (sg.springs[s] == '#')
                    prev = 0;
                else
                    prev = arr[s - 1, g];

                long curr;
                if (max_damaged < sg.damaged_groups[g])
                    curr = 0;
                else if (sg.damaged_groups[g] <= s && sg.springs[s - sg.damaged_groups[g]] == '#')
                    curr = 0;
                else if (g == 0)
                {
                    if (first_damaged + sg.damaged_groups[g] < s)
                        curr = 0;
                    else
                        curr = 1;
                }
                else if (sg.damaged_groups[g] + 1 <= s)
                    curr = arr[s - sg.damaged_groups[g] - 1, g - 1];
                else
                    curr = 0;

                arr[s, g] = prev + curr;
            }
        }
        return arr[sg.springs.Length - 1, sg.damaged_groups.Length - 1];
    }

    public static long ArrangementsRec(Puzzle sg) => ArrangementsRecImpl(sg.springs.AsMemory(), sg.damaged_groups.AsMemory(), new());
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

    static long Arrangements(Puzzle sg) => ArrangementsTable(sg);


    enum P
    {
        Damaged = '#',
        Any = '?',
        Operational = '.',
    };

    [Property]
    public Property TableMatchesRec()
    {
        return Prop.ForAll<P[], PositiveInt[]>((s0, g0) =>
        {
            var s = new string(s0.Select(x => (char)x).ToArray());
            var g = g0.Select(x => x.Get).ToImmutableArray();
            return ArrangementsRec(new(s, g)) == ArrangementsTable(new(s, g));
        });

    }
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
