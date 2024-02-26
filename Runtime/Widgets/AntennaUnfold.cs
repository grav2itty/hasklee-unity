using DG.Tweening;
using System;
using UnityEngine;

namespace Hasklee {

public class AntennaUnfold : MonoBehaviour
{
    public float sensitivity;

    private float r = 0;
    public int goID;
    private AntennaUnfold first;
    private AntennaUnfold last;
    private Tween tween;

#if HASKLEE_CURSOR
    void OnMouseDragN()
#else
    void OnMouseDrag()
#endif
    {
        Lua.Action(goID, "aunfold", Value());
        DoDrag();
    }

#if HASKLEE_CURSOR
    void OnMouseDragEndN()
    {
        CursorN.Instance.FollowObject(null);
        Lua.Action(goID, "aunfoldE", Value());
        DoDrag();
    }

    void OnMouseDragStartN()
    {
        if (r < 0.1 && first != null && first.gameObject == gameObject && last != null)
        {
            CursorN.Instance.FollowObject(last.gameObject);
        }
        else
        {
            CursorN.Instance.FollowObject(gameObject);
        }
        Lua.Action(goID, "aunfoldS", Value());
        DoDrag();
    }
#endif

    void Start()
    {
        var stop = gameObject.GetComponentInParent<Stop>();
        if (stop != null)
        {
            first = stop.gameObject.GetComponent<AntennaUnfold>();
            goID = first.gameObject.ID();
            last = Last();
        }

        var animController = gameObject.GetComponent<AnimController>();
        if (animController != null)
        {
            tween = animController.TweenFromKey("slider");
        }
    }

    private void DoDrag()
    {
        r = Value();
        if (first != null)
        {
            first.Slide(r);
        }
        else
        {
            Slide(r);
        }
    }

    private AntennaUnfold Last()
    {
        foreach (Transform child in gameObject.transform)
        {
            var au = child.gameObject.GetComponent<AntennaUnfold>();
            if (au != null)
            {
                return au.Last();
            }
        }
        return this;
    }

    private void Slide(float d)
    {
        r = d;

        if (first.gameObject != gameObject)
        {
            tween.Goto(d, false);
        }

        foreach (Transform child in gameObject.transform)
        {
            var au = child.gameObject.GetComponent<AntennaUnfold>();
            if (au != null)
            {
                au.Slide(d);
            }
        }
    }

    private float Value()
    {
        float dd = (CursorN.Instance.dx + CursorN.Instance.dy) * sensitivity;
        return Math.Max(Math.Min(r + dd, 1.0f), 0.0f);
    }
}

}
