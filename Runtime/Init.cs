using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

namespace Hasklee {

class Init
{
    private static List<GameObject> prefabs = new List<GameObject>();
    private static List<GameObject> meshRefs = new List<GameObject>();
    private static List<DynValue> funRefs = new List<DynValue>();
    private static List<List<TransformV>> paths = new List<List<TransformV>>();
    private static List<Texture2D> textures = new List<Texture2D>();
    private static List<Material> materials = new List<Material>();

    private static List<GameObject> gameObjects = new List<GameObject>();

    private static List<Dictionary<string, int>> namedPaths = new List<Dictionary<string, int>>();
    //shared by prefab instances
    private static Dictionary<int, Dictionary<string, TweenOpts>> sharedTweenOpts =
        new Dictionary<int, Dictionary<string, TweenOpts>>();

    private static List<SetupA> setupActions = new List<SetupA>();

    private static Dictionary<string, Material> resourceMaterials =
        new Dictionary<string, Material>(StringComparer.Ordinal);

    private static bool readNormals;
    private static bool readUVs;
    private static bool readColors;

    private static GameObject haskleeGroup;
    private static GameObject prefabGroup;
    private static GameObject meshRefGroup;


    interface SetupA
    {
        void Setup();
    }


    public static List<TransformV> GetPath(int pathsRef, string s)
    {
        if(namedPaths[pathsRef].ContainsKey(s))
        {
            return paths[namedPaths[pathsRef][s]];
        }
        else
        {
            return null;
        }
    }

    public static Dictionary<string, TweenOpts> GetTweenOpts(int pathsRef)
    {
        if(sharedTweenOpts.ContainsKey(pathsRef))
        {
            return sharedTweenOpts[pathsRef];
        }
        else
        {
            return null;
        }
    }

    public static void Destroy()
    {
        Debug.Log("**********HASKLEE DESTROY SCENE**********");
        foreach (GameObject v in gameObjects)
        {
            UnityEngine.Object.Destroy(v);
        }
        gameObjects.Clear();

        foreach (GameObject v in meshRefs)
        {
            UnityEngine.Object.Destroy(v);
        }
        meshRefs.Clear();

        foreach (var v in prefabs)
        {
            UnityEngine.Object.Destroy(v);
        }
        prefabs.Clear();

        foreach (var v in materials)
        {
            UnityEngine.Object.Destroy(v);
        }
        materials.Clear();

        foreach (var v in textures)
        {
            UnityEngine.Object.Destroy(v);
        }
        textures.Clear();

        funRefs.Clear();
        sharedTweenOpts.Clear();
        paths.Clear();

        resourceMaterials.Clear();

        if (meshRefGroup != null)
        {
            UnityEngine.Object.Destroy(meshRefGroup);
        }
        if (prefabGroup != null)
        {
            UnityEngine.Object.Destroy(prefabGroup);
        }
        if (haskleeGroup != null)
        {
            UnityEngine.Object.Destroy(haskleeGroup);
        }

#if HASKLEE_SHADERS
        ProcMan.Instance.Destroy();
#endif
    }

    public static void ReadScene(BinaryReader reader)
    {
        Debug.Log("**********HASKLEE READ SCENE**********");

        meshRefGroup = new GameObject();
        meshRefGroup.name = "MeshRefs";
        if (Hasklee.Instance.groupPrefabs == true)
        {
            prefabGroup = new GameObject();
            prefabGroup.name = "Prefabs";
        }

        if (Hasklee.Instance.groupHasklee == true)
        {
            haskleeGroup = new GameObject();
            haskleeGroup.name = "Hasklee";
            meshRefGroup.transform.SetParent(haskleeGroup.transform);
            if (prefabGroup != null)
            {
                prefabGroup.transform.SetParent(haskleeGroup.transform);
            }
        }

        ReadOptions(reader);

        var meshCount = reader.ReadInt32();
        for (int i=0; i<meshCount; i++)
        {
            var meshRef = MeshRef(ReadInMesh(reader));
            meshRefs.Add(meshRef);
            if (meshRefGroup != null)
            {
                meshRef.transform.SetParent(meshRefGroup.transform);

            }
        }

        var codeCount = reader.ReadInt32();
        for (int i=0; i<codeCount; i++)
        {
            funRefs.Add(Lua.DoFunctionString(ReadInString(reader)));
        }

        var textureCount = reader.ReadInt32();
        for (int i=0; i<textureCount; i++)
        {
            textures.Add(ReadInTexture(reader));
        }

        var materialCount = reader.ReadInt32();
        for (int i=0; i<materialCount; i++)
        {
            materials.Add(ReadInMaterial(reader));
        }

        var pathCount = reader.ReadInt32();
        for (int i=0; i<pathCount; i++)
        {
            paths.Add(ReadInPath(reader));
        }

        var prefabCount = reader.ReadInt32();
        for (int i=0; i<prefabCount; i++)
        {
            var prefab = ReadPrefab(reader);
            prefabs.Add(prefab);
            if (prefabGroup != null)
            {
                prefab.transform.SetParent(prefabGroup.transform);

            }
        }

        var objectCount = reader.ReadInt32();
        for (int i=0; i<objectCount; i++)
        {
            var obj = ReadObject(reader, true);
            AddRootGameObject(obj);
        }

        foreach (SetupA act in setupActions)
        {
            act.Setup();
        }
        setupActions.Clear();

#if HASKLEE_SHADERS
        ProcMan.Instance.Init();
#endif
    }

