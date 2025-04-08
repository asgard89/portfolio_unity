using UnityEngine;
using System;
using ASGA.DS;

public class PlayerHandler : ObjectHandler
{
    public override void Start()
    {
        SetUp();
        base.Start();
        Debug.LogError($"## Start PlayerHandler");
    }

    public override void OnDisable()
    {
        Debug.LogError($"## OnDisable PlayerHandler");
        base.OnDisable();

    }
    protected override void SetUp()
    {
        base.SetUp();
        OnInitialize += initialize;
        OnRelease += release;

        moveArgs = new OnMoveActionArgs();
        skillArgs = new OnSkillActionArgs();

        OnMoveActionEvent += onMoveAction;
        OnSkillActionEvent += onPlayerSkillAction;
    }

    protected override void initialize(object _o, EventArgs _args)
    {
        base.initialize(_o, _args);
        Debug.LogError($"## initialize PlayerHandler");
    }

    protected override void release(object _o, EventArgs _args)
    {
        base.release(_o, _args);
        Debug.LogError($"## release PlayerHandler");
        Destroy(this);
    }

    public override void SetHandleTarget(BaseObject _targetObject)
    {
        base.SetHandleTarget(_targetObject);
    }

    void onMoveAction(object _o, OnMoveActionArgs _args)
    {

    }

    void onPlayerSkillAction(object _o, OnSkillActionArgs _args)
    {
        SkillController.CastSkill(_args.GetData<SkillInfoData>());
    }

    public override void OnSkillAction(SkillInfoData _data)
    {
        skillArgs.SetData(_data);

        base.OnSkillAction(_data);
    }
}
