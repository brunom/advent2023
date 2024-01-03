using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Xunit;
using static System.Runtime.InteropServices.JavaScript.JSType;

public enum Pulse : byte
{
    low, // Low = default, since Conjunctions "initially default to remembering a low pulse for each input"
    high,
}
public enum Type
{
    FlipFlop,
    Conjunction,
    Broadcast,
    Button,
}
public class Module
{
    public Pulse LastPulse;
    public Type Type;
    public string Name; // does not include starting % or &
    public List<Cable> Inputs = new();
    public List<Cable> Outputs = new();
}

public class Cable
{
    public Pulse LastPulse;
    public Module Input;
    public Module Output;
}
public struct Message
{
    public Pulse Pulse;
    public Cable Cable;
    public override string ToString()
    {
        return $"{Cable.Input.Name} -{Pulse}-> {Cable.Output.Name}";
    }
}
public class Configuration
{
    Module Button { get; }
    public Configuration(string filename)
    {
        var lines = File.ReadLines(filename);

        var byName = new Dictionary<string, Module>();
        foreach (var line in lines)
        {
            var names = line.Split(new[] { ", ", " -> " }, StringSplitOptions.None);
            string srcName = names[0];
            Type type;
            if (srcName[0] == '%')
            {
                type = Type.FlipFlop;
                srcName = srcName[1..];
            }
            else if (srcName[0] == '&')
            {
                type = Type.Conjunction;
                srcName = srcName[1..];
            }
            else if (srcName == "broadcaster")
            {
                type = Type.Broadcast;
            }
            else throw new NotImplementedException();

            if (!byName.TryGetValue(srcName, out Module srcModule))
            {
                srcModule = new Module { Name = srcName };
                byName.Add(srcName, srcModule);
            }
            srcModule.Type = type;

            if (type == Type.Broadcast)
            {
                Button = new Module { Name = "button", Type = Type.Button, };
                var cable = new Cable
                {
                    Input = Button,
                    Output = srcModule
                };
                Button.Outputs.Add(cable);
                srcModule.Inputs.Add(cable);
            }
            foreach (var dstName in names.AsSpan()[1..])
            {
                if (!byName.TryGetValue(dstName, out Module dstModule))
                {
                    dstModule = new Module { Name = dstName };
                    byName.Add(dstName, dstModule);
                }
                var cable = new Cable
                {
                    Input = srcModule,
                    Output = dstModule
                };
                srcModule.Outputs.Add(cable);
                dstModule.Inputs.Add(cable);
            }
        }
    }
    public IEnumerable<Message> Receive(Message message)
    {
        Module module = message.Cable.Output;
        if (module.Type == Type.FlipFlop)
        {
            if (message.Pulse == Pulse.low)
            {
                Pulse newPulse = 1 - module.LastPulse;
                module.LastPulse = newPulse;
                foreach (var cable in module.Outputs)
                {
                    yield return new Message
                    {
                        Pulse = newPulse,
                        Cable = cable,
                    };
                }
            }
        }
        else if (module.Type == Type.Conjunction)
        {
            message.Cable.LastPulse = message.Pulse;
            Pulse newPulse = module.Inputs.All(input1 => input1.LastPulse == Pulse.high) ? Pulse.low : Pulse.high;
            foreach (var cable in module.Outputs)
            {
                yield return new Message
                {
                    Pulse = newPulse,
                    Cable = cable,
                };
            }
        }
        else if (module.Type == Type.Broadcast)
        {
            foreach (var cable in module.Outputs)
            {
                yield return new Message
                {
                    Pulse = message.Pulse,
                    Cable = cable,
                };
            }
        }
        else throw new NotImplementedException();
    }
    public IEnumerable<Message> Press(int times = 1)
    {
        Queue<Message> q = new();
        for (int i = 0; i < times; i++)
        {
            q.Enqueue(new Message { Pulse = Pulse.low, Cable = Button.Outputs.Single(), });
            while (q.Count > 0)
            {
                var oldMessage = q.Dequeue();
                yield return oldMessage;
                foreach (var newMessage in Receive(oldMessage))
                    q.Enqueue(newMessage);
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
        var t = c.Press();
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
        var t = c.Press(1000).ToList();
        Assert.Equal(8000, t.Select(x => x.Pulse).Where(x => x == Pulse.low).LongCount());
        Assert.Equal(4000, t.Select(x => x.Pulse).Where(x => x == Pulse.high).LongCount());
    }
    [Fact]
    public static void Test_trace_example2()
    {
        var c = new Configuration("example2.txt");
        var t = c.Press(4);
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
        var t = c.Press(1000).ToList();
        Assert.Equal(4250, t.Select(x => x.Pulse).Where(x => x == Pulse.low).LongCount());
        Assert.Equal(2750, t.Select(x => x.Pulse).Where(x => x == Pulse.high).LongCount());
    }
    [Fact]
    public static void Test_count_input()
    {
        var c = new Configuration("input.txt");
        var t = c.Press(1000).ToList();
        Assert.Equal(739960225,
            t.Select(x => x.Pulse).Where(x => x == Pulse.low).LongCount() *
            t.Select(x => x.Pulse).Where(x => x == Pulse.high).LongCount());
    }
}
