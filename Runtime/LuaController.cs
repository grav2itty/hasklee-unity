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

}
