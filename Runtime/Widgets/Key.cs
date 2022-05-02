using DG.Tweening;
using UnityEngine;

namespace Hasklee {

public class Key : MonoBehaviour
{
    private static Key pressedKey;

    private bool pressed = false;
    private int goID = 0;
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
        if (Input.GetMouseButton(0) && CursorN.Instance.cursorMoved)
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
        if (CursorN.Instance.cursorMoved)
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


    void Start()
    {
        var animController = gameObject.GetComponent<AnimController>();
        if (animController != null)
        {
            tween = animController.TweenFromKey("key");
        }
        goID = gameObject.ID();
    }

    private void KeyPress()
    {
        if ((pressedKey != null) && (pressedKey != this))
        {
            pressedKey.KeyRelease();
        }

        Lua.Action(goID, "key", 1);

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

    private void KeyRelease()
    {
        if (pressedKey == null) return;
        if (pressed == false) return;
        pressedKey = null;
        pressed = false;

        Lua.Action(goID, "key", 0);

        if (tween != null)
        {
            tween.PlayBackwards();
        }
    }

}

public class Colada : MonoBehaviour
{
    private Key key;

#if HASKLEE_CURSOR
    void OnMouseDownN()
#else
    void OnMouseDown()
#endif
    {
        if (key != null)
        {
            key.OnMouseDownA();
        }
    }

#if HASKLEE_CURSOR
    void OnMouseEnterN()
#else
    void OnMouseEnter()
#endif
    {
        if (key != null)
        {
            key.OnMouseEnterA();
        }
    }

#if HASKLEE_CURSOR
    void OnMouseExitN()
#else
    void OnMouseExit()
#endif
    {
        if (key != null)
        {
            key.OnMouseExitA();
        }
    }

#if HASKLEE_CURSOR
    void OnMouseUpN()
#else
    void OnMouseUp()
#endif
    {
        if (key != null)
        {
            key.OnMouseUpA();
        }
    }

    void Start()
    {
        key = transform.parent.gameObject.GetComponent<Key>();
    }
}

}