    private static void AddRootGameObject(GameObject obj)
    {
        gameObjects.Add(obj);
        if (haskleeGroup != null)
        {
            obj.transform.SetParent(haskleeGroup.transform);
        }
    }

    private static GameObject Instantiate(GameObject prefab)
    {
        var go = UnityEngine.Object.Instantiate(prefab);
        return go;
    }

    private static void ReadOptions(BinaryReader reader)
    {
        readNormals = reader.ReadBoolean();
        readUVs = reader.ReadBoolean();
        readColors = reader.ReadBoolean();
    }

    private static GameObject ReadPrefab(BinaryReader reader)
    {
        var go = ReadObject(reader, false);
        go.SetActive(false);
        return go;
    }

    private static GameObject ReadObject(BinaryReader reader, bool active)
    {
        int nodeCount = reader.ReadInt32();
        List<GameObject> gameObjects = new List<GameObject>(nodeCount);
        for (int i=0; i<nodeCount; i++)
        {
            gameObjects.Add(ReadObjectN(reader, active));
        }

        gameObjects[0].SetActive(false);

        int parent, child;
        int relCount = reader.ReadInt32();
        for (int i=0; i<relCount; i++)
        {
            parent = reader.ReadInt32();
            child = reader.ReadInt32();
            gameObjects[child].transform.SetParent(gameObjects[parent].transform, false);
            gameObjects[child].SetActive(true);

        }
        gameObjects[0].SetActive(active);

        return gameObjects[0];
    }

    private static GameObject ReadObjectN(BinaryReader reader, bool active)
    {
        NodeT type = (NodeT)reader.ReadByte();
        if (type == NodeT.DummyObj)
        {
            var go = new GameObject();
            go.SetActive(false);
            ReadInAttributes(go, reader, active);
            return go;
        }
        else if (type == NodeT.MeshObj)
        {
            Mesh mesh = ReadInMesh(reader);
            var go = SimpleGO(mesh);
            go.SetActive(false);
            ReadInAttributes(go, reader, active);
            return go;
        }
        else if (type == NodeT.MeshRefObj)
        {
            var meshRef = reader.ReadInt16();
            var go = Instantiate(meshRefs[meshRef]);
            go.SetActive(false);
            ReadInAttributes(go, reader, active);
            return go;
        }
        else if (type == NodeT.InstanceObj)
        {
            int datal;
            var i = reader.ReadInt16();

            var prefab = prefabs[i];

            if (active == true)
            {
                if (prefab != null)
                {
                    var go = Instantiate(prefab);
                    go.SetActive(false);
                    datal = reader.ReadInt32();

                    ReadInstanceAttributes(go, reader);
                    return go;
                }
                else
                {
                    var go = new GameObject();
                    return go;
                }
            }
            else
            {
                var go = new GameObject();
                go.SetActive(false);

                var instan = go.AddComponent<Instan>();
                instan.prefabRef = i;
                datal = reader.ReadInt32();

                //is this DATA even used? where is merge?
                instan.attribs = new byte[datal];
                reader.Read(instan.attribs, 0, datal);
                return go;
            }
        }
        else
        {
            var go = new GameObject();
            return go;
        }
    }

    private static void ReadInstanceAttributes(GameObject go, BinaryReader reader)
    {
        int atrCount = reader.ReadInt16();
        if (atrCount > 0)
        {
            ReadInIAttributes(atrCount, go, reader);
        }
    }

