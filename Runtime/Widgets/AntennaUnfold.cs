using DG.Tweening;
using System;
using UnityEngine;

namespace Hasklee {

public class Stop : MonoBehaviour {}

public class AntennaUnfold : MonoBehaviour
{
    public float sensitivity;

    private float r = 0;
    private Tween tween;

#if HASKLEE_CURSOR
    void OnMouseDragN()
#else
    void OnMouseDrag()
#endif
    {
        float dd = CursorN.Instance.dx * sensitivity + CursorN.Instance.dy * sensitivity;

        var rs = gameObject.GetComponentInParent<Stop>();
        if (rs)
        {
            var hController = rs.gameObject.GetComponent<HController>();
            if (hController != null)
            {
                Lua.Action(hController.ID, r);
            }

            var ss = rs.gameObject.GetComponent<AntennaUnfold>();
            if (ss)
            {
                ss.Slide(dd);
            }
        }
        else
        {
            Slide(dd);
        }
    }

    void Slide(float d)
    {
        r += d;
        r = Math.Max(Math.Min(r, 1.0f), 0.0f);

        if (gameObject.GetComponent<Stop>() == null)
        {
            tween.Goto(r, false);
        }

        foreach (Transform child in gameObject.transform)
        {
            var c = child.gameObject.GetComponent<AntennaUnfold>();
            if (c != null)
            {
                c.Slide(d);
            }
        }
    }

    void Start()
    {
        var animController = gameObject.GetComponent<AnimController>();
        if (animController != null)
        {
            tween = animController.TweenFromKey("slider");
        }
    }
}

}
