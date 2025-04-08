using UnityEngine;
using System;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;

/// <summary>
/// Unity Addressable 형태로 관리하는 Asset의 Load, Instaniate 및 Release를 수행
/// </summary>
public class AddressableManager
{
    public static async UniTask<T> LoadAssetAsync<T>(string _key, Action<T> _cb = null) where T : ScriptableObject
    {
        //handle을 release
        var __asyncOperation = Addressables.LoadAssetAsync<T>(_key);

        __asyncOperation.Completed += (handle) =>
        {
            try
            {
                T __r = handle.Result;

                _cb?.Invoke(__r);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"LoadAssetAsync : {e.Message}");
            }

        };

        ScriptableObject __ret = await __asyncOperation.Task.AsUniTask();

        return __ret as T;

    }

    public static async UniTask<T> InstantiateAssetAsyncUI<T>(string _key, Action<T> _cb = null) where T : UIBase
    {
        var __asyncOperation = Addressables.InstantiateAsync(_key, MainProc.Instance.UI_Root);

        __asyncOperation.Completed += (handle) =>
        {
            try
            {
                T __r = handle.Result.GetComponent<T>();

                _cb?.Invoke(__r);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"InstantiateAssetAsyncUI : {e.Message}");
            }

        };

        GameObject __ret = await __asyncOperation.Task.AsUniTask();

        return __ret as T;

    }

    public static async UniTask<T> InstantiateAssetAsync<T>(string _label, Action<T> _cb = null) where T : BaseObject
    {
        var __asyncOperation = Addressables.InstantiateAsync(_label);

        __asyncOperation.Completed += (handle) =>
        {
            try
            {
                T __r = handle.Result.GetComponent<T>();

                _cb?.Invoke(__r);
            }
            catch(Exception e)
            {
                Debug.LogWarning($"InstantiateAssetAsync : {e.Message}");
            }
           
        };

        GameObject __ret = await __asyncOperation.Task.AsUniTask();

        return __ret as T;
    }

    public static void ReleaseInstance<T>(T __obj) where T : BaseObject
    {
        try
        {
            Addressables.ReleaseInstance(__obj.gameObject);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    public static void ReleaseInstanceUI<T>(T __obj) where T : UIBase
    {
        try
        {
            Addressables.ReleaseInstance(__obj.gameObject);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    public static async UniTask<T> LoadAssetAsync2<T>(string _key, Action<T> _cb = null) where T : class
    {
        //handle을 release
        var __asyncOperation = Addressables.LoadAssetAsync<T>(_key);

        __asyncOperation.Completed += (handle) =>
        {
            try
            {
                T __r = handle.Result;

                _cb?.Invoke(__r);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"LoadAssetAsync : {e.Message}");
            }

        };

        T __ret = await __asyncOperation.Task.AsUniTask();

        return __ret as T;

    }

}
