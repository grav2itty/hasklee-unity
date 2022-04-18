using UnityEngine;

namespace Hasklee {

public interface IEnable
{
    void Enable();
}

public interface ICValue<T>
{
    T cvalue { get; set; }
}

public interface IPerformer<T>
{
    T Perform(T arg);
}

public delegate void Performs<T>(T arg);

public interface IConductor<T>
{
    void Conduct(T arg);
    Performs<T> performers { get; set; }
}


public interface ShaderSignal<T>
{
    void Signal(T sig);
}

public interface ShaderTransformUp
{
    void ShaderTransformUp();
}

public interface ShaderColorUp
{
    void ShaderColorUp(Color c);
}


public class IdT : MonoBehaviour
{
    public int[] ids;
}

}
