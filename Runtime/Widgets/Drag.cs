using UnityEngine;

namespace Hasklee {

public class Drag : Conductor<Vector3>
{
    public float sensitivity;

    private int goID = 0;

#if HASKLEE_CURSOR
    void OnMouseDragN()
#else
    void OnMouseDrag()
#endif
    {
        Lua.Action(goID, "drag", Value());
        DoDrag();
    }

#if HASKLEE_CURSOR
    void OnMouseDragEndN()
    {
        CursorN.Instance.FollowObject(null);
        Lua.Action(goID, "dragE", Value());
        DoDrag();
    }

    void OnMouseDragStartN()
    {
        CursorN.Instance.FollowObject(gameObject);
        Lua.Action(goID, "dragS", Value());
        DoDrag();
    }
#endif

    void Start()
    {
        goID = gameObject.ID();

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

    private void DoDrag()
    {
        Conduct(Value());
    }

    private Vector3 Value()
    {
        float dx = CursorN.Instance.dx * sensitivity;
        float dy = CursorN.Instance.dy * sensitivity;
        return new Vector3(dx, dy, 0);
    }
}

}
