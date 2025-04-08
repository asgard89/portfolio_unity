using UnityEngine;
using System;
using System.Collections.Generic;

public enum OBJECT_TYPE
{
    DEFAULT,
    ENEMY,
}

[Serializable, CreateAssetMenu(fileName = "SOPoolManager", menuName = "Scriptable Manager/Pool", order = 10004)]
public class PoolManager : Manager, IInitializeHandler
{
    Dictionary<string, Stack<BaseObject>> baseObjectPool;

    #region Derived Interface - IInitializeHandler

    public event EventHandler OnInitialize;
    public event EventHandler OnRelease;
    public event EventHandler OnRefresh;

    #endregion

    #region Derived Class - Manager

    protected override void SetUp()
    {
        base.SetUp();

        OnInitialize += initialize;
        OnRelease += release;
    }
    public override void CustomStart()
    {
        SetUp();

        OnInitialize?.Invoke(this, EventArgs.Empty);
    }

    public override void CustomOnDisable()
    {
        OnRelease?.Invoke(this, EventArgs.Empty);

        OnInitialize = null;
        OnRelease = null;
        OnRefresh = null;
    }

    #endregion

    void initialize(object _o, EventArgs _Args)
    {
        Debug.Log($"## OnInitialize : {_o.GetType().Name}");
        baseObjectPool = new Dictionary<string, Stack<BaseObject>>();
    }

    void release(object _o, EventArgs _Args)
    {
        Debug.Log($"## OnRelease : {_o.GetType().Name}");
    }

    public void PopObjectAsync<T>(string _name, Action<T> _cb = null) where T : BaseObject
    {
        popObjectAsync(_name, _cb);
    }

    async void popObjectAsync<T>(string _name, Action<T> _cb = null) where T : BaseObject
    {
        if (baseObjectPool.TryGetValue(_name, out Stack<BaseObject> __stack) && __stack != null && __stack.Count > 0)
        {
            BaseObject __r = __stack.Pop();

            _cb?.Invoke(__r as T);

            return;
        }

        await AddressableManager.InstantiateAssetAsync(_name, _cb);
    }

    public void PushObject<T>(T _object, Action _cb = null) where T : BaseObject
    {
        _cb?.Invoke();
        _object.gameObject.SetActive(false);

        string __dataId = "";

        if (baseObjectPool.TryGetValue(__dataId, out Stack<BaseObject> __stack))
        {
            if (__stack == null)
            {
                __stack = new Stack<BaseObject>();
            }
            __stack.Push(_object);
        }
        else
        {
            baseObjectPool.Add(__dataId, new Stack<BaseObject>());
            baseObjectPool[__dataId].Push(_object);
        }

    }
}
