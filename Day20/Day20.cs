using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;
using Xunit;
using static System.Runtime.InteropServices.JavaScript.JSType;


Day20.Test_rx_input();

public enum PulseValue : byte
{
    low, // Low = default, since Conjunctions "initially default to remembering a low pulse for each input" and FlipFlops "are initially off"
    high,
}

public enum ModuleType
{
    Broadcast, // Broadcast == default, do nothing when no destinations
    FlipFlop,
    Conjunction,
    Button,
}
public class Module
{
    public int FlipFlopIndex = -1;
    public ModuleType Type;
    required public string Name { get; init; } // does not include starting % or &
    public List<Cable> Inputs = new();
    public List<Cable> Destinations = new();
}

public class Cable
{
    public PulseValue LastValue;
    required public Module Input { get; init; }
    required public Module Destination { get; init; }
}
public struct Pulse
{
    public PulseValue Value;
    public Cable Cable;
    public override string ToString()
    {
        return $"{Cable.Input.Name} -{Value}-> {Cable.Destination.Name}";
    }
}
public class Configuration
{
    public Dictionary<string, Module> Modules = new();
    public UInt64 flipFlops;
    public Queue<Pulse> InFlight = new();

    public Configuration(string filename)
    {
        int nFlipFlops = 0;
        foreach (var line in File.ReadLines(filename))
        {
            var names = line.Split(new[] { ", ", " -> " }, StringSplitOptions.None);
            string srcName = names[0];
            ModuleType type;
            int flipFlopIndex = -1;
            if (srcName[0] == '%')
            {
                flipFlopIndex = nFlipFlops++;
                type = ModuleType.FlipFlop;
                srcName = srcName[1..];
            }
            else if (srcName[0] == '&')
            {
                type = ModuleType.Conjunction;
                srcName = srcName[1..];
            }
            else if (srcName == "broadcaster")
            {
                type = ModuleType.Broadcast;
            }
            else throw new NotImplementedException();

            if (!Modules.TryGetValue(srcName, out Module? srcModule))
            {
                srcModule = new Module { Name = srcName, };
                Modules.Add(srcName, srcModule);
            }
            srcModule.Type = type;
            srcModule.FlipFlopIndex = flipFlopIndex;

            if (type == ModuleType.Broadcast)
            {
                var button = new Module { Name = "button", Type = ModuleType.Button, };
                var cable = new Cable
                {
                    Input = button,
                    Destination = srcModule
                };
                button.Destinations.Add(cable);
                srcModule.Inputs.Add(cable);
                Modules.Add("button", button);
            }
            foreach (var dstName in names.AsSpan()[1..])
            {
                if (!Modules.TryGetValue(dstName, out Module? dstModule))
                {
                    dstModule = new Module { Name = dstName, };
                    Modules.Add(dstName, dstModule);
                }
                var cable = new Cable
                {
                    Input = srcModule,
                    Destination = dstModule
                };
                srcModule.Destinations.Add(cable);
                dstModule.Inputs.Add(cable);
            }
        }
    }
    public void Propagate(Pulse pulse)
    {
        Module module = pulse.Cable.Destination;
        if (module.Type == ModuleType.FlipFlop)
        {
            if (pulse.Value == PulseValue.low)
            {
                //(module.State, var newPulse)
                //    = module.State == FlipFlopState.off
                //    ? (FlipFlopState.on, PulseValue.high)
                //    : (FlipFlopState.off, PulseValue.low);
                Debug.Assert(0 <= module.FlipFlopIndex);
                var newValue = 1 - (PulseValue)((flipFlops >> module.FlipFlopIndex) & 1);
                flipFlops ^= (UInt64)1 << module.FlipFlopIndex;
                foreach (var cable in module.Destinations)
                {
                    InFlight.Enqueue(new Pulse
                    {
                        Value = newValue,
                        Cable = cable,
                    });
                }
            }
        }
        else if (module.Type == ModuleType.Conjunction)
        {
            pulse.Cable.LastValue = pulse.Value;

            PulseValue newValue = PulseValue.low;
            foreach (var cable in module.Inputs)
            {
                if (cable.LastValue != PulseValue.high)
                    newValue = PulseValue.high;
            }
            foreach (var cable in module.Destinations)
            {
                InFlight.Enqueue(new Pulse
                {
                    Value = newValue,
                    Cable = cable,
                });
            }
        }
        else if (module.Type == ModuleType.Broadcast)
        {
            foreach (var cable in module.Destinations)
            {
                InFlight.Enqueue(new Pulse
                {
                    Value = pulse.Value,
                    Cable = cable,
                });
            }
        }
        else throw new NotImplementedException();
    }
    public IEnumerable<Pulse> PressButton(int times = 1)
    {
        var button = Modules["button"];
        for (int i = 0; i < times; i++)
        {
            InFlight.Enqueue(new Pulse { Value = PulseValue.low, Cable = button.Destinations.Single(), });
            while (InFlight.Count > 0)
            {
                var pulse = InFlight.Dequeue();
                yield return pulse;
                Propagate(pulse);
            }
        }
    }
}

