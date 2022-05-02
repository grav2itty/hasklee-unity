using DG.Tweening;
using System;
using UnityEngine;

namespace Hasklee {

public class Bell : MonoBehaviour
{
    private Vector3 lastAngV;
    private Rigidbody rigidBody;
    private int goID;

    void FixedUpdate()
    {
        if (rigidBody != null)
        {
            if (Vector3.Dot(lastAngV, rigidBody.angularVelocity) < 0)
            {
                Lua.Action(goID, "bell");
            }
            lastAngV = rigidBody.angularVelocity;
        }
    }

    void Start()
    {
        lastAngV = new Vector3(0,0,0);
        goID = gameObject.ID();
        rigidBody = gameObject.GetComponent<Rigidbody>();
    }
}

}
