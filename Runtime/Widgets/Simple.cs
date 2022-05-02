using UnityEngine;

namespace Hasklee {

public class Click : MonoBehaviour
{
    private int goID = 0;

#if HASKLEE_CURSOR
    void OnMouseUpAsButtonN()
#else
    void OnMouseUpAsButton()
#endif
    {
        Lua.Action(goID);
    }

    void Start()
    {
        goID = gameObject.ID();
    }
}


public class StringPluck : MonoBehaviour
{
    private int goID = 0;
    private ShaderSignal<float> shaderSignal;

#if HASKLEE_CURSOR
    void OnMouseExitN()
#else
    void OnMouseExit()
#endif
    {
        if (Input.GetMouseButton(0))
        {
            Lua.Action(goID, "string");
            if (shaderSignal != null)
            {
                shaderSignal.Signal(Time.timeSinceLevelLoad);
            }
        }
    }

    void Start()
    {
        goID = gameObject.ID();
        shaderSignal = gameObject.GetComponent<ShaderSignal<float>>();
    }
}

}
