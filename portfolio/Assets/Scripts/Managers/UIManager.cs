using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable, CreateAssetMenu(fileName = "SOUIManager", menuName = "Scriptable Manager/UI", order = 10006)]
public class UIManager : Manager
{
    List<UIBase> loadUIs = new List<UIBase>();

    protected override void SetUp()
    {
        base.SetUp();

    }
    public override void CustomStart()
    {
        SetUp();
    }

    public override void CustomOnDisable()
    {
        
    }

    public async void LoadUI<T>(string _key, Action<T> _cb = null) where T : UIBase
    {
        await AddressableManager.InstantiateAssetAsyncUI<T>(_key, (loadUIObject) =>
        {
            _cb?.Invoke(loadUIObject as T);
            loadUIs.Add(loadUIObject);
        });
    }

    public void UnLoadUI<T>(T _uiOBject) where T : UIBase
    {
        loadUIs.Remove(_uiOBject);

        AddressableManager.ReleaseInstanceUI(_uiOBject);
    }
}
