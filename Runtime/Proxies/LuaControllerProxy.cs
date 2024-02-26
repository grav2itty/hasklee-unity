using DG.Tweening;
using MoonSharp.Interpreter;
using System;
using UnityEngine;

namespace Hasklee {

class LuaControllerProxy
{
    LuaController luaController;

    [MoonSharpHidden]
    public LuaControllerProxy(LuaController luaController)
    {
        this.luaController = luaController;
    }

    public AnimController animController
    {
        get => luaController.gameObject.GetComponent<AnimController>();
    }

    public CsoundUnity csound
    {
        get => luaController.gameObject.GetComponent<CsoundUnity>();
    }

    public int ID
    {
        get => luaController.hController.ID;
        set => luaController.hController.ID = value;
    }

    public IntGraphV idGraph
    {
        get => luaController.gameObject.GetComponent<IdGraphR>().graph;
    }

    public GraphPropagate graphPropagate
    {
        get => luaController.gameObject.GetComponent<GraphPropagate>();
    }

    public Light light
    {
        get => luaController.gameObject.GetComponent<Light>();
    }

    public String name
    {
        get => luaController.gameObject.name;
        set => luaController.gameObject.name = value;
    }

    public Table parent
    {
        get => luaController.gameObject.transform.parent.gameObject.GetComponent<LuaController>().lt;
    }

    public String tag
    {
        get => luaController.gameObject.tag;
        set => luaController.gameObject.tag = value;
    }

    public Transform transform
    {
        get => luaController.gameObject.transform;
    }

    public GameObject unityGameObject
    {
        get => luaController.gameObject;
    }

    public void DOLocalMove(Vector3 t, float d)
    {
        luaController.gameObject.transform.DOLocalMove(t, d).Play();
    }

    public void DOLocalRotate(Quaternion q, float d)
    {
        luaController.gameObject.transform.DOLocalRotateQuaternion(q, d);
    }

    public void DOLocalRotate(Vector3 a, float r, float d)
    {
        Vector3 v = a.normalized * Mathf.Sin(r/2);
        luaController.gameObject.transform.DOLocalRotateQuaternion(new Quaternion(v.x,v.y,v.z, Mathf.Cos(r/2)), d).Play();
    }

    public Color GetColor()
    {
        return luaController.hController.GetColor();
    }

    public float GetColorR()
    {
        return luaController.hController.GetColorR();
    }

    public float GetColorG()
    {
        return luaController.hController.GetColorG();
    }

    public float GetColorB()
    {
        return luaController.hController.GetColorB();
    }

    public Vector3 GetPosition()
    {
        return luaController.transform.position;
    }

    public Vector3 GetRotation()
    {
        return luaController.transform.eulerAngles;
    }

    public int idTable(int i)
    {
        return luaController.gameObject.GetComponent<IdT>().ids[i - 1];
    }

    public MonoBehaviour Mono(string s)
    {
        return luaController.gameObject.GetComponent(s) as MonoBehaviour;
    }

    public void PlayTween(string s)
    {
        animController.PlayTween(s);
    }

    public void SetColor(Color c)
    {
        luaController.hController.SetColor(c);
    }

    public void SetColorR(float r)
    {
        luaController.hController.SetColorR(r);
    }

    public void SetColorG(float r)
    {
        luaController.hController.SetColorG(r);
    }

    public void SetColorB(float r)
    {
        luaController.hController.SetColorB(r);
    }

    public void SetLightColorR(float x)
    {
        var light = luaController.gameObject.GetComponent<Light>();
        if (light != null)
        {
            Color c = light.color;
            c.r = x;
            light.color = c;
        }
    }

    public void SetLightColorG(float x)
    {
        var light = luaController.gameObject.GetComponent<Light>();
        if (light != null)
        {
            Color c = light.color;
            c.g = x;
            light.color = c;
        }
    }

    public void SetLightColorB(float x)
    {
        var light = luaController.gameObject.GetComponent<Light>();
        if (light != null)
        {
            Color c = light.color;
            c.b = x;
            light.color = c;
        }
    }

    public void SetParent(LuaController lc, bool r)
    {
        luaController.transform.SetParent(lc.transform, r);
    }

    public void SetParent(LuaController lc)
    {
        SetParent(lc, true);
    }

    public void SetPosition(Vector3 v)
    {
        luaController.transform.position = v;
    }

    public void SetRotation(Vector3 v)
    {
        luaController.transform.eulerAngles = v;
    }
}

}
