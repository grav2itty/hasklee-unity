using DG.Tweening;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hasklee {

public class LuaController : MonoBehaviour
{
    [NonSerialized]
    public Table lt;

    public HController hController;

    public static LuaController AddGetLuaController(GameObject go)
    {
        return AddGetLuaController(go, null);
    }

    public static LuaController AddGetLuaController(GameObject go, Table? table)
    {
        if (go == null)
        {
            return null;
        }

        var luaController = go.GetComponent<LuaController>();
        if (luaController == null)
        {
            luaController = go.AddComponent<LuaController>();
            var hController = go.GetComponent<HController>();
            if (hController != null)
            {
                Table lt = Lua.NewLuaGameObject(table, hController.ID);
                luaController.SetGlue(lt);
            }
            else
            {
                Table lt = Lua.NewLuaGameObject(table);
                luaController.SetGlue(lt);
            }
        }
        return luaController;
    }

    public void SetGlue(Table lt)
    {
        if (lt != null)
        {
            this.lt = lt;
            lt["gameObject"] = this;
            lt["go"] = this;

            hController = gameObject.GetComponent<HController>();
        }
    }

    void Awake()
    {
        if (lt != null)
        {
            DynValue v = lt.Get("awake");
            if (v.IsNotNil())
            {
                v.Function.Call(lt);
            }
        }
    }

    void Start()
    {
        if (lt != null)
        {
            DynValue v = lt.Get("start");
            if (v.IsNotNil())
            {
                v.Function.Call(lt);
            }
        }
    }
}

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

    public void SetParent(Transform parent, bool r)
    {
        luaController.transform.SetParent(parent, r);
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
