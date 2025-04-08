using System;
using ASGA.DS;
using System.Collections.Generic;

/// <summary>
/// EventHandler Arguments
/// </summary>
public class BaseEventArgs : EventArgs
{
    public BaseEventArgs() { }

    public virtual void SetData<T>(T _data) where T : BaseInfoData
    {

    }

    public virtual T GetData<T>() where T : BaseInfoData
    {
        return null;
    }
}

public class OnMoveActionArgs : BaseEventArgs
{
    public OnMoveActionArgs() { }
}

public class OnStatusUpdateArgs : BaseEventArgs
{
    public OnStatusUpdateArgs() { }
}

public class OnSkillActionArgs : BaseEventArgs
{
    SkillInfoData skillInfo;
    public OnSkillActionArgs() 
    {
        skillInfo = new();
    }
    public override void SetData<T>(T _data) 
    {
        skillInfo = _data as SkillInfoData;
    }

    public override T GetData<T>()
    {
        return skillInfo as T;
    }
}

public class OnLoadingArgs : BaseEventArgs
{
    public OnLoadingArgs() { }
}

public class OnInfinityGridInitArgs : BaseEventArgs
{
    List<UIBase> gridItemList;
    public OnInfinityGridInitArgs(List<UIBase> _gridItemList) 
    { 
        gridItemList = _gridItemList;
    }

    public List<UIBase> GetGridItemList()
    {
        return gridItemList;
    }
}
