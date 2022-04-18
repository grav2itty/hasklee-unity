using UnityEngine;

namespace Hasklee {

//ADD SENSI
public class Drag : MonoBehaviour
{
    public float sensitivity;

#if HASKLEE_CURSOR
    void OnMouseDragN()
#else
    void OnMouseDrag()
#endif
    {
        float dx = CursorN.Instance.dx * sensitivity;
        float dy = CursorN.Instance.dy * sensitivity;
        transform.Translate(new Vector3(dx, dy, 0));
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
