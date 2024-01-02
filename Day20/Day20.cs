using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
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
    Module[] modules;
    Module broadcaster;
    public Configuration(string filename)
    {
        var lines =
            File.ReadLines(filename)
            .Select(x => x.Split(new[] { ",", "->", " " }, StringSplitOptions.RemoveEmptyEntries))
            .ToList();
        Dictionary<string, Module> byName = new();
        modules = new Module[lines.Count];
        for (int i = 0; i < lines.Count; i++)
        {
            string type_name = lines[i][0];
            string name;
            if (type_name == "broadcaster")
            {
                name = type_name;
            }
            else
            {
                name = type_name[1..];
            }
            var module = new Module { Name = name, Type = type_name[0] == '%' ? Type.FlipFlop : Type.Conjunction };
            byName.Add(name, module);
            modules[i] = module;
        }
        broadcaster = byName["broadcaster"];

        for (int iSrcModule = 0; iSrcModule < lines.Count; iSrcModule++)
        {
            Module srcModule = modules[iSrcModule];
            foreach (var dstName in lines[iSrcModule].AsSpan()[1..])
            {
                Module dstModule = byName[dstName];
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
    IEnumerable<Message> PushButton()
    {
        foreach (var cable in broadcaster.Outputs)
        {
            yield return new Message
            {
                Pulse = Pulse.low,
                Cable = cable,
            };
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
        else throw new NotImplementedException();
    }
    public IEnumerable<Message> Trace()
    {
        Queue<Message> q = new();
        foreach (var m in PushButton())
            q.Enqueue(m);
        while (q.Count > 0)
        {
            var oldMessage = q.Dequeue();
            yield return oldMessage;
            foreach (var newMessage in Receive(oldMessage))
                q.Enqueue(newMessage);
        }
    }
}

public class Day20
{
    [Fact]
    public static void Test_trace_example1()
    {
        var c = new Configuration("example1.txt");
        var t = c.Trace();
        Assert.Equal("""
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
    public static void Test_trace_example2()
    {
        var c = new Configuration("example2.txt");
        var t = c.Trace();
        Assert.Equal("""
            broadcaster -low-> a
            a -high-> inv
            a -high-> con
            inv -low-> b
            con -high-> output
            b -high-> con
            con -low-> output
            """.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None),
            t.Select(x => x.ToString()));
    }
}
