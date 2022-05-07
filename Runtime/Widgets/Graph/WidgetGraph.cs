using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hasklee {

public interface IGraphPropagate<T>
{
    void Init(IdGraphR graph, int start);
    void Propagate(T arg);
    void PropagateFrom(T arg, int start);
}

public class IdGraphR : MonoBehaviour
{
    public IntGraphV graph;

    // public void PropagateLua(string code, int start, Vector3 v)
    // {
    //     var gp = new GraphProp0(this, code, start);
    //     gp.PropagateFrom(v, start);
    // }

    // public void PropagateLua(DynValue fun, int start, Vector3 v)
    // {
    //     var gp = new GraphProp0(this, fun, start);
    //     gp.PropagateFrom(v, start);
    // }
}

public abstract class GraphPropagate : MonoBehaviour, IGraphPropagate<Vector3>
{
    public static void Init(GameObject go, int type, int firstNode, float s)
    {
        var idGraph = go.GetComponent<IdGraphR>();
        GraphPropagate gp = null;

        if (idGraph != null)
        {
            switch (type)
            {
                case 1:
                    gp = go.AddComponent<GraphPropagate1>();
                    gp.Init(idGraph, firstNode);
                    break;
                case 2:
                    gp = go.AddComponent<GraphPropagate2>();
                    gp.Init(idGraph, firstNode);
                    break;
                case 3:
                    gp = go.AddComponent<GraphPropagate3>();
                    gp.Init(idGraph, firstNode);
                    break;
                default:
                    gp = go.AddComponent<GraphPropagate1>();
                    gp.Init(idGraph, firstNode);
                    break;
            }
            gp.sensitivity = s;
            gp.start = firstNode;
        }
    }

    public float sensitivity;

    protected IntGraphV graph;
    protected int start;

    public abstract void Init(IdGraphR graph, int start);
    public abstract void Propagate(Vector3 arg);
    public abstract void PropagateFrom(Vector3 arg, int start);
}

}
