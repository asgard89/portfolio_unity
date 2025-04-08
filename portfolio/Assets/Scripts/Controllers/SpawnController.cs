using System;
using ASGA.DS;
using UnityEngine;

/// <summary>
/// BaseObject를 생성하고 목적에 맞는 Handler를 붙임.
/// </summary>
public class SpawnController : Controller
{
    public static void SpawnObject<T>(SpawnInfoData _data, Action<T> _cb = null) where T : BaseObject
    {
        switch (_data.objectType)
        {
            case 0:
                spawnPlayer<T>(_data.dataName, _cb);
                break;
            case 1:
                spawnMob<T>(_data.dataName, _cb);
                break;
            case 2:
                spawnEffect<T>(_data.dataName, _cb);
                break;
            case 3:
                spawnScene<T>(_data.dataName, _cb);
                break;
            default:
                break;
        }
    }

    static void spawnPlayer<T>(string _name, Action<T> _cb = null) where T : BaseObject
    {
        PoolManager __poolMgr = MainProc.Instance.GetMGR<PoolManager>();

        Action<T> __cb = (spawnObject) =>
        {
            if (spawnObject is BaseObject __obj)
            {
                PlayerHandler __handler = __obj.AddObjectHandler<PlayerHandler>();
                __handler.SetHandleTarget(__obj);
            }

            _cb?.Invoke(spawnObject);
        };

        __poolMgr.PopObjectAsync<T>(_name, __cb);
    }

    static void spawnMob<T>(string _name, Action<T> _cb = null) where T : BaseObject
    {
        PoolManager __poolMgr = MainProc.Instance.GetMGR<PoolManager>();

        Action<T> __cb = (spawnObject) =>
        {
            if (spawnObject is BaseObject __obj)
            {
                MobHandler __handler = __obj.AddObjectHandler<MobHandler>();
                __handler.SetHandleTarget(__obj);
            }
            _cb?.Invoke(spawnObject);
        };

        __poolMgr.PopObjectAsync<T>(_name, __cb);
    }

    static void spawnEffect<T>(string _name, Action<T> _cb = null) where T : BaseObject
    {
        PoolManager __poolMgr = MainProc.Instance.GetMGR<PoolManager>();

        Action<T> __cb = (spawnObject) =>
        {
            if (spawnObject is BaseObject __obj)
            {
                EffectHandler __handler = __obj.AddObjectHandler<EffectHandler>();
                __handler.SetHandleTarget(__obj);
            }

            _cb?.Invoke(spawnObject);
        };

        __poolMgr.PopObjectAsync<T>(_name, __cb);
    }

    static void spawnScene<T>(string _name, Action<T> _cb = null) where T : BaseObject
    {
        PoolManager __poolMgr = MainProc.Instance.GetMGR<PoolManager>();

        Action<T> __cb =  (sceneObject) =>
        {
            if (sceneObject is BaseObject __obj)
            {
               
            }

            _cb?.Invoke(sceneObject);
        };

        __poolMgr.PopObjectAsync<T>(_name, __cb);
    }
}
