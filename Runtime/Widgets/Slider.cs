using DG.Tweening;
using System;
using UnityEngine;

namespace Hasklee {

public class Slider : MonoBehaviour {

    public float initp;
    public float sensitivity;

    private float r = 0.0f;
    private float momentum;
    private bool goingForward;

    private Tween tween;
    private Tween tweenBefore;
    private Tween tweenAfter;

    private GameObject before;
    private GameObject after;

    private float pathLength;
    private float space;

    private int targetID = 0;

    void Start() {

        targetID = gameObject.ID();

        before = new GameObject();
        before.name = "before";
        before.transform.SetParent(gameObject.transform.parent);
        before.transform.position = gameObject.transform.position;
        before.transform.rotation = gameObject.transform.rotation;
        before.transform.localScale = gameObject.transform.localScale;

        after = new GameObject();
        after.name = "after";
        after.transform.SetParent(gameObject.transform.parent);
        after.transform.position = gameObject.transform.position;
        after.transform.rotation = gameObject.transform.rotation;
        after.transform.localScale = gameObject.transform.localScale;

        var key = "slider";
        var path = gameObject.GetComponent<AnimController>().GetPath(key);
        if (path != null)
        {
            tween = AnimController.TweenFromPath(gameObject.transform, path);
            tweenBefore = AnimController.TweenFromPath(before.transform, path);
            tweenAfter = AnimController.TweenFromPath(after.transform, path);

            //NOT HERE
            pathLength = TransformV.PathLength(path);

            space = 0.05f / pathLength;

            r = initp;

            tween.Goto(r);
            tweenBefore.Goto(r - space);
            tweenAfter.Goto(r + space);
        }
    }

#if HASKLEE_CURSOR
    void OnMouseDownN()
    {
        CursorN.Instance.directControl = false;
#else
    void OnMouseDown()
    {
#endif
        momentum = 1;
    }

#if HASKLEE_CURSOR
    void OnMouseDragEndN()
    {
        CursorN.Instance.directControl = true;
        // momentum = 0;
    }
#endif

#if HASKLEE_CURSOR
    void OnMouseDragN()
#else
    void OnMouseDrag()
#endif
    {
        float dx = CursorN.Instance.dx * sensitivity;
        float dy = CursorN.Instance.dy * sensitivity;

        if (Math.Abs(dx) + Math.Abs(dy) == 0f)
        {
            return;
        }

        var pos = gameObject.transform.position;
        var afterPosW = after.transform.position;
        var beforePosW = before.transform.position;

#if HASKLEE_CURSOR
        var posVP = CursorN.Instance.WorldToViewportPoint(pos);
        var afterPosVP = CursorN.Instance.WorldToViewportPoint(afterPosW);
        var beforePosVP = CursorN.Instance.WorldToViewportPoint(beforePosW);
#else
        var camera = Camera.main;
        var posVP = camera.WorldToViewportPoint(pos);
        var afterPosVP = camera.WorldToViewportPoint(afterPosW);
        var beforePosVP = camera.WorldToViewportPoint(beforePosW);
#endif

        var V = (afterPosVP - posVP).normalized; var V2 = (beforePosVP - posVP).normalized;
        var ddd = (V.x*dx + V.y*dy) * posVP.z;
        var ddd2 = (V2.x*dx + V2.y*dy) * posVP.z;
        // var ddd = (V.x*dx + V.y*dy + V.z*dy) * posVP.z;
        // var ddd2 = (V2.x*dx + V2.y*dy + V2.z*dy ) * posVP.z;

        float d;

        if (goingForward == true)
        {
            ddd *= momentum;
        }
        else
        {
            ddd2 *= momentum;
        }


        if (ddd > ddd2)
        {
            UpdateMomentum(true);
            d = ddd;
            // d = 0.1f;
        }
        else
        {
            UpdateMomentum(false);
            d = -ddd2;
            // d = -0.1f;
        }

        // Slide(d/pathLength);
        Slide(d);
    }

    void UpdateMomentum(bool dir)
    {
        if (goingForward == dir)
        {
            momentum = momentum * 1.01f;
        }
        else
        {
            momentum = 1f;
        }
        goingForward = dir;
    }

    void Slide (float d)
    {
        r += d;
        r = Math.Max(Math.Min(r, 1.0f), 0.0f);

        Lua.Action(targetID, r);

        tween.Goto(r, false);
        tweenAfter.Goto(r + space, false);
        tweenBefore.Goto(r - space, false);

#if HASKLEE_CURSOR
        CursorN.Instance.viewportPosition = CursorN.Instance.WorldToViewportPoint(transform.position);
#endif
    }
}

}
