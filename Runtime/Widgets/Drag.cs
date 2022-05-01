using UnityEngine;

namespace Hasklee {

public class Drag : MonoBehaviour
{
    public float sensitivity;

    private int targetID = 0;

#if HASKLEE_CURSOR
    void OnMouseDragN()
#else
    void OnMouseDrag()
#endif
    {
        Lua.Action(targetID, "drag", Value());
    }

#if HASKLEE_CURSOR
    void OnMouseDragEndN()
    {
        Lua.Action(targetID, "dragE", Value());
    }

    void OnMouseDragStartN()
    {
        Lua.Action(targetID, "dragS", Value());
    }
#endif

    void Start()
    {
        targetID = gameObject.ID();
    }

    private Vector3 Value()
    {
        float dx = CursorN.Instance.dx * sensitivity;
        float dy = CursorN.Instance.dy * sensitivity;
        return new Vector3(dx, dy, 0);
    }
}

public class DragC : MonoBehaviour, IConductor<Vector3>, IEnable
{
    public float sensitivity;

    public Performs<Vector3> performers
    {
        get;
        set;
    }

    public void Conduct(Vector3 v)
    {
        if (performers != null)
        {
            performers(v);
        }
    }

    public void Enable()
    {
        enabled = true;
    }

#if HASKLEE_CURSOR
    void OnMouseDragN()
#else
    void OnMouseDrag()
#endif
    {
        if (enabled == true)
        {
            float dx = CursorN.Instance.dx * sensitivity;
            float dy = CursorN.Instance.dy * sensitivity;
            Conduct(new Vector3(dx, dy, 0));
        }
    }
}

public class DragSelf : DragC
{
    void Start()
    {
        var performer1 = gameObject.GetComponent<IPerformer<Vector3>>();
        if (performer1 != null)
        {
            performers += v => { performer1.Perform(v); };
        }
        else
        {
            var performer2 = gameObject.GetComponent<IPerformer<float>>();
            if (performer2 != null)
            {
                performers += v => { performer2.Perform((v.x + v.y + v.z)); };
            }
        }
    }
}

}