    private static void ReadInIAttributes(int n, GameObject gameObject, BinaryReader reader)
    {
        List<int> searchingFor = null;
        List<int> weAreHere = new List<int>();

        weAreHere.Add(0);
        Transform currentNode = gameObject.transform;
        for(int i=0; i<n; i++)
        {
            ReadInTreeKey(ref searchingFor, reader);

            int commonRooti = 1;
            while (weAreHere.Count > commonRooti &&
                   searchingFor.Count > commonRooti &&
                   weAreHere[commonRooti] == searchingFor[commonRooti])
            {
                commonRooti = commonRooti + 1;
            }

            while (commonRooti < weAreHere.Count)
            {
                currentNode = currentNode.parent;
                weAreHere.RemoveAt(weAreHere.Count - 1);
            }

            while (searchingFor.Count > weAreHere.Count)
            {
                currentNode = currentNode.GetChild(searchingFor[weAreHere.Count]);
                weAreHere.Add(searchingFor[weAreHere.Count]);
            }

            if (weAreHere[weAreHere.Count - 1] == searchingFor[searchingFor.Count - 1])
            {
                //FOUND
                GameObject go = currentNode.gameObject;
                var nestedInstance = reader.ReadBoolean();
                if (nestedInstance == true)
                {
                    var instan = go.GetComponent<Instan>();
                    var chil = Instantiate(prefabs[instan.prefabRef]);

                    var datal =  reader.ReadInt32();
                    var bb = new byte[datal];
                    reader.Read(bb, 0, datal);

                    chil.SetActive(false);
                    ReadInstanceAttributes(chil, new BinaryReader(new MemoryStream(bb)));

                    if (go.transform.parent != null)
                    {
                        chil.transform.SetParent(go.transform.parent, false);
                    }
                    else
                    {
                        //what are the other situations this could happen and be an actual error?
                        // Debug.LogWarning("Hasklee: Lost child.");

                        AddRootGameObject(chil);
                    }
                    chil.SetActive(true);
                    foreach (Transform child in go.transform)
                    {
                        child.SetParent(chil.transform, false);
                    }
                    //destroy in not immediate
                    UnityEngine.Object.Destroy(go);
                    currentNode = chil.transform;
                }
                else
                {
                    //?? is true correct here?
                    ReadInAttributes(go, reader, true);
                }
                continue;
            }
            else
            {
                Debug.LogError("Hasklee: Invalid prefab tree key.");
            }
        }

    }

    private static void ReadInTreeKey(ref List<int> searchingFor, BinaryReader reader)
    {
        var kCount = reader.ReadInt16();
        searchingFor = new List<int>();
        searchingFor.Add(0);
        for (int i=0; i<kCount; i++)
        {
            searchingFor.Add(reader.ReadInt16());
        }
    }

    private static Mesh ReadInMesh(BinaryReader reader)
    {
        int vertexCount = reader.ReadInt32();
        int faceCount = reader.ReadInt32();
        int indexCount = faceCount*3;

        var vertices = new Vector3[vertexCount];
        var indices = new int[indexCount];

        Vector3[] normals = null;
        Vector2[] uvs = null;
        Color[] colors = null;

        if (readNormals == true)
        {
            normals = new Vector3[vertexCount];
        }
        if (readUVs == true)
        {
            uvs = new Vector2[vertexCount];
        }
        if (readColors == true)
        {
            colors = new Color[vertexCount];
        }

        for (int i=0; i<vertexCount; i++) {
            vertices[i] = new Vector3(reader.ReadSingle(), reader.ReadSingle(),
                                      reader.ReadSingle());
            if (readNormals == true)
            {
                normals[i] = new Vector3(reader.ReadSingle(), reader.ReadSingle(),
                                         reader.ReadSingle());
            }
            if (readUVs == true)
            {
                uvs[i] = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            }
            if (readColors == true)
            {
                colors[i] = new Color(reader.ReadSingle(), reader.ReadSingle(),
                                      reader.ReadSingle(), reader.ReadSingle());
            }
        }

        for (int i=0; i<indexCount; i++) {
            indices[i] = reader.ReadInt32();
        }

        Mesh mesh = new Mesh();
        if (vertexCount > 65535)
        {
            mesh.indexFormat = IndexFormat.UInt32;
        }
        mesh.vertices = vertices;
        mesh.triangles = indices;

        if (readNormals == true)
        {
            mesh.normals = normals;
        }
        if (readUVs == true)
        {
            mesh.uv = uvs;
        }
        if (readColors == true)
        {
            mesh.colors = colors;
        }

        return mesh;
    }

    private static void ReadInTransform(Transform t, BinaryReader reader)
    {
        var type = reader.ReadByte();
        if (type == 0)
        {
            t.SetPositionAndRotation(ReadInVector3(reader), ReadInQuaternion(reader));
            t.localScale = ReadInVector3(reader);
        }
        else if (type == 1)
        {
            var position = ReadInVector3(reader);
            var forward = ReadInVector3(reader);
            var up = ReadInVector3(reader);
            var rotation = Quaternion.LookRotation(forward, up);
            t.SetPositionAndRotation(position, rotation);
            t.localScale = ReadInVector3(reader);
        }
    }

    private static void ReadInTransform(out Vector3 position, out Quaternion rotation,
                                        out Vector3 scale, BinaryReader reader)
    {
        var type = reader.ReadByte();
        if (type == 0)
        {
            position = ReadInVector3(reader);
            rotation = ReadInQuaternion(reader);
            scale = ReadInVector3(reader);
        }
        else if (type == 1)
        {
            position = ReadInVector3(reader);
            var forward = ReadInVector3(reader);
            var up = ReadInVector3(reader);
            rotation = Quaternion.LookRotation(forward, up);
            scale = ReadInVector3(reader);
        }
        else
        {
            position = new Vector3(0,0,0);
            rotation = Quaternion.identity;
            scale = new Vector3(1,1,1);
        }
    }

