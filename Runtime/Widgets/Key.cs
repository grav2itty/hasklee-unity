using DG.Tweening;
using UnityEngine;

namespace Hasklee {

public class Colada : MonoBehaviour
{
    private Key key;

    void Start()
    {
        key = transform.parent.gameObject.GetComponent<Key>();
    }

#if HASKLEE_CURSOR
    void OnMouseDownN() {
#else
    void OnMouseDown() {
#endif
        if (key != null)
        {
            key.OnMouseDownA();
        }
    }

#if HASKLEE_CURSOR
    void OnMouseEnterN() {
#else
    void OnMouseEnter() {
#endif
        if (key != null)
        {
            key.OnMouseEnterA();
        }
    }

#if HASKLEE_CURSOR
    void OnMouseExitN() {
#else
    void OnMouseExit() {
#endif
        if (key != null)
        {
            key.OnMouseExitA();
        }
    }

#if HASKLEE_CURSOR
    void OnMouseUpN() {
#else
    void OnMouseUp() {
#endif
        if (key != null)
        {
            key.OnMouseUpA();
        }
    }
}

public class Key : MonoBehaviour
{
    private static Key pressedKey;
    private static bool anyKeyPressed = false;

    private bool pressed = false;
    private int targetID = 0;
    private Tween tween;

#if HASKLEE_CURSOR
    void OnMouseDownN() => OnMouseDownA();
    void OnMouseEnterN() => OnMouseEnterA();
    void OnMouseExitN() => OnMouseExitA();
    void OnMouseUpN() => OnMouseUpA();
#else
    void OnMouseDown() => OnMouseDownA();
    void OnMouseEnter() => OnMouseEnterA();
    void OnMouseExit() => OnMouseExitA();
    void OnMouseUp() => OnMouseUpA();
#endif

    public void OnMouseDownA()
    {
        KeyPress();
    }

    public void OnMouseEnterA()
    {
#if HASKLEE_CURSOR
        if (Input.GetMouseButton(0) && CursorN.Instance.cursorMoved())
#else
        if (Input.GetMouseButton(0))
#endif
        {
            KeyPress();
        }

    }

    public void OnMouseExitA()
    {
#if HASKLEE_CURSOR
        if (CursorN.Instance.cursorMoved())
#else
        if (true)
#endif
        {
            KeyRelease();
        }
    }

    public void OnMouseUpA()
    {
        KeyRelease();
    }

    void KeyPress()
    {
        if (anyKeyPressed && (pressedKey != this))
        {
            pressedKey.KeyRelease();
        }

        Lua.Action(targetID, 1);

        anyKeyPressed = true;
        pressedKey = this;
        pressed = true;

        if (tween != null)
        {
            tween.SetLoops(0);
            if (tween.IsBackwards())
                tween.Flip();
            tween.Play();
        }
    }

    void KeyRelease()
    {
        if (anyKeyPressed == false)
            return;
        if (pressed == false)
            return;
        anyKeyPressed = false;
        pressed = false;

        Lua.Action(targetID, 0);

        if (tween != null)
        {
            tween.PlayBackwards();
        }
    }

    void Start()
    {
        var animController = gameObject.GetComponent<AnimController>();
        if (animController != null)
        {
            tween = animController.TweenFromKey("key");
        }
        targetID = gameObject.ID();
    }
}

}
