using UnityEngine;

namespace Hasklee {

public class Rotator : MonoBehaviour, IPerformer<float>, ICValue<float>
{
    public Vector3 point;
    public Vector3 axis;
    public float minr;
    public float maxr;
    public bool sendAction;

    public float cvalue
    {
        get => r;
        set => r = value;
    }

    private float r = 0;
    private bool limited;
    private int targetID = 0;
    private ShaderTransformUp shaderTransformUp;

    public float Perform(float d)
    {
        float nr = r + d;
        float dd = d;
        if (limited == true)
        {
            if (dd > 0)
            {
                if (nr > maxr)
                {
                    dd = maxr - r;
                }
                // else if (nr == maxr)
                // {
                //     dd *= 0.99f;
                // }
            }
            else if (dd < 0)
            {
                if (nr < minr)
                {
                    dd = minr - r;
                }
                // else if (nr == minr)
                // {
                //     dd *= 0.99f;
                // }
            }
        }

        r += dd;
        Rotate(dd);

        if (sendAction == true)
        {
            Lua.Action(targetID, "rot", r);
        }

        return dd;
    }

    public void Rotate(float d)
    {
        var pointW = transform.TransformPoint(point);
        var axisW = transform.TransformVector(axis);
        transform.RotateAround(pointW, axisW, d);
        if (shaderTransformUp != null)
        {
            shaderTransformUp.ShaderTransformUp();
        }
    }

    void Start()
    {
        shaderTransformUp = gameObject.GetComponent<ShaderTransformUp>();
        if (sendAction == true)
        {
            targetID = gameObject.ID();
        }
        if (minr == 0.0f && maxr == 0.0f)
        {
            limited = false;
        }
        else
        {
            limited = true;
        }
    }
}


public class Translator : MonoBehaviour, IPerformer<float>, ICValue<float>
{
    public Vector3 axis;
    public float minr;
    public float maxr;
    public bool sendAction;

    public float cvalue
    {
        get => r;
        set => r = value;
    }

    private float r = 0;
    private bool limited;
    private int targetID = 0;
    private ShaderTransformUp shaderTransformUp;

    public float Perform(float d)
    {
        float nr = r + d;
        float dd = d;
        if (limited == true)
        {
            if (dd > 0)
            {
                if (nr > maxr)
                {
                    dd = maxr - r;
                }
            }
            else if (dd < 0)
            {
                if (nr < minr)
                {
                    dd = minr - r;
                }
            }
        }

        r += dd;
        Translate(dd);

        if (sendAction == true)
        {
            Lua.Action(targetID, "tran", r);
        }

        return dd;
    }

    void Translate(float d)
    {
        transform.Translate(axis * d);
        if (shaderTransformUp != null)
        {
            shaderTransformUp.ShaderTransformUp();
        }
    }

    void Start()
    {
        shaderTransformUp = gameObject.GetComponent<ShaderTransformUp>();
        if (sendAction == true)
        {
            targetID = gameObject.ID();
        }
        if (minr == 0.0f && maxr == 0.0f)
        {
            limited = false;
        }
        else
        {
            limited = true;
        }
    }
}

}
