using MoonSharp.Interpreter;
using UnityEngine;

namespace Hasklee {

class LightProxy
{
    Light light;

    [MoonSharpHidden]
    public LightProxy(Light light)
    {
        this.light = light;
    }

    public Color color
    {
        get => light.color;
        set => light.color = value;
    }

    public bool enabled
    {
        get => light.enabled;
        set => light.enabled = value;
    }

    public float intensity
    {
        get => light.intensity;
        set => light.intensity = value;
    }

    public float range
    {
        get => light.range;
        set => light.range = value;
    }

    public float spotAngle
    {
        get => light.spotAngle;
        set => light.spotAngle = value;
    }
}

}
