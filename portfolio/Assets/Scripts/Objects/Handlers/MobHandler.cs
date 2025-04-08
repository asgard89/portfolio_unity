using System;
using UnityEngine;

public class MobHandler : ObjectHandler
{
    public override void Start()
    {
        SetUp();
        base.Start();
        Debug.LogError($"## Start MobHandler");
    }

    public override void OnDisable()
    {
        Debug.LogError($"## OnDisable MobHandler");
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
        Debug.LogError($"## initialize MobHandler");
    }

    protected override void release(object _o, EventArgs _args)
    {
        base.release(_o, _args);
        Debug.LogError($"## release MobHandler");
    }

    public override void SetHandleTarget(BaseObject _targetObject)
    {
        base.SetHandleTarget(_targetObject);
    }
}
