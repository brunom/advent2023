using System.Collections.Immutable;
using Xunit;

public class day12
{
    static long Arrangements(string s) => Arrangements(s, 1);
    static long Arrangements5(string s) => Arrangements(s, 5);
    static long Arrangements(string s, int unfolding)
    {
        if (s.Split() is not [string springs, string sgroups])
            throw new NotImplementedException();

        // TODO repeat after parse
        springs = string.Join('?', Enumerable.Repeat(springs, unfolding));
        sgroups = string.Join(',', Enumerable.Repeat(sgroups, unfolding));

        var groups =
            sgroups
            .Split(',')
            .Select(s => int.Parse(s))
            .ToImmutableArray();
        return Arrangements(springs, groups).Last();
    }

    enum Arrangeable
    {
        No = -1,
        Yes = -2,
    }
    static IEnumerable<long> Arrangements(string springs, ImmutableArray<int> groups)
    {
        if (!groups.Any())
        {
            if (springs.Any(ch => ch == '#'))
            {
                yield return (long)Arrangeable.No;
                yield return 0;
            }
            else
            {
                yield return (long)Arrangeable.Yes;
                yield return 1;
            }
        }

        long sum = 0;
        bool signaled_arrangeable = false;
        int mid = groups.Length / 2;
        for (int i = 0; i + groups[mid] <= springs.Length; ++i)
        {
            if (springs[i..(i + groups[mid])].Any(ch => ch == '.'))
                continue;

            int prev;
            if (i == 0)
            {
                prev = 0;
            }
            else
            {
                prev = i - 1;
                if (springs[prev] == '#')
                    continue;
            }

            int next;
            if (i + groups[mid] == springs.Length)
            {
                next = springs.Length;
            }
            else
            {
                next = i + groups[mid] + 1;
                if (springs[i + groups[mid]] == '#')
                    continue;
            }

            using var lhs = Arrangements(springs[..prev], groups[..mid]).GetEnumerator();
            if (!lhs.MoveNext()) throw new NotImplementedException();
            if (lhs.Current == (long)Arrangeable.No)
                continue;

            var rhs = Arrangements(springs[next..], groups[(mid + 1)..]).GetEnumerator();
            if (!rhs.MoveNext()) throw new NotImplementedException();
            if (rhs.Current == (long)Arrangeable.No)
                continue;

            if (!lhs.MoveNext()) throw new NotImplementedException();
            if (!rhs.MoveNext()) throw new NotImplementedException();

            if (!signaled_arrangeable)
            {
                signaled_arrangeable = true;
                yield return (long)Arrangeable.Yes;
            }
            sum += lhs.Current * rhs.Current;
        }

        if (!signaled_arrangeable)
        {
            yield return (long)Arrangeable.No;
            yield return 0;
        }
        else
        {
            yield return sum;
        }
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
