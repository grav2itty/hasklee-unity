using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hasklee {

public class GraphPropagate3 : GraphPropagate
{
    //second int tells the distance from the starting node
    private SortedSet<Tuple<int, int>> targets;
    private SortedSet<int> starts;
    private SortedSet<int> nextStarts;
    private Dictionary<int, float> dds;
    private SortedSet<int> visited;

    private int iterations = 1;
    private float iterationM = 1f;
    private float dissipate = 0.9f;

    private Dictionary<int, IPerformer<float>> iperfCache;
    private Dictionary<int, bool> stopCache;
    private Dictionary<int, ICValue<float>> cvalCache;

    public override void Init(IdGraphR g, int start)
    {
        graph = g.graph;

        targets = new SortedSet<Tuple<int, int>>(new NodeLCompare());
        starts = new SortedSet<int>();
        nextStarts = new SortedSet<int>();
        dds = new Dictionary<int, float>();
        visited = new SortedSet<int>();

        iperfCache = new Dictionary<int, IPerformer<float>>();
        stopCache = new Dictionary<int, bool>();
        cvalCache = new Dictionary<int, ICValue<float>>();
    }


    public override void Propagate(Vector3 v) => PropagateFrom(v, start);

    public override void PropagateFrom(Vector3 v, int start)
    {
        float result;
        float dd = (v.x + v.y) * sensitivity;
        starts.Clear();
        starts.Add(start);
        dds[start] = dd;
        visited.Clear();

        float itM = iterationM;

        for (int i=0; i<iterations; i++)
        {
            foreach(var s in starts)
            {
                targets.Clear();
                visited.Clear();
                targets.Add(new Tuple<int, int>(s, 0));
                while (targets.Count > 0 )
                {
                    var node2 = targets.Min;
                    var node = node2.Item1;
                    targets.Remove(node2);

                    visited.Add(node);

                    // dd = dds[node] * itM;
                    dd = dds[node];

                    result = iperfCache[node].Perform(dd);
                    dd = dissipate * result;
                    // if (false)
                    // if (Math.Abs(result) < 0.01)
                    if (Math.Abs(result) < Single.Epsilon)
                    {
                    }
                    else
                    {
                        foreach (var tos in graph.PostSet(node))
                        {
                            if (visited.Contains(tos.Item1) == false )
                            {
                                if (stopCache[tos.Item1] == true)
                                {
                                    float nv = cvalCache[node].cvalue;
                                    float cv = cvalCache[tos.Item1].cvalue;
                                    dds[node] = (cv - nv) * itM;
                                    nextStarts.Add(node);
                                }
                                else
                                {
                                    targets.Add(new Tuple<int, int>(tos.Item1, node2.Item2 + 1));
                                    dds[tos.Item1] = dd;
                                }
                            }
                        }
                    }
                }
            }
            starts = nextStarts;
            nextStarts = new SortedSet<int>();
            itM = itM * 0.5f;
        }
    }

    void Start()
    {
        foreach (int k in graph.AdjDict.Keys)
        {
            var obj = Hasklee.Instance.GameObjectFromId(k);
            iperfCache[k] = obj.GetComponent<IPerformer<float>>();
            stopCache[k] = obj.GetComponent<Stop>() != null ? true : false;
            cvalCache[k] = obj.GetComponent<ICValue<float>>();
        }

        var lc = gameObject.GetComponent<LuaController>();
        if (lc != null)
        {
            DynValue dv;
            dv = lc.lt.Get("iterations");
            if (dv.IsNotNil())
            {
                iterations = dv.ToObject<int>();
            }
            dv = lc.lt.Get("iterationM");
            if (dv.IsNotNil())
            {
                iterationM = dv.ToObject<float>();
            }
            dv = lc.lt.Get("dissipate");
            if (dv.IsNotNil())
            {
                iterationM = dv.ToObject<float>();
            }
        }
    }

    //I think smth was wrong with this
    class NodeLCompare : IComparer<Tuple<int, int>>
    {
        public int Compare(Tuple<int, int> x, Tuple<int, int> y)
        {
            int r = x.Item2 - y.Item2;
            if (r != 0)
            {
                return r;
            }
            else
            {
                return x.Item1 - y.Item1;
            }
        }
    }
}

}
