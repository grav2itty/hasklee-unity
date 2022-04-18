using DG.Tweening;
using UnityEngine;

namespace Hasklee {

public class Click : MonoBehaviour
{
    private int targetID = 0;

#if HASKLEE_CURSOR
    void OnMouseUpAsButtonN()
#else
    void OnMouseUpAsButton()
#endif
    {
        Lua.Action(targetID);
    }

    void Start()
    {
        targetID = gameObject.ID();
    }
}

public class Button : MonoBehaviour
{
    private bool beingPressed;
    private int targetID = 0;
    private ShaderTransformUp shaderTransformUp;

#if HASKLEE_CURSOR
    public void OnMouseUpAsButtonN()
#else
    public void OnMouseUpAsButton()
#endif
    {
        if (!beingPressed) {
            beingPressed = true;
            Tween tween = gameObject.transform
                .DOMove(gameObject.transform.position + gameObject.transform.forward * 0.1f, .5f)
                .SetLoops(2, LoopType.Yoyo)
                .OnComplete(() => beingPressed = false)
                .SetAutoKill(true);

            if (shaderTransformUp != null)
            {
                tween.OnUpdate(
                    () =>
                    {
                        shaderTransformUp.ShaderTransformUp();
                    }
                );
            }

            tween.Play();
            Lua.Action(targetID);
        }
    }

    void Start()
    {
        beingPressed = false;
        targetID = gameObject.ID();
        shaderTransformUp = gameObject.GetComponent<ShaderTransformUp>();
    }

}

public class StringPluck : MonoBehaviour
{
    private int targetID = 0;
    private ShaderSignal<float> shaderSignal;

#if HASKLEE_CURSOR
    void OnMouseExitN()
#else
    void OnMouseExit()
#endif
    {
        if (Input.GetMouseButton(0))
        {
            Lua.Action(targetID);
            if (shaderSignal != null)
            {
                shaderSignal.Signal(Time.timeSinceLevelLoad);
            }
        }
    }

    void Start()
    {
        targetID = gameObject.ID();
        shaderSignal = gameObject.GetComponent<ShaderSignal<float>>();
    }
}

}
