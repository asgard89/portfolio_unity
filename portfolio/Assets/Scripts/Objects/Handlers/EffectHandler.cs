using UnityEngine;
using System;

public class EffectHandler : ObjectHandler
{
    public override void Start()
    {
        SetUp();
        base.Start();
        Debug.LogError($"## Start ObjectHandler");
    }

    public override void OnDisable()
    {
        Debug.LogError($"## OnDisable ObjectHandler");
        base.OnDisable();

    }
    protected override void SetUp()
    {
        base.SetUp();
        OnInitialize += initialize;
        OnRelease += release;
    }

    protected override void initialize(object _o, EventArgs _args)
    {
        base.initialize(_o, _args);
        Debug.LogError($"## initialize ObjectHandler");
    }

    protected override void release(object _o, EventArgs _args)
    {
        base.release(_o, _args);
        Debug.LogError($"## release ObjectHandler");
    }

    public override void SetHandleTarget(BaseObject _targetObject)
    {
        base.SetHandleTarget(_targetObject);
    }
}