public class Day20
{
    [Fact]
    public static void Test_trace_example1()
    {
        var c = new Configuration("example1.txt");
        var t = c.PressButton();
        Assert.Equal("""
            button -low-> broadcaster
            broadcaster -low-> a
            broadcaster -low-> b
            broadcaster -low-> c
            a -high-> b
            b -high-> c
            c -high-> inv
            inv -low-> a
            a -low-> b
            b -low-> c
            c -low-> inv
            inv -high-> a
            """.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None),
            t.Select(x => x.ToString()));
    }
    [Fact]
    public static void Test_count_example1()
    {
        var c = new Configuration("example1.txt");
        var t = c.PressButton(1000).ToList();
        Assert.Equal(8000, t.Where(x => x.Value == PulseValue.low).LongCount());
        Assert.Equal(4000, t.Where(x => x.Value == PulseValue.high).LongCount());
    }
    [Fact]
    public static void Test_trace_example2()
    {
        var c = new Configuration("example2.txt");
        var t = c.PressButton(4);
        Assert.Equal("""
            button -low-> broadcaster
            broadcaster -low-> a
            a -high-> inv
            a -high-> con
            inv -low-> b
            con -high-> output
            b -high-> con
            con -low-> output
            button -low-> broadcaster
            broadcaster -low-> a
            a -low-> inv
            a -low-> con
            inv -high-> b
            con -high-> output
            button -low-> broadcaster
            broadcaster -low-> a
            a -high-> inv
            a -high-> con
            inv -low-> b
            con -low-> output
            b -low-> con
            con -high-> output
            button -low-> broadcaster
            broadcaster -low-> a
            a -low-> inv
            a -low-> con
            inv -high-> b
            con -high-> output
            """.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None),
            t.Select(x => x.ToString()));
    }
    [Fact]
    public static void Test_count_example2()
    {
        var c = new Configuration("example2.txt");
        var t = c.PressButton(1000).ToList();
        Assert.Equal(4250, t.Where(x => x.Value == PulseValue.low).LongCount());
        Assert.Equal(2750, t.Where(x => x.Value == PulseValue.high).LongCount());
    }
    [Fact]
    public static void Test_count_input()
    {
        var c = new Configuration("input.txt");
        long low = 0;
        long high = 0;
        foreach (var x in c.PressButton(1000))
        {
            if (x.Value == PulseValue.low)
                low++;
            else
                high++;
        }
        Assert.Equal(739960225, low * high);
    }
    [Fact]
    public static void Test_rx_input()
    {
        var c = new Configuration("input.txt");
        const int npartitions = 4;
        string[][] partition_names =
        [
            ["kg", "pt", "vv", "nc", "gb", "ls", "lf", "hr", "fq", "qn", "bh", "vq"],
            ["dz", "dc", "fk", "sl", "rp", "jb", "kp", "pz", "zg", "bb", "hg", "dl"],
            ["ff", "vl", "vx", "cv", "jp", "kt", "hm", "tz", "mf", "sx", "rj", "xt"],
            ["bq", "mg", "dp", "mh", "rz", "tj", "nd", "jx", "zz", "pf", "xk", "sf"],
        ];
        Module[] partition_sinks = new[] { "kf", "kr", "zs", "qk" }.Select(x => c.Modules[x]).ToArray();
        UInt64[] partition_masks =
            partition_names
            .Select(ns => ns.Select(n => 1UL << c.Modules[n].FlipFlopIndex).Aggregate((a, b) => a | b))
            .ToArray();
        Dictionary<UInt64, long>[] partitions_dicts
            = [new(), new(), new(), new(),];
        List<UInt64>[] partitions_lists
            = [new(), new(), new(), new(),];
        for (int ipartition = 0; ipartition < npartitions; ipartition++)
        {
            c.flipFlops = 0;
            for (long presses = 0; ; )
            {
                Debug.Assert(c.InFlight.Count == 0);
                if (!partitions_dicts[ipartition].TryAdd(c.flipFlops & partition_masks[ipartition], presses))
                {
                    Console.WriteLine(new { ipartition, maxiter = presses, ff = c.flipFlops & partition_masks[ipartition] });
                    break;
                }
                partitions_lists[ipartition].Add(c.flipFlops & partition_masks[ipartition]);

                presses += 1;

                foreach (var pulse in c.PressButton())
                {
                    if (pulse.Cable.Input == partition_sinks[ipartition] && pulse.Value == PulseValue.high)
                    {
                        Console.WriteLine(new { ipartition, presses, });
                    }
                }
            }
        }

        "".ToString();
    }
}
