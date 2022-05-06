using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hasklee {

public class GraphPropagate2 : GraphPropagate
{
    private SortedSet<int> visited;
    private float off = 1.0f;

    public override void Init(IdGraphR g, int start)
    {
        visited = new SortedSet<int>();
        graph = g.graph;
    }

    public override void Propagate(Vector3 v) => PropagateFrom(v, start);

    public override void PropagateFrom(Vector3 v, int start)
    {
        float d = (v.x + v.y) * sensitivity;
        visited.Clear();
        Propagate(d, start);
    }

    private void Propagate(float d, int node)
    {
        visited.Add(node);
        float result;
        result = Hasklee.Instance.GameObjectFromId(node).GetComponent<IPerformer<float>>().Perform(d);

        foreach (var tos in graph.PostSet(node))
        {
            if (visited.Contains(tos.Item1) == false)
            {
                float ncv = Hasklee.Instance.GameObjectFromId(tos.Item1).GetComponent<ICValue<float>>().cvalue;
                float cv = Hasklee.Instance.GameObjectFromId(node).GetComponent<ICValue<float>>().cvalue;
                if (Math.Abs(cv - ncv) >= off)
                {
                    Propagate(d, tos.Item1);
                }
            }
        }
    }

    void Start()
    {
        var lc = gameObject.GetComponent<LuaController>();
        if (lc != null)
        {
            DynValue dv;
            dv = lc.lt.Get("off");
            if (dv.IsNotNil())
            {
                off = dv.ToObject<float>();
            }
        }
    }
}

}