    private static void ReadInTransformV(out TransformV transformV, BinaryReader reader)
    {
        ReadInTransform(out transformV.position, out transformV.rotation,
                        out transformV.scale, reader);
        transformV.time = reader.ReadSingle();
    }

    private static List<TransformV> ReadInPath(BinaryReader reader)
    {
        int stepsCount = reader.ReadInt32();
        List<TransformV> path = new List<TransformV>();
        TransformV trv;

        for (int i=0; i<stepsCount; i++)
        {
            ReadInTransformV(out trv, reader);
            path.Add(trv);
        }

        return path;
    }

    private static TweenOpts ReadTweenOpts(out TweenOpts opts, BinaryReader reader)
    {
        opts.relative = reader.ReadBoolean();
        opts.autoplay = reader.ReadBoolean();
        opts.goTo = reader.ReadSingle();
        opts.loops = reader.ReadInt32();
        opts.loopType = reader.ReadInt16();

        return opts;
    }

    private static Rigidbody AddRigidBody(GameObject go)
    {
        var rb = go.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = go.AddComponent<Rigidbody>();
        }

        var collider = go.GetComponent<MeshCollider>();
        if (collider != null)
        {
            collider.convex =  true;
        }

        return rb;
    }

    private static void ReadInComponent(GameObject go, BinaryReader reader)
    {
        ComponentT ctype = (ComponentT)reader.ReadInt16();
        switch (ctype)
        {
            case ComponentT.ParamC:
                break;
            case ComponentT.Drag:
                go.AddComponent<Drag>().sensitivity = reader.ReadSingle();
                break;
            case ComponentT.Bell:
                go.AddComponent<Bell>();
                break;
            case ComponentT.StringPluck:
                go.AddComponent<StringPluck>();
                break;
            case ComponentT.Button:
                go.AddComponent<Button>();
                break;
            case ComponentT.Key:
                go.AddComponent<Key>();
                break;
            case ComponentT.Click:
                go.AddComponent<Click>();
                break;
            case ComponentT.AntennaUnfold:
                go.AddComponent<AntennaUnfold>().sensitivity = reader.ReadSingle();
                break;
            case ComponentT.Stop:
                go.AddComponent<Stop>();
                break;
            case ComponentT.Rigidbody:
                var rb = AddRigidBody(go);
                rb.isKinematic = reader.ReadBoolean();
                rb.useGravity = reader.ReadBoolean();
                rb.drag = 0;
                rb.mass = reader.ReadSingle();
                break;
            case ComponentT.ConfigurableJointN:
                ReadInCJoint(go, reader);
                break;
            case ComponentT.FixedJointN:
                var joint = go.AddComponent<FixedJoint>();
                setupActions.Add(new JointSetup(joint, reader.ReadInt32()));
                break;
            case ComponentT.Slider:
                var slider = go.AddComponent<Slider>();
                slider.initp = reader.ReadSingle();
                slider.sensitivity = reader.ReadSingle();
                break;
            case ComponentT.Rotator:
                var rotf = go.AddComponent<Rotator>();
                rotf.point = ReadInVector3(reader);
                rotf.axis = ReadInVector3(reader);
                rotf.minr = reader.ReadSingle();
                rotf.maxr = reader.ReadSingle();
                rotf.sendAction = reader.ReadBoolean();
                break;
            case ComponentT.Translator:
                var traf = go.AddComponent<Translator>();
                traf.axis = ReadInVector3(reader);
                traf.minr = reader.ReadSingle();
                traf.maxr = reader.ReadSingle();
                traf.sendAction = reader.ReadBoolean();
                break;
            case ComponentT.LuaComponent:
                string name = ReadInString(reader);
                string code = ReadInString(reader);
                Table lt = Lua.SafeDoString("return { " + code + " }").Table;
                break;
            case ComponentT.CustomComponent:
                string tp = ReadInString(reader);
                string json = ReadInString(reader);
                Type type = Hasklee.Instance.GetComponentType(tp);
                if (type != null)
                {
                    JsonUtility.FromJsonOverwrite(json, go.AddComponent(type));
                }
                break;
#if HASKLEE_EXTRA_COMPONENTS
            case ComponentT.IdGraphRC:
                ReadInIdGraph(go, reader, true);
                break;
            case ComponentT.GraphConductor:
                var gcon = go.AddComponent<GraphConductor>();
                gcon.n = reader.ReadInt32();
                break;
            case ComponentT.GraphPropagate:
                var first = reader.ReadInt32();
                var flap = go.AddComponent<GraphPropagateC>();
                flap.first = first;
                flap.type = reader.ReadInt16();
                break;
            case ComponentT.CameraNode:
                // FIX THIS DANGLING GO
                GameObject emptyGO = new GameObject();
                ReadInTransform(emptyGO.transform, reader);
                var camn = go.AddComponent<CameraNode>();
                camn.dest = emptyGO.transform;
                break;
            case ComponentT.Proximity:
                var proximity = go.AddComponent<Proximity>();
                proximity.targetID = reader.ReadInt32();
                proximity.offset = ReadInVector3(reader);
                proximity.tolerance = reader.ReadSingle();
                break;
#endif
        }
    }

