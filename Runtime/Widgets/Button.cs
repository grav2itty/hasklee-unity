using DG.Tweening;
using UnityEngine;

namespace Hasklee {

public class Button : MonoBehaviour
{
    private bool beingPressed;
    private int goID = 0;
    private ShaderTransformUp shaderTransformUp;

#if HASKLEE_CURSOR
    void OnMouseUpAsButtonN()
#else
    void OnMouseUpAsButton()
#endif
    {
        if (beingPressed == false)
        {
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
            Lua.Action(goID, "button");
        }
    }

    void Start()
    {
        beingPressed = false;
        goID = gameObject.ID();
        shaderTransformUp = gameObject.GetComponent<ShaderTransformUp>();
    }

}

}
