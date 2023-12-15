using Xunit;

public class UnitTest1
{
    const string example = "rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7";

    static long HashStep(IEnumerable<char> step)
    {
        long result = 0;
        foreach (var ch in step)
        {
            result += ch;
            result *= 17;
        }
        return result & 0xff;
    }
    static long HashSequence(string sequence)
    {
        return
            sequence
            .Split(',')
            .Select(step => step.Where(ch => !char.IsWhiteSpace(ch)))
            .Select(HashStep)
            .Sum();
    }
    [Fact] public void part1_example() => Assert.Equal(1320, HashSequence(example));
    [Fact] public void part1_input() => Assert.Equal(512797, HashSequence(File.ReadAllText("input.txt")));
}