    private static void ReadInComponents(GameObject go, BinaryReader reader)
    {
        int compCount = reader.ReadInt32();
        for (int i=0; i<compCount; i++)
        {
            ReadInComponent(go, reader);
        }
    }

    private static void ReadInAttributes(GameObject go, BinaryReader reader, bool active)
    {
        short atrCount = reader.ReadInt16();
        if (atrCount <= 0)
        {
            return;
        }

        Dictionary<string, int> nPaths = null;
        AttributeT type;

        for (int i=0; i<atrCount; i++)
        {
            type = (AttributeT)reader.ReadInt16();
            switch (type)
            {
                case AttributeT.NoAtr:
                    break;
                case AttributeT.TransformAtr:
                    ReadInTransform(go.transform, reader);
                    break;
                case AttributeT.ComponentAtr:
                    ReadInComponents(go, reader);
                    break;
                case AttributeT.VectorAtr:
                    LuaController.AddGetLuaController(go).lt[ReadInString(reader)] =
                        ReadInVector3(reader);
                    break;
                case AttributeT.ScalarAtr:
                    LuaController.AddGetLuaController(go).lt[ReadInString(reader)] =
                        reader.ReadSingle();
                    break;
                case AttributeT.ColourAtr:
                case AttributeT.AlphaColourAtr:
                    go.AddGetComponent<HController>().color = ReadColor(reader);
                    break;
                case AttributeT.SpecularAtr:
                    go.AddGetComponent<HController>().specular = ReadColor(reader);
                    break;
                case AttributeT.DanceAtr:
                {
                    if (nPaths == null)
                    {
                        nPaths = new Dictionary<string, int>(StringComparer.Ordinal);
                        namedPaths.Add(nPaths);
                    }
                    string name = ReadInString(reader);
                    paths.Add(ReadInPath(reader));
                    nPaths.Add(name, paths.Count - 1);
                    var pr = (namedPaths.Count - 1);
                    go.AddGetComponent<AnimController>().pathRef = pr;
                    if (!sharedTweenOpts.ContainsKey(pr))
                    {
                        sharedTweenOpts.Add(pr, new Dictionary<string, TweenOpts>
                                            (StringComparer.Ordinal));
                    }
                    TweenOpts opts;
                    ReadTweenOpts(out opts, reader);
                    sharedTweenOpts[pr][name] = opts;
                    break;
                }
                case AttributeT.DanceInstance:
                {
                    string name = ReadInString(reader);
                    TweenOpts opts2;
                    ReadTweenOpts(out opts2, reader);
                    go.AddGetComponent<AnimController>().AddTweenOpts(name, opts2);
                    break;
                }
                case AttributeT.PathAtr:
                {
                    if (nPaths == null)
                    {
                        nPaths = new Dictionary<string, int>(StringComparer.Ordinal);
                        namedPaths.Add(nPaths);
                    }
                    string name = ReadInString(reader);
                    paths.Add(ReadInPath(reader));
                    nPaths.Add(name, paths.Count - 1);
                    go.AddGetComponent<AnimController>().pathRef = (namedPaths.Count - 1);
                    break;
                }
                case AttributeT.PathRefAtr:
                {
                    if (nPaths == null)
                    {
                        nPaths = new Dictionary<string, int>(StringComparer.Ordinal);
                        namedPaths.Add(nPaths);
                    }
                    string name = ReadInString(reader);
                    var ppp = reader.ReadInt32();
                    nPaths.Add(name, ppp);
                    go.AddGetComponent<AnimController>().pathRef = (namedPaths.Count - 1);
                    break;
                }
                case AttributeT.LightAtr:
                    ReadInLight(go, reader);
                    break;
                case AttributeT.RealIDAtr:
                {
                    int id = reader.ReadInt32();
                    go.AddGetComponent<HController>().ID = id;
                    Hasklee.Instance.AddId(id, go);
                    break;
                }
                case AttributeT.RealIDTAtr:
                    var idc = reader.ReadInt32();
                    var idt = go.AddGetComponent<IdT>();
                    idt.ids = new int[idc];
                    for (int j=0; j<idc; j++)
                    {
                        idt.ids[j] = reader.ReadInt32();
                    }
                    break;
                case AttributeT.IgnoreRayCastAtr:
                    go.layer = 2;
                    break;
                case AttributeT.ColliderAtr:
                {
                        bool readUVs2 = readUVs;
                        bool readNormals2 = readNormals;
                        bool readColors2 = readColors;
                        readUVs = false;
                        readNormals = false;
                        readColors = false;
                        var cmesh = ReadInMesh(reader);
                        readUVs = readUVs2;
                        readNormals = readNormals2;
                        readColors = readColors2;

                        var collider = go.AddComponent<MeshCollider>();
                        collider.sharedMesh = cmesh;
                        go.AddComponent<Colada>();
                        break;
                }
                case AttributeT.ColliderConvexAtr:
                {
                        var collider = go.GetComponent<MeshCollider>();
                        if (collider != null)
                        {
                            collider.convex = true;
                        }
                        break;
                }
                case AttributeT.ColliderDisableAtr:
                {
                        var collider = go.GetComponent<MeshCollider>();
                        if (collider != null)
                        {
                            collider.enabled = false;
                        }
                        break;
                }
                case AttributeT.CsoundInline:
                {
                    string code = ReadInString(reader);
                    if (active)
                    {
                    CsoundUnity csound = go.AddComponent<CsoundUnity>();
                    csound.csoundString = code;
                    //would have to do some kind of csound table (de)serialize
                    //in order to get rid of the active requirement
                    Lua.AddCsound(LuaController.AddGetLuaController(go).lt);
                    }
                    break;
                }
                case AttributeT.MaterialAtr:
                {
                        var matt = reader.ReadByte();

                        //create new material
                        if (matt == 1)
                        {
                            var material = ReadInMaterial(reader);
                            if (material != null)
                            {
                                materials.Add(material);
                                AddGetMeshRenderer(go).material = material;
                            }
                        }
                        //reference to created material
                        else if (matt == 0)
                        {
                            var mi = reader.ReadInt16();
                            if (mi >= 0 && mi < materials.Count)
                            {
                                AddGetMeshRenderer(go).material = materials[mi];
                            }
                            // renderer.material = materials[reader.ReadInt16()];
                        }
                        //material by name
                        else if (matt == 2)
                        {
                            string materialName = ReadInString(reader);
#if HASKLEE_SHADERS
                            if (materialName == "Proc")
                            {
                                var comp = go.AddComponent<GeoMM>();
                                comp.procShaderI = 0;
                            }
                            else if (materialName == "ProcTrans")
                            {
                                var comp = go.AddComponent<GeoMM>();
                                comp.procShaderI = 2;
                            }
                            else if (materialName == "ProcString")
                            {
                                var comp = go.AddComponent<GeoMMS>();
                                comp.procShaderI = 3;
                            }
#else
                            if (materialName == "Proc" ||
                                materialName == "ProcTrans" ||
                                materialName == "ProcString")
                            {
                                Material material = Hasklee.Instance.defaultMaterial;
                                if (material != null)
                                {
                                    AddGetMeshRenderer(go).material = material;
                                }
                                AddGetMeshRenderer(go).material = material;
                            }
#endif
                            //material from resources pool
                            else
                            {
                                Material material;
                                MaterialByName(materialName, out material);
                                if (material == null)
                                {
                                    material = Hasklee.Instance.defaultMaterial;
                                }
                                if (material != null)
                                {
                                    AddGetMeshRenderer(go).material = material;
                                }
                            }
                        }
                        break;
                }
                case AttributeT.ActionRRCodeString:
                {
                    int id = reader.ReadInt32();
                    string code = ReadInString(reader);
                    if (active == true)
                    {
                        go.AddGetComponent<HController>().ActionR(id, code);
                        LuaController.AddGetLuaController(go);
                    }
                    break;
                }
                case AttributeT.ActionRRCodeRef:
                {
                    int id = reader.ReadInt32();
                    int codeRef = reader.ReadInt16();
                    if (active == true)
                    {
                        go.AddGetComponent<HController>().ActionR(id, funRefs[codeRef]);
                        LuaController.AddGetLuaController(go);
                    }
                    break;
                }
                case AttributeT.ActionSSCodeString:
                {
                    int id = reader.ReadInt32();
                    string code = ReadInString(reader);
                    if (active == true)
                    {
                        go.AddGetComponent<HController>().ActionS(id, code);
                        LuaController.AddGetLuaController(go);
                        setupActions.Add(new LuaControllerSetup(id));
                    }
                    break;
                }
                case AttributeT.ActionSSCodeRef:
                {
                    int id = reader.ReadInt32();
                    int codeRef = reader.ReadInt16();
                    if (active == true)
                    {
                        go.AddGetComponent<HController>().ActionS(id, funRefs[codeRef]);
                        LuaController.AddGetLuaController(go);
                        setupActions.Add(new LuaControllerSetup(id));
                    }
                    break;
                }
                case AttributeT.LuaCode:
                {
                    string name = ReadInString(reader);
                    string code = ReadInString(reader);
                    go.AddGetComponent<HController>();
                    if (active == true)
                    {
                        var lc = LuaController.AddGetLuaController(go);
                        lc.lt[name] = Lua.DoFunctionString(code);
                    }
                    break;
                }
                case AttributeT.LuaController:
                    LuaController.AddGetLuaController(go);
                    break;
                case AttributeT.NameAtr:
                    go.name = ReadInString(reader);
                    break;
                case AttributeT.TagAtr:
                    ReadInString(reader);
                    break;
            }
        }
    }

