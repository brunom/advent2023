using Xunit;

SortedDictionary<string, (int size, SortedSet<(string name, int nwires)> connections)> graph = new();
void Dump()
{
    foreach (var kv in graph)
    {
        string src = kv.Key;
        Console.Write((src, kv.Value.size));
        Console.Write(':');
        foreach (var dst in kv.Value.connections)
        {
            Console.Write(' ');
            Console.Write(dst);
        }
        Console.WriteLine();
    }
    Console.WriteLine();
}
void Add(string lhs, string rhs)
{
    if (!graph.TryGetValue(lhs, out var node))
    {
        node = new();
        graph.Add(lhs, (1, [(rhs, 1)]));
    }
    else
    {
        node.connections.Add((rhs, 1));
    }
}
void Load()
{
    foreach (var line in File.ReadLines("example.txt"))
    {
        var s = line.Split(new[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);
        string lhs = s[0];
        foreach (var rhs in s.Skip(1))
        {
            Add(lhs, rhs);
            Add(rhs, lhs);
        }
    }
}
void Solve()
{
    foreach (var kv in graph)
    {
        string src = kv.Key;
        foreach (var (dst, _) in kv.Value.connections)
        {
            if (src.CompareTo(dst) > 0)
                continue;
            HashSet<(string, string)> flowing = new();
            bool find(string curr, HashSet<string> visited)
            {
                if (curr == dst)
                    return true;
                foreach (var (next, _) in graph[curr].connections)
                {
                    if (flowing.Contains((curr, next)))
                        continue;
                    if (!visited.Add(next))
                        continue;

                    if (find(next, visited))
                    {
                        if (!flowing.Remove((next, curr)))
                        {
                            flowing.Add((curr, next));
                        }
                        //Console.Write(next);
                        //Console.Write(' ');
                        return true;
                    }

                    //visited.Remove(next);
                }
                return false;

            }
            flowing.Add((src, dst));
            flowing.Add((dst, src));
            int found = 1;
            while (find(src, [src]))
            {
                //Console.WriteLine();
                ++found;
            }
            Console.WriteLine((src, dst, found));
        }

    }
}

Load();
Dump();
Solve();

public class Day25
{
    [Fact]
    public void Test1()
    {

    }
}
