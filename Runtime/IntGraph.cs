using System;
using System.Collections.Generic;

namespace Hasklee {

public class IntGraphV
{
    private Dictionary<int, List<Tuple<int, float>>> adjDict;

    public IntGraphV() : this(null) {}

    public IntGraphV(Dictionary<int, List<Tuple<int, float>>> adjDict)
    {
        if (adjDict == null)
            this.adjDict = new Dictionary<int, List<Tuple<int, float>>>();
        else
            this.adjDict = adjDict;
    }

    // public IntGraphV(IntGraph graph, float v) : this(null)
    // {
    //     foreach (KeyValuePair<int, List<int>> kvp in graph.AdjDict)
    //     {
    //         foreach (var to in kvp.Value)
    //         {
    //             SafeAddEdge(kvp.Key, to, v);
    //         }
    //     }
    // }

    public Dictionary<int, List<Tuple<int, float>>> AdjDict
    {
        get => adjDict;
        set => adjDict = value;
    }

    public void AddNode(int node)
    {
        adjDict.Add(node, new List<Tuple<int, float>>());
    }

    public void SafeAddNode(int node)
    {
        if (!adjDict.ContainsKey(node))
        {
            adjDict.Add(node, new List<Tuple<int, float>>());
        }
    }

    public void AddEdge(int from, int to, float v)
    {
        AddEdge(from, new Tuple<int, float>(to, v));
    }

    public void AddEdge(int from, Tuple<int, float> to)
    {
        adjDict[from].Add(to);
    }

    public void AddEdgeU(int from, int to, float f)
    {
        SafeAddEdge(from, to, f);
        SafeAddEdge(to, from, f);
    }

    public void SafeAddEdge(int from, int to, float v)
    {
        SafeAddNode(from);
        AddEdge(from, to, v);
    }

    public void SafeAddEdge(int from, Tuple<int, float> to)
    {
        SafeAddNode(from);
        AddEdge(from, to);
    }

    public List<Tuple<int, float>> PostSet(int node)
    {
        List<Tuple<int, float>> value;
        if (adjDict.TryGetValue(node, out value))
        {
            return value;
        }
        else
        {
            return null;
        }
    }

    public IntGraphV Transpose()
    {
        IntGraphV transposed = new IntGraphV();

        foreach (KeyValuePair<int, List<Tuple<int, float>>> kvp in adjDict)
        {
            foreach (var ii in kvp.Value)
            {
                transposed.SafeAddEdge(ii.Item1, kvp.Key, ii.Item2);
            }
        }

        return transposed;
    }

    public IntGraphV TransposeN()
    {
        IntGraphV transposed = new IntGraphV();

        foreach (KeyValuePair<int, List<Tuple<int, float>>> kvp in adjDict)
        {
            foreach (var ii in kvp.Value)
            {
                transposed.SafeAddEdge(ii.Item1, kvp.Key, -ii.Item2);
            }
        }

        return transposed;
    }
}

}
