using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using ASGA.DS;

/// <summary>
/// BaseObject를 제어하는 컴포넌트
/// Handler의 종류에 따라서 BaseObject를 다양한 형태로 사용
/// </summary>
public class ObjectHandler : MonoBehaviour, IInitializeHandler
{
    #region Derived Interface - IInitializeHandler

    public event EventHandler OnInitialize;
    public event EventHandler OnRelease;
    public event EventHandler OnRefresh;

    #endregion

    protected BaseObject targetObject;

    #region ActionEvent

    protected OnStatusUpdateArgs statusUpdateArgs;
    protected OnMoveActionArgs moveArgs;
    protected OnSkillActionArgs skillArgs;

    protected event EventHandler<OnStatusUpdateArgs> OnStatusUpdateEvent;
    protected event EventHandler<OnMoveActionArgs> OnMoveActionEvent;
    protected event EventHandler<OnSkillActionArgs> OnSkillActionEvent;

    #endregion

    private CancellationTokenSource _cancelSourceAsncUpdate;
    public virtual void Start()
    {   
        OnInitialize?.Invoke(this, EventArgs.Empty);
        AsncUpdate().Forget();
    }

    public virtual void OnDisable()
    {
        OnRelease?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void SetUp()
    {

    }

    protected virtual void initialize(object _o, EventArgs _args)
    {
        Debug.Log("## initialize ObjectHandler");
        _cancelSourceAsncUpdate = new CancellationTokenSource();
    }

    protected virtual void release(object _o, EventArgs _args)
    {
        Debug.Log("## release ObjectHandler");
        _cancelSourceAsncUpdate.Cancel();
        _cancelSourceAsncUpdate.Dispose();

        releaseArgs();
        releaseEvent();
    }

    private void releaseArgs()
    {
        statusUpdateArgs = null;
        moveArgs = null;
        skillArgs = null;
    } 

    private void releaseEvent()
    {
        OnStatusUpdateEvent = null;
        OnMoveActionEvent = null;
        OnSkillActionEvent = null;
    }

    public virtual void SetHandleTarget(BaseObject _targetObject)
    {
        Debug.LogError($"## SetHandleTarget");
        targetObject = _targetObject;
    }

    private async UniTask AsncUpdate()
    {
        while (true)
        {
            eventUpdate();

            await UniTask.WaitForFixedUpdate(_cancelSourceAsncUpdate.Token);
        }
    }

    private void eventUpdate()
    {
        OnStatusUpdateEvent?.Invoke(this, statusUpdateArgs);
        OnMoveActionEvent?.Invoke(this, moveArgs);
          
    }

    public virtual void OnSkillAction(SkillInfoData _data)
    {
        OnSkillActionEvent?.Invoke(this, skillArgs);
    }

}
