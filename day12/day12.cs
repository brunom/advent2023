using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Xunit;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
    public static long Arrangements(string s) => Arrangements(s, 1);
    public static long Arrangements5(string s) => Arrangements(s, 5);
    static long Arrangements(string s, int unfolding)
    {
        if (s.Split() is not [string springs, string sgroups])
            throw new NotImplementedException();

        var groups =
            sgroups
            .Split(',')
            .Select(s => int.Parse(s))
            .ToImmutableArray();

        springs = string.Join('?', Enumerable.Repeat(springs, unfolding));
        groups =
            Enumerable.Repeat(groups, unfolding)
            .SelectMany(x => x)
            .ToImmutableArray();
        Dictionary<(EquatableMemory<char> springs, EquatableMemory<int> groups), long> cache = new();
        var result = Arrangements(springs.AsMemory(), groups.AsMemory(), cache);
        return result;
    }

    static bool Any(ReadOnlySpan<char> s, char ch)
    {
        foreach (var ch2 in s)
        {
            if (ch2 == ch)
                return true;
        }
        return false;
    }
    static long Arrangements(
        ReadOnlyMemory<char> springs,
        ReadOnlyMemory<int> groups,
        Dictionary<(EquatableMemory<char> springs, EquatableMemory<int> groups), long> cache)
    {
        if (groups.Length == 0)
        {
            if (Any(springs.Span, '#'))
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        if (cache.TryGetValue((springs, groups), out long sum))
        {
            return sum;
        }

        int mid = groups.Length / 2;
        for (int i = 0; i + groups.Span[mid] <= springs.Length; ++i)
        {
            if (Any(springs[i..(i + groups.Span[mid])].Span, '.'))
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

            var lhs = Arrangements(springs[..prev], groups[..mid], cache);
            var rhs = Arrangements(springs[next..], groups[(mid + 1)..], cache);

            sum += lhs * rhs;
        }

        cache.Add((springs, groups), sum);
        return sum;
    }

    [Fact] public void Test_Arrangements_Line1() => Assert.Equal(1, Arrangements("???.### 1,1,3"));
    [Fact] public void Test_Arrangements_Line2() => Assert.Equal(4, Arrangements(".??..??...?##. 1,1,3"));
    [Fact] public void Test_Arrangements_Line3() => Assert.Equal(1, Arrangements("?#?#?#?#?#?#?#? 1,3,1,6"));
    [Fact] public void Test_Arrangements_Line4() => Assert.Equal(1, Arrangements("????.#...#... 4,1,1"));
    [Fact] public void Test_Arrangements_Line5() => Assert.Equal(4, Arrangements("????.######..#####. 1,6,5"));
    [Fact] public void Test_Arrangements_Line6() => Assert.Equal(10, Arrangements("?###???????? 3,2,1"));
    [Fact] public void Test_part1_example() => Assert.Equal(21, File.ReadLines("example.txt").Sum(Arrangements));
    [Fact] public void Test_part1_input() => Assert.Equal(7084, File.ReadLines("input.txt").Sum(Arrangements));
    [Fact] public void Test_part2_example() => Assert.Equal(525152, File.ReadLines("example.txt").Sum(Arrangements5));
    [Fact] public void Test_part2_input() => Assert.Equal(8414003326821, File.ReadLines("input.txt").Sum(Arrangements5));
}
