using System.Collections.Immutable;
using Xunit;

public class Day24
{
    public readonly record struct Vec(double x, double y, double z);
    public readonly record struct Hail(Vec pos, Vec vel);

    public static Hail ParseLine(string line)
    {
        if (line.Split('@') is not [string pos, string vel])
            throw new ArgumentException();
        if (pos.Split(',') is not [string spx, string spy, string spz])
            throw new ArgumentException();
        if (vel.Split(',') is not [string svx, string svy, string svz])
            throw new ArgumentException();
        return new(
            new(double.Parse(spx), double.Parse(spy), double.Parse(spz)),
            new(double.Parse(svx), double.Parse(svy), double.Parse(svz))
            );
    }
    public static List<Hail> ParseFile(string filename)
    => File.ReadLines(filename).Select(ParseLine).ToList();

    static (double ta, Vec pa) Intersect(Hail ha, Hail hb)
    {
        double ta
            = (-hb.vel.y * ha.pos.x + hb.vel.y * hb.pos.x + hb.vel.x * ha.pos.y - hb.vel.x * hb.pos.y)
            / (-hb.vel.x * ha.vel.y + ha.vel.x * hb.vel.y);
        Vec pa = new(
            ha.pos.x + ha.vel.x * ta,
            ha.pos.y + ha.vel.y * ta,
            ha.pos.z + ha.vel.z * ta);
        return (ta, pa);
    }
    public static long Intersections(IReadOnlyList<Hail> hails, Vec testmin, Vec testmax)
    {
        long result = 0;
        for (int ia = 0; ia < hails.Count; ia++)
        {
            for (int ib = ia + 1; ib < hails.Count; ib++)
            {
                var ha = hails[ia];
                var hb = hails[ib];

                var (ta, pa) = Intersect(ha, hb);
                var (tb, pb) = Intersect(hb, ha);

                if (ta < 0)
                    continue;
                if (tb < 0)
                    continue;
                if (pa.x < testmin.x)
                    continue;
                if (pa.y < testmin.y)
                    continue;
                if (testmax.x < pa.x)
                    continue;
                if (testmax.y < pa.y)
                    continue;
                result++;
            }
        }
        return result;
    }

    [Fact] public void Test_part1_example() => Assert.Equal(2, Intersections(ParseFile("example.txt"), new(7, 7, -1), new(27, 27, -1)));
    [Fact] public void Test_part1_input() => Assert.Equal(17867, Intersections(ParseFile("input.txt"), new(200000000000000, 200000000000000, -1), new(400000000000000, 400000000000000, -1)));

}
