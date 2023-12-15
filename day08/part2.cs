var lines = File.ReadLines("input.txt");
string instructions = lines.First();

var nodes = lines
    .Skip(2)
    .Select(x => (N: x[0..3], L: x[7..10], R: x[12..15]))
    .SelectMany(x => new[] {
        //(Dir: Dir.DL, Key:x.N, Value:x.L),
        //(Dir: Dir.DR, Key:x.N, Value:x.R),
        (Dir: Dir.IL, Key:x.L, Value:x.N),
        (Dir: Dir.IR, Key:x.R, Value:x.N), })
    .GroupBy(x => x.Dir, (Dir, es) => (Dir, es.ToLookup(x => x.Key, x => x.Value)))
    .ToDictionary(x => x.Dir, x => x.Item2);

SortedDictionary<(long I, string SrcN), (long Steps, string DstN)> hyper = new();
{
    long steps = 0;
    var states = nodes[Dir.IL]
        .SelectMany(x => x)
        .Where(x => x.Last() == 'Z')
        .SelectMany(x => Enumerable.Range(0, instructions.Length).Select(i => (I: i, SrcN: x, DstN: x)))
        .ToList();

    while (true)
    {
        ++steps;
        states = states
            .Select(x => (I: (x.I > 0 ? x.I : instructions.Length) - 1, x.SrcN, x.DstN))
            .SelectMany(x => nodes[instructions[x.I] == 'L' ? Dir.IL : Dir.IR][x.SrcN].Select(y => (x.I, SrcN: y, x.DstN)))
            .Where(s => !hyper.ContainsKey((s.I, s.SrcN)))
            .ToList(); // Avoid rework.

        if (!states.Any())
            break;

        foreach (var s in states)
        {
            hyper.Add((s.I, s.SrcN), (steps, s.DstN));
        }
    }
}

{
    long steps = 0;
    var states = nodes[Dir.IL]
        .SelectMany(x => x)
        .Where(srcN => srcN.Last() == 'A')
        //.Where(srcN => srcN == "AAA")
        .Select(srcN => hyper[(0, srcN)])
        .ToArray();

    while (true)
    {
        long dsteps = states.Select(s => s.Steps).Min();
        if (states.Where(s => s.Steps == dsteps).Count() == 2)
        {
            static long mcd(long numero1, long numero2)
            {
                long a = Math.Max(numero1, numero2);
                long b = Math.Min(numero1, numero2);
                do //ciclo para las iteraciones
                {
                    long resultado = b;  // Guardamos el divisor en el resultado
                    b = a % b;      //Guardamos el resto en el divisor
                    a = resultado;  //El divisor para al dividendo
                } while (b != 0);
                return a;
            }
            static long lcm(long numero1, long numero2) => numero1 * numero2 / mcd(numero1, numero2);
            var fst = states.Where(s => s.Steps == dsteps).First();
            var snd = states.Where(s => s.Steps == dsteps).Last();
            long l = lcm(hyper[(0, fst.DstN)].Steps, hyper[(0, snd.DstN)].Steps);
            hyper.Add((0, fst.DstN + snd.DstN), (l, fst.DstN + snd.DstN));
            states = states.Where(s => s.Steps != dsteps).Concat(new[] { (dsteps, fst.DstN + snd.DstN) }).ToArray();
        }
        steps += dsteps;
        if (states.All(s => s.Steps == dsteps))
            break;
        for (int i = 0; i != states.Length; i++)
        {
            if (states[i].Steps == dsteps)
            {
                var aux = hyper[(steps % instructions.Length, states[i].DstN)];
                //Console.WriteLine($"{i} {steps % instructions.Length} {states[i].DstN} {aux.Steps} {aux.DstN}");
                states[i] = aux;
            }
            else
            {
                states[i].Steps -= dsteps;
            }
        }
    }
    Console.WriteLine(steps);
}
enum Dir
{
    //DL,
    //DR,
    IL,
    IR,
};