    private static ConfigurableJointMotion ReadInCJointMType(BinaryReader reader)
    {
        return (ConfigurableJointMotion)(Convert.ToInt32(reader.ReadInt16()));
    }

    private static void ReadInCJoint(GameObject go, BinaryReader reader)
    {
        var cj = go.AddComponent<ConfigurableJoint>();
        cj.projectionMode = JointProjectionMode.PositionAndRotation;

        var id = reader.ReadInt32();

        var jAnchor = reader.ReadBoolean();
        if (jAnchor == true )
        {
            cj.anchor = ReadInVector3(reader);
        }

        var jAxis = reader.ReadBoolean();
        if (jAxis == true )
        {
            cj.axis = ReadInVector3(reader);
        }

        var lMotion = reader.ReadBoolean();
        if (lMotion == true )
        {
            cj.xMotion = ReadInCJointMType(reader);
            cj.yMotion = ReadInCJointMType(reader);
            cj.zMotion = ReadInCJointMType(reader);
        }
        var lLimit = reader.ReadBoolean();
        if (lLimit == true )
        {
        }
        var lSpring = reader.ReadBoolean();
        if (lSpring == true )
        {
        }
        var aMotion = reader.ReadBoolean();
        if (aMotion == true )
        {
            cj.angularXMotion = ReadInCJointMType(reader);
            cj.angularYMotion = ReadInCJointMType(reader);
            cj.angularZMotion = ReadInCJointMType(reader);
        }
        var  aLimitXH = reader.ReadBoolean();
        if (aLimitXH == true )
        {
            var lim = cj.highAngularXLimit;
            lim.limit = reader.ReadSingle();
            lim.bounciness = reader.ReadSingle();
            lim.contactDistance = reader.ReadSingle();
        }
        var  aLimitXL = reader.ReadBoolean();
        if (aLimitXL == true )
        {
            var lim = cj.lowAngularXLimit;
            lim.limit = reader.ReadSingle();
            lim.bounciness = reader.ReadSingle();
            lim.contactDistance = reader.ReadSingle();
        }

        setupActions.Add(new JointSetup(cj, id));
    }

