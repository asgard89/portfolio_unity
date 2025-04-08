using System;
using UnityEngine;

/// <summary>
/// 로딩할 때 로딩화면을 관리
/// </summary>
public class LoadingController : Controller
{
    //public static event EventHandler<OnLoadingArgs> OnComplete;
    public static async void SetLoading<T>(Action<T> _cb = null) where T : UIBase
    {
        await AddressableManager.InstantiateAssetAsyncUI<UIBase>("ui_loading", (loadingUI) =>
        {
            _cb?.Invoke(loadingUI as T);
        });
    }

    public static void LoadingEnd(UIBase _loading)
    {
        Debug.LogError($"## loadingEnd");
        AddressableManager.ReleaseInstanceUI(_loading);

        //OnComplete?.Invoke(null, new());
    }
}
