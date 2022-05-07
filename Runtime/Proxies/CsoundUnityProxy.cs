using MoonSharp.Interpreter;

#if UNITY_EDITOR || UNITY_STANDALONE
using MYFLT = System.Double;
#elif UNITY_ANDROID // and maybe iOS?
using MYFLT = System.Single;
#endif

namespace Hasklee {

public class CsoundUnityProxy
{
    CsoundUnity csound;

    [MoonSharpHidden]
    public CsoundUnityProxy(CsoundUnity csound)
    {
        this.csound = csound;
    }

    public MYFLT GetChannel(string channel)
    {
        return csound.GetChannel(channel);
    }

    public void SetChannel(string channel, MYFLT val)
    {
        csound.SetChannel(channel, val);
    }

    public void SendScoreEvent(string scoreEvent)
    {
        csound.SendScoreEvent(scoreEvent);
    }
}

}
