using UnityEngine;

namespace Hasklee {

public class Trigger : MonoBehaviour
{
    private int goID = 0;

    void OnTriggerEnter(Collider other)
    {
        var lc = other.gameObject.GetComponent<LuaController>();
        if (lc != null)
            Lua.Action(goID, "TriggerEnter", other.gameObject);
        else
            Lua.Action(goID, "TriggerEnter");
    }

    void Start()
    {
        goID = gameObject.ID();
    }
}

}
