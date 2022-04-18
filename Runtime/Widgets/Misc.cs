using DG.Tweening;
using System;
using UnityEngine;

namespace Hasklee {

public class ParentRotator : MonoBehaviour
{
    public float sensitivity = 10;

    void Awake()
    {
        enabled = false;
    }

#if HASKLEE_CURSOR
    void OnMouseDragN()
#else
    void OnMouseDrag()
#endif
    {
        if (enabled && transform.parent)
        {
            float dx = CursorN.Instance.dx * sensitivity;
            float dy = CursorN.Instance.dy * sensitivity;
            var pointW = transform.parent.position;
            var axisW = transform.parent.forward;
            transform.RotateAround(pointW, axisW, dx+dy);
        }
    }
}

public class Bell : MonoBehaviour
{
    private Vector3 lastAngV;
    private Rigidbody rigidBody;
    private int targetID = 0;

    void FixedUpdate()
    {
        if (rigidBody != null)
        {
            if (Vector3.Dot(lastAngV, rigidBody.angularVelocity) < 0)
            {
                Lua.Action(targetID, 0);
                // Lua.Action(hController.ID, 0);
            }
            lastAngV = rigidBody.angularVelocity;
        }
    }

    void Start()
    {
        lastAngV = new Vector3(0,0,0);
        targetID = gameObject.ID();
        rigidBody = gameObject.GetComponent<Rigidbody>();
    }
}

}
