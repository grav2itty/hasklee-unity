using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hasklee {

public struct TweenOpts
{
    public bool relative;
    public bool autoplay;
    public float goTo;
    public int loops;
    public int loopType;
}

public class AnimController : MonoBehaviour
{
    public int pathRef;

    private Dictionary<string, Tween> tweens =
        new Dictionary<string, Tween>(StringComparer.Ordinal);
    private Dictionary<string, TweenOpts> tweenOpts;

    private ShaderTransformUp shaderTransformUp;


    public static Tween TweenFromPath(Transform transform, List<TransformV> path)
    {
        Sequence mySequence = DOTween.Sequence();

        foreach (var transformV in path)
        {
            mySequence.Append(transform.DOLocalMove(transformV.position, transformV.time));
        }

        float time = 0;
        foreach (var transformV in path)
        {
            // mySequence.Insert(time, transform.DOLocalRotate(kvp.Value.rotation.eulerAngles, transformV.time));
            mySequence.Insert(time, transform.DOLocalRotateQuaternion(transformV.rotation, transformV.time));
            time += transformV.time;
        }

        // time = 0;
        // foreach (var transformV in path)
        // {
        //     mySequence.Insert(time, transform.DOScale(transformV.scale, transformV.time));
        //     time += transformV.time;
        // }

        return mySequence;
        // return mySequence.SetEase(Ease.InExpo);
    }

    public void AddTweenOpts(string s, TweenOpts tO)
    {
        if (tweenOpts == null)
        {
            tweenOpts = new Dictionary<string, TweenOpts>(StringComparer.Ordinal);
        }
        tweenOpts.Add(s, tO);
    }

    public List<TransformV> GetPath(string s)
    {
        return Init.GetPath(pathRef, s);
    }

    public void PlayTween(string key)
    {
        PlayTween(TweenFromKey(key));
    }

    public Tween TweenFromKey(string key)
    {
        Tween tween;
        tweens.TryGetValue(key, out tween);
        if (tween != null)
        {
            return tween;
        }
        else
        {
            var path = GetPath(key);
            if (path != null)
            {
                tween = TweenFromPath(transform, path);
                tweens[key] = tween;
                ShaderLink(tween);
            }
            return tween;
        }
    }

    void OnDestroy()
    {
        foreach (KeyValuePair<string, Tween> kvp in tweens)
        {
            kvp.Value.Kill();
        }
    }

    void Start()
    {
        var sharedTweenOpts = Init.GetTweenOpts(pathRef);
        if (sharedTweenOpts != null)
        {
            foreach (KeyValuePair<string, TweenOpts> kvp in sharedTweenOpts)
            {
                InitTween(kvp.Key);
            }
        }

        if (tweenOpts != null)
        {
            foreach (KeyValuePair<string, TweenOpts> kvp in tweenOpts)
            {
                InitTween(kvp.Key, kvp.Value);
            }
        }

        shaderTransformUp = gameObject.GetComponent<ShaderTransformUp>();
        if (shaderTransformUp != null)
        {
            foreach (KeyValuePair<string, Tween> kvp in tweens)
            {
                ShaderLink(kvp.Value);
            }
        }
    }

    private void InitTween(string key, TweenOpts opts)
    {
        Tween tween = TweenFromKey(key);
        if (opts.relative)
        {
            tween.SetRelative();
        }

        tween.Goto(opts.goTo);

        switch (opts.loopType)
        {
            case 0:
                tween.SetLoops(opts.loops, LoopType.Restart);
                break;
            case 1:
                tween.SetLoops(opts.loops, LoopType.Yoyo);
                break;
            case 2:
                break;
            case 3:
                tween.OnComplete(() => { tween.Flip(); })
                    .OnRewind(() => { tween.Flip(); });
                break;
            case 4:
                //this shoud only work with Tweeners according to documentation?
                tween.SetLoops(opts.loops, LoopType.Incremental);
                tween.SetRelative(true)
                    .OnStepComplete(() => { tween.Pause(); });
                break;
        }
        if (opts.autoplay)
        {
            tween.Play();
        }
    }

    private void InitTween(string key)
    {
        if (tweenOpts != null && tweenOpts.ContainsKey(key))
        {
            InitTween(key, tweenOpts[key]);
        }
        else if (Init.GetTweenOpts(pathRef) != null )
        {
            InitTween(key, Init.GetTweenOpts(pathRef)[key]);
        }
    }

    private void PlayTween(Tween tween)
    {
        if (tween != null)
        {
            tween.Play();
        }
    }

    private void ShaderLink(Tween tween)
    {
        if (shaderTransformUp != null)
        {
            tween.OnUpdate(
                () =>
                {
                    shaderTransformUp.ShaderTransformUp();
                }
            );
        }
    }
}

}
