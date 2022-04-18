using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Hasklee {

public class Hasklee : MonoBehaviour
{
    public static Hasklee Instance { get; private set; }

    public Material defaultMaterial;
    public bool groupHasklee;
    public bool groupPrefabs;

    private Dictionary<int, GameObject> idgo = new Dictionary<int, GameObject>();

    private Dictionary<string, Type> customComponents = new Dictionary<string, Type>();


    public void AddId(int id, GameObject go)
    {
        if (idgo.ContainsKey(id) == false)
        {
            idgo.Add(id, go);
        }
        else
        {
            Debug.LogWarning("Trying to add duplicate ID");
        }
    }

    public void Destroy()
    {
        Lua.ClearLua();
        idgo.Clear();
        Init.Destroy();
    }

    public GameObject GameObjectFromId(int id)
    {
        GameObject go;
        idgo.TryGetValue(id, out go);
        return go;
    }

    public Type GetComponentType(string s)
    {
        Type t;
        customComponents.TryGetValue(s, out t);
        return t;
    }

    public void Load(BinaryReader reader)
    {
        Init.ReadScene(reader);
    }

    public void LoadFile(string fileName)
    {
        if (File.Exists(fileName))
        {
            using (BinaryReader reader = new BinaryReader(File.Open(fileName, FileMode.Open)))
            {
                Load(reader);
            }
        }
    }

    public void RegisterComponent<T>()
    {
        customComponents.Add(typeof(T).Name, typeof(T));
    }

    void Awake()
    {
        Instance = this;
        HController.matProp = new MaterialPropertyBlock();
    }
}

}
