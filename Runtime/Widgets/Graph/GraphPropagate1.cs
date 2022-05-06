using System.Collections.Generic;
using UnityEngine;

namespace Hasklee {

//moves to post nodes when current node is done
//that is result does not match requested action
public class GraphPropagate1 : GraphPropagate
{
    private SortedSet<int> targets;
    private SortedSet<int> toFollow;
    private SortedSet<int> toRemove;

    //do some cache

    public override void Init(IdGraphR g, int start)
    {
        targets = new SortedSet<int>();
        toFollow = new SortedSet<int>();
        toRemove = new SortedSet<int>();

        graph = g.graph;
        targets.Add(start);
    }

    public override void Propagate(Vector3 v)
    {
        if (targets.Count == 0 ) return;

        toFollow.Clear();
        toRemove.Clear();
        float result;
        float d = (v.x + v.y) * sensitivity;
        float dd = d / targets.Count;
        if (dd > 0)
        {
            foreach (var node in targets)
            {
                result = Hasklee.Instance.GameObjectFromId(node).GetComponent<IPerformer<float>>().Perform(dd);
                if (result == 0 && toRemove.Count < (targets.Count - 1))
                {
                    toRemove.Add(node);
                }
                else if (result < dd)
                {
                    toFollow.Add(node);
                }
            }
            foreach (var node in toFollow)
            {
                foreach (var tos in graph.PostSet(node))
                {
                    if (tos.Item2 > 0)
                    {
                        targets.Add(tos.Item1);
                    }
                }
            }
        }
        else if (dd < 0)
        {
            foreach (var node in targets)
            {
                result = Hasklee.Instance.GameObjectFromId(node).GetComponent<IPerformer<float>>().Perform(dd);
                if (result == 0 && toRemove.Count < (targets.Count - 1))
                {
                    toRemove.Add(node);
                }
                else if (result > dd)
                {
                    toFollow.Add(node);
                }
            }
            foreach (var node in toFollow)
            {
                foreach (var tos in graph.PostSet(node))
                {
                    if (tos.Item2 < 0)
                    {
                        targets.Add(tos.Item1);
                    }
                }
            }
        }

        targets.ExceptWith(toRemove);
    }

    public override void PropagateFrom(Vector3 v, int start)
    {
        Propagate(v);
    }
}

}
