using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection.Emit;
using Xunit;

public class UnitTest1
{
    const string example = "rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7";
    private static IEnumerable<string> Steps(string sequence)
    {
        return sequence
            .Split(',')
            .Select(step => new string(step.Where(ch => !char.IsWhiteSpace(ch)).ToArray()));
    }

    static long Hash(string str)
    {
        long result = 0;
        foreach (var ch in str)
        {
            result += ch;
            result *= 17;
        }
        return result & 0xff;
    }
    static long Part1(string sequence)
    {
        return
            Steps(sequence)
            .Select(Hash)
            .Sum();
    }
    static long Part2(string sequence)
    {
        var boxes =
            Enumerable.Range(0, 256)
            .Select(_ => new OrderedDictionary())
            .ToArray();

        foreach (var step in Steps(sequence))
        {
            if (step.EndsWith("-"))
            {
                string label = step.Substring(0, step.Length - "-".Length);
                long hash = Hash(label);
                boxes[hash].Remove(label);
            }
            else if (step.Split('=') is [string label, string value])
            {
                long hash = Hash(label);
                boxes[hash][label] = int.Parse(value);
            }
            else throw new NotImplementedException();
        }
        return
            boxes
            .Select((b, i) => (i + 1) * b.Cast<DictionaryEntry>().Select((de, y) => (y + 1) * (int)de.Value!).Sum())
            .Sum();
    }

    [Fact] public void part1_example() => Assert.Equal(1320, Part1(example));
    [Fact] public void part1_input() => Assert.Equal(512797, Part1(File.ReadAllText("input.txt")));
    [Fact] public void part2_example() => Assert.Equal(145, Part2(example));
    [Fact] public void part2_input() => Assert.Equal(262454, Part2(File.ReadAllText("input.txt")));
}
