using MoonSharp.Interpreter;
using UnityEngine;

namespace Hasklee {

class MonoBehaviourProxy
{
    MonoBehaviour mono;

    [MoonSharpHidden]
    public MonoBehaviourProxy(MonoBehaviour mono)
    {
        this.mono = mono;
    }

    public bool enabled
    {
        get => mono.enabled;
        set => mono.enabled = value;
    }
}

}
