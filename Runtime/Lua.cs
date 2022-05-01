using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Hasklee {

public class Lua : MonoBehaviour
{
    public static Script script;

    public static ScriptFunctionDelegate AddCsound;
    public static ScriptFunctionDelegate ClearLua;
    public static ScriptFunctionDelegate<Table> NewLuaGameObject;

    private static LuaExit luaExit;

    private static DynValue control;
    private static GameObject prototype;
    private static DynValue controlAction;


    public static void Action(params object[] list)
    {
        controlAction.Function.Call(list);
    }

    public static void AddActionR(int sender, int receiver, DynValue fun)
    {
        control.Table.Get("addActionR").Function.Call(control.Table, sender, receiver, fun.Function);
    }

    public static DynValue DoFunctionString(string code)
    {
        return script.DoString("return function(self, ...) " + code + " end");
    }

    public static void Instantiate(GameObject gameObject, Table lt)
    {
        if (gameObject == null)
        {
            return;
        }
        var newObject = UnityEngine.Object.Instantiate(gameObject);
        newObject.SetActive(false);
        var luaController = newObject.GetComponent<LuaController>();
        luaController.SetGlue(lt);
    }

    public static void SafeCall(DynValue fun, Table lt)
    {
        try
        {
            fun.Function.Call(lt);
        }
        catch (ScriptRuntimeException ex)
        {
            Debug.LogError(String.Format("Doh! An error occured! {0}",
                                         ex.DecoratedMessage));
        }
    }

    public static DynValue SafeDoString(string s)
    {
        try
        {
            DynValue result = script.DoString(s);
            return result;
        }
        catch (SyntaxErrorException ex)
        {
            Debug.LogError(String.Format("Doh! An error occured! {0}",
                                                     ex.DecoratedMessage));
            foreach (var foo in ex.CallStack)
            {
                Debug.LogError(foo.Name);
            }
            return DynValue.NewNil();
        }
    }

    void Awake()
    {
        if (script == null)
        {
            script = new Script(CoreModules.Preset_SoftSandbox);
            ((ScriptLoaderBase)script.Options.ScriptLoader).ModulePaths =
                new string[] { "./?", "./?.lua" };

            SetupCustomConverters();
            RegisterTypes();

            script.Globals["Instantiate"] = (Action<GameObject, Table>)Instantiate;
            luaExit = new LuaExit();
            script.Globals["LuaExit"] = luaExit;
            script.Globals["Log"] = (Action<string>)Debug.Log;
            script.Globals["Time"] = (Func<float>)(() => Time.realtimeSinceStartup);
        }

        if (prototype == null)
        {
            prototype = new GameObject();
            prototype.name = "Prototype";
            var luaController = prototype.AddComponent<LuaController>();

            script.DoFile("object");
            script.DoFile("control");
            script.DoFile("csound");
            script.DoFile("luagameobject");

            luaController.SetGlue(script.Globals.Get("LuaGameObject").Table);
        }

        control = script.Globals.Get("control");
        controlAction = script.Globals.Get("controlAction");
        AddCsound = script.Globals.Get("addCsound").Function.GetDelegate();
        ClearLua = script.Globals.Get("clearLua").Function.GetDelegate();
        NewLuaGameObject = script.Globals.Get("newLuaGameObject").Function.GetDelegate<Table>();
    }

    void OnApplicationQuit()
    {
        if (luaExit != null)
        {
            luaExit.RaiseTheEvent();
        }
    }

    void RegisterTypes()
    {
        UserData.RegisterType<EventArgs>();
        UserData.RegisterType<LuaExit>();

        UserData.RegisterType<GameObject>(InteropAccessMode.HideMembers);
        UserData.RegisterProxyType<TransformProxy, Transform>(r => new TransformProxy(r));
        UserData.RegisterProxyType<LightProxy, Light>(r => new LightProxy(r));
        UserData.RegisterProxyType<LuaControllerProxy, LuaController>(r => new LuaControllerProxy(r));
        UserData.RegisterProxyType<MonoBehaviourProxy, MonoBehaviour>(r => new MonoBehaviourProxy(r));
        UserData.RegisterProxyType<CsoundUnityProxy, CsoundUnity>(r => new CsoundUnityProxy(r));
    }

    void SetupCustomConverters()
    {
        Script.GlobalOptions.CustomConverters.SetClrToScriptCustomConversion<Vector3>
            (v => DynValue.NewTable(
                new Table(Lua.script, new DynValue[] {DynValue.NewNumber(v.x),
                                                      DynValue.NewNumber(v.y),
                                                      DynValue.NewNumber(v.z)})));

        Script.GlobalOptions.CustomConverters.SetClrToScriptCustomConversion<Vector4>
            (v => DynValue.NewTable(
                new Table(Lua.script, new DynValue[] {DynValue.NewNumber(v.x),
                                                      DynValue.NewNumber(v.y),
                                                      DynValue.NewNumber(v.z),
                                                      DynValue.NewNumber(v.w)})));

        Script.GlobalOptions.CustomConverters.SetClrToScriptCustomConversion<Quaternion>
            (v => DynValue.NewTable(
                new Table(Lua.script, new DynValue[] {DynValue.NewNumber(v.x),
                                                      DynValue.NewNumber(v.y),
                                                      DynValue.NewNumber(v.z),
                                                      DynValue.NewNumber(v.w)})));

        Script.GlobalOptions.CustomConverters.SetClrToScriptCustomConversion<Color>
            (v => DynValue.NewTable(
                new Table(Lua.script, new DynValue[] {DynValue.NewNumber(v.r),
                                                      DynValue.NewNumber(v.g),
                                                      DynValue.NewNumber(v.b),
                                                      DynValue.NewNumber(v.a)})));


        Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion
            (DataType.Table, typeof(Vector3), v =>
             new Vector3((float)v.Table.Get(1).Number,
                         (float)v.Table.Get(2).Number,
                         (float)v.Table.Get(3).Number));

        Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion
            (DataType.Nil, typeof(Vector3), v =>
             new Vector3(0f, 0f, 0f));

        Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion
            (DataType.Table, typeof(Vector4), v =>
             new Vector4((float)v.Table.Get(1).Number,
                         (float)v.Table.Get(2).Number,
                         (float)v.Table.Get(3).Number,
                         (float)v.Table.Get(4).Number));

        Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion
            (DataType.Nil, typeof(Vector4), v =>
             new Vector4(0f, 0f, 0f, 1f));

        Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion
            (DataType.Table, typeof(Quaternion), v =>
             new Quaternion((float)v.Table.Get(1).Number,
                            (float)v.Table.Get(2).Number,
                            (float)v.Table.Get(3).Number,
                            (float)v.Table.Get(4).Number));

        Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion
            (DataType.Table, typeof(Color), v =>
             new Color((float)v.Table.Get(1).Number,
                       (float)v.Table.Get(2).Number,
                       (float)v.Table.Get(3).Number,
                       (float)v.Table.Get(4).Number));

        Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion
            (DataType.Number, typeof(ConfigurableJointMotion), v =>
             (ConfigurableJointMotion)(Convert.ToInt32(v.Number)));

        var LightTypeDict = new Dictionary<string, LightType>();
        LightTypeDict.Add("spot", LightType.Spot);
        LightTypeDict.Add("directional", LightType.Directional);
        LightTypeDict.Add("point", LightType.Point);
        LightTypeDict.Add("area", LightType.Area);

        Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion
            (DataType.String, typeof(LightType), v => LightTypeDict[v.String]);

    }

    class LuaExit
    {
        public event EventHandler LuaExitHandler;

        public void RaiseTheEvent()
        {
            if (LuaExitHandler != null)
            {
                LuaExitHandler(this, EventArgs.Empty);
            }
        }
    }
}

}
