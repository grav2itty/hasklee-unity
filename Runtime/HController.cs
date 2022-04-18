using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hasklee {

public static class Extensions
{
    public static T AddGetComponent<T>(this GameObject go) where T : Component
    {
        T t = go.GetComponent<T>();
        return t ? t : go.AddComponent<T>();
    }

    public static int ID(this GameObject go)
    {
        var hController = go.GetComponent<HController>();
        if (hController != null)
        {
            return hController.ID;
        }
        else
        {
            return 0;
        }
    }
}

public class Instan : MonoBehaviour
{
    public int prefabRef;
    public byte[] attribs;
}

public class HController : MonoBehaviour
{
    public int ID;

    public Color color = new Color(1,1,1,1);
    public Color specular = new Color(1,1,1,1);

    private ShaderColorUp scup;
    [SerializeField] private MeshRenderer meshRenderer;

    public static MaterialPropertyBlock matProp;

    void Start()
    {
#if HASKLEE_SHADERS
        scup = gameObject.GetComponent<ShaderColorUp>();
#else
        scup = null;
#endif
        if (scup == null)
        {
            meshRenderer = gameObject.GetComponent<MeshRenderer>();
            SetColor(color);
        }
    }

    public void ActionR(int ids, string s)
    {
        ActionR(ids, Lua.DoFunctionString(s));
    }

    public void ActionR(int ids, DynValue fun)
    {
        Lua.AddActionR(ids, ID, fun);
    }

    public void ActionS(int ids, string s)
    {
        ActionS(ids, Lua.DoFunctionString(s));
    }

    public void ActionS(int ids, DynValue fun)
    {
        Lua.AddActionR(ID, ids, fun);
    }

    public float GetColorR()
    {
        return color.r;
    }

    public float GetColorG()
    {
        return color.g;
    }

    public float GetColorB()
    {
        return color.b;
    }

    public void SetColorR(float r)
    {
        color.r = r;
        ApplyColor();
    }

    public void SetColorG(float r)
    {
        color.g = r;
        ApplyColor();
    }

    public void SetColorB(float r)
    {
        color.b = r;
        ApplyColor();
    }

    public Color GetColor()
    {
        return color;
    }

    public void SetColor(Color c)
    {
        color = c;
        ApplyColor();
    }

    private void ApplyColor()
    {
        if (scup != null)
        {
            scup.ShaderColorUp(color);
        }
        else if(meshRenderer != null)
        {
            meshRenderer.GetPropertyBlock(matProp);
            matProp.SetColor("_Color", color);
            meshRenderer.SetPropertyBlock(matProp);
        }
    }
}

}
