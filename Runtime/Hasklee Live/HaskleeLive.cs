using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Hasklee {

public class HaskleeLive : MonoBehaviour
{
    public string ip;
    public bool anyip = true;
    public int port = 9734;

    private static Connector connector;

    void Awake()
    {
        connector = new Connector(ip, anyip, port);
    }

    void OnApplicationQuit()
    {
        connector.CleanUp();
    }

    void Update()
    {
        if (connector.FramesSemWait())
        {
            foreach (Frame frame in connector.Frames)
            {
                if (frame.binData.Length > 0)
                {
                    Hasklee.Instance.Destroy();
                    Hasklee.Instance.Load(new BinaryReader(new MemoryStream(frame.binData)));
                }
                if (frame.luaCommand.Length > 0)
                {
                    Lua.SafeDoString(frame.luaCommand);
                }
            }
            connector.CleanFrames();
            connector.FramesSemRelease();
        }
    }
}

}
