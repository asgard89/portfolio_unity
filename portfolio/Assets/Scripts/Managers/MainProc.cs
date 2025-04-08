using UnityEngine;
using ASGA.Single;
using ASGA.DS;
using System.Collections.Generic;
using System;
using System.Linq;

public enum MGR
{
    POOL,
    DATA,
    UI,
}

/// <summary>
/// 메인프로세서 (싱글톤)
/// Manager 와 Controller로 관리
/// scirptableobject로 불러올 manager를 정의.
/// </summary>
/// 
public class MainProc : MonoSingletone<MainProc>
{
    Dictionary<MGR, Manager> managers;

    public Transform UI_Root;

    private void Start()
    {
        OnStartInit = true;

        if (OnStartInit)
        {
            Initialize();
            DontDestroyOnLoad(this.gameObject);
        }            
    }

    public override void Initialize()
    {
        base.Initialize();

        instance = this;

        LoadingController.SetLoading<UIBase>((_loadingUI) => {
            loadData(() => {
                GetMGR<UIManager>().LoadUI<UITitle>("UI_Title", (uiTitle) => {
                    LoadingController.LoadingEnd(_loadingUI);
                });
                //SceneController.LoadScene<BaseObject>("scene_testroom", (sceneObject) =>
                //{
                //    //NOTE. test data
                //    using (SpawnInfoData __data = new SpawnInfoData
                //    {
                //        dataID = 0,
                //        dataName = "default_cube"
                //    })
                //    {
                //        SpawnController.SpawnObject<BaseObject>(__data, (baseObject) =>
                //        {
                //            Debug.LogError($"## Player Spawn Position Init");
                //            Vector3 _spawnPosition = Vector3.zero;
                //            _spawnPosition.y = 0.5f;
                //            baseObject.SetPosition(_spawnPosition);
                //            //
                //            LoadingController.LoadingEnd(_loadingUI);
                //        });
                //    }                       
                //});
            });
        });
    }

    async void loadData(Action _cb = null)
    {
        await AddressableManager.LoadAssetAsync<SOManagerDatas>("ds_manager", (dsmanagers) => {
            if (dsmanagers != null)
            {
                dsmanagers.managers.OrderBy(x => x.Priority);

                managers = new Dictionary<MGR, Manager>();

                foreach (var v in dsmanagers.managers)
                {
                    managers.Add(v.Key, v.Value);

                    v.Value.CustomStart();
                }

                _cb?.Invoke();
            }
        });
    }

    public override void Release()
    {
        base.Release();

        foreach (var v in managers)
        {
            v.Value.CustomOnDisable();
        }

        managers = null;
    }


    public T GetMGR<T>() where T : Manager
    {
        Type __t = typeof(T);

        if (__t == typeof(PoolManager))
        {
            return managers[MGR.POOL] as T;
        }
        else if (__t == typeof(DataManager))
        {
            return managers[MGR.DATA] as T;
        }
        else if (__t == typeof(UIManager))
        {
            return managers[MGR.UI] as T;
        }

        return null;
    }
}