    private static void ReadInLight(GameObject go, BinaryReader reader)
    {
        var ltype = reader.ReadInt16();
        if (ltype == 3)
        {
#if HASKLEE_SHADERS
            var light = go.AddComponent<LightMM>();
            light.procShaderI = 1;

            light.range = reader.ReadSingle();
            light.intensity = reader.ReadSingle();
            light.spotAngle = reader.ReadSingle();
            light.color = ReadColor(reader);
#endif
        }
        else
        {
            var light = go.AddComponent<Light>();
            switch (ltype)
            {
                case 0:
                    light.type = LightType.Spot;
                    break;
                case 1:
                    light.type = LightType.Point;
                    break;
                case 2:
                    light.type = LightType.Directional;
                    break;
            }
            light.range = reader.ReadSingle();
            light.intensity = reader.ReadSingle();
            light.spotAngle = reader.ReadSingle();
            light.color = ReadColor(reader);
        }
    }

    private static string ReadInString(BinaryReader reader)
    {
        var count = reader.ReadInt32();
        char[] chars = new char[count];
        reader.Read(chars, 0, count);
        return new string(chars);
    }

    private static Vector3 ReadInVector3(BinaryReader reader)
    {
        return new Vector3(reader.ReadSingle(),
                           reader.ReadSingle(),
                           reader.ReadSingle());
    }

    private static Quaternion ReadInQuaternion(BinaryReader reader)
    {
        return new Quaternion(reader.ReadSingle(), reader.ReadSingle(),
                              reader.ReadSingle(), reader.ReadSingle());
    }

    private static Color ReadColor(BinaryReader reader)
    {
        return new Color(reader.ReadSingle(), reader.ReadSingle(),
                         reader.ReadSingle(), reader.ReadSingle());
    }

#if HASKLEE_EXTRA_COMPONENTS
    private static void ReadInIdGraph(GameObject go, BinaryReader reader, bool twoway)
    {
        // IntGraph graph = ReadIntGraph(reader, twoway);
        IntGraphV graph = ReadIntGraphVN(reader);
        var idGraph = go.AddComponent<IdGraphR>();
        idGraph.graph = graph;
    }

