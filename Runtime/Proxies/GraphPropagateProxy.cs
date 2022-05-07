using MoonSharp.Interpreter;
using UnityEngine;

namespace Hasklee {

public class GraphPropagateProxy
{
    GraphPropagate graphPropagate;

    [MoonSharpHidden]
    public GraphPropagateProxy(GraphPropagate graphPropagate)
    {
        this.graphPropagate = graphPropagate;
    }

    public void Propagate(Vector3 v)
    {
        graphPropagate.Propagate(v);
    }

    public void PropagateFrom(Vector3 v, int start)
    {
        graphPropagate.PropagateFrom(v, start);
    }
}

}