    private static IntGraph ReadIntGraph(BinaryReader reader, bool twoway)
    {
        IntGraph graph = new IntGraph();

        int from, to;
        int adjCount;
        int nodeCount = reader.ReadInt32();
        for (int i=0; i<nodeCount; i++)
        {
            from = reader.ReadInt32();
            graph.SafeAddNode(from);

            adjCount = reader.ReadInt32();
            for (int j=0; j<adjCount; j++)
            {
                to = reader.ReadInt32();
                if (twoway)
                {
                    graph.AddEdge(from, to);
                    graph.SafeAddEdge(to, from);
                }
                else
                {
                    graph.AddEdge(from, to);
                }
            }
        }

        return graph;
    }

    private static IntGraphV ReadIntGraphVN(BinaryReader reader)
    {
        IntGraphV graph = new IntGraphV();

        int from, to;
        int adjCount;
        int nodeCount = reader.ReadInt32();
        for (int i=0; i<nodeCount; i++)
        {
            from = reader.ReadInt32();
            graph.SafeAddNode(from);

            adjCount = reader.ReadInt32();
            for (int j=0; j<adjCount; j++)
            {
                to = reader.ReadInt32();
                graph.AddEdge(from, to, 1);
                graph.SafeAddEdge(to, from, -1);
            }
        }

        return graph;
    }
#endif

    private static Material ReadInMaterial(BinaryReader reader)
    {
        string shaderName = ReadInString(reader);
        var material = new Material(Shader.Find(shaderName));

        var hasTexture = reader.ReadBoolean();
        if (hasTexture == true)
        {
            Texture2D texture = ReadInTexture(reader);
            //this is not the way
            material.mainTexture = texture;
        }

        material.color = ReadColor(reader);
        material.SetFloat("_Glossiness", reader.ReadSingle());
        material.SetFloat("_Metallic", reader.ReadSingle());

        return material;
    }

    private static Texture2D ReadInTexture(BinaryReader reader)
    {
        var type = reader.ReadByte();
        switch (type)
        {
            case 0:
                {
                    var datal = reader.ReadInt32();
                    byte[] data = new byte[datal];
                    reader.Read(data, 0, datal);
                    Texture2D tex = new Texture2D(2, 2);
                    var ress = tex.LoadImage(data);
                    return tex;
                }
            case 1:
                {
                    var ti = reader.ReadInt32();
                    return textures[ti];
                }
            case 2:
                {
                    var fileName = ReadInString(reader);
                    byte[] data = System.IO.File.ReadAllBytes(fileName);
                    Texture2D tex = new Texture2D(2, 2);
                    var ress = tex.LoadImage(data);
                    return tex;
                }
        }
        return null;
    }

    private static GameObject SimpleGO(Mesh mesh)
    {
        GameObject newObject = new GameObject();

        if (readNormals == false)
        {
            mesh.RecalculateNormals();
        }
        mesh.RecalculateTangents();

        var meshFilter = newObject.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh;

        var renderer = newObject.AddComponent<MeshRenderer>();
        renderer.enabled = true;
        renderer.material = Hasklee.Instance.defaultMaterial;

        var collider = newObject.AddComponent<MeshCollider>();

        return newObject;
    }

    private static GameObject MeshRef(Mesh mesh)
    {
        GameObject newObject = SimpleGO(mesh);
        newObject.SetActive(false);
        newObject.name = "MeshRef";
        return newObject;
    }

    private static MeshRenderer AddGetMeshRenderer(GameObject go)
    {
        var renderer = go.AddGetComponent<MeshRenderer>();
        renderer.enabled = true;
        renderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
        return renderer;
    }

    private static bool MaterialByName(string materialName, out Material material)
    {
        bool result = resourceMaterials.TryGetValue(materialName, out material);
        if (result == true)
        {
            if (material != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            material = (Material)Resources.Load(materialName, typeof(Material));
            if (material == null)
            {
                Debug.LogWarning("Unable to find material " + materialName);
                //add null to prevent more calls to Resources.Load
                resourceMaterials.Add(materialName, material);
                return false;
            }
            else
            {
                resourceMaterials.Add(materialName, material);
                return true;
            }
        }
    }

    class LuaControllerSetup : SetupA
    {
        private int id;

        public LuaControllerSetup(int id)
        {
            this.id = id;
        }

        public void Setup()
        {
            var go = Hasklee.Instance.GameObjectFromId(id);
            if (go != null)
            {
                LuaController.AddGetLuaController(go);
            }
        }
    }

    class JointSetup : SetupA
    {
        private Joint joint;
        private int id;

        public JointSetup(Joint joint, int id)
        {
            this.joint = joint;
            this.id = id;
        }

        public void Setup()
        {
            var go = Hasklee.Instance.GameObjectFromId(id);
            if (go != null)
            {
                var rb = go.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    joint.connectedBody = rb;
                }
            }
        }
    }
}

[Serializable]
public struct TransformV
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public float time;

    public static float PathLength(List<TransformV> path)
    {
        float l = 0f;
        for (int i=0; i<(path.Count-1); i++)
        {
            l += (path[i+1].position - path[i].position).magnitude;
        }
        return l;
    }
}

}
