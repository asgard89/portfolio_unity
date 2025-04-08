using UnityEngine;
using System;
using ASGA.DS;

/// <summary>
/// 하나의 씬(맵)을 구성하는 Prefab 관리
/// </summary>
public class SceneController : Controller
{
    public static void LoadScene<T>(string _sceneName, Action<T> _cb = null) where T : BaseObject
    {
        using (SpawnInfoData __data = new SpawnInfoData
        {
            dataID = 4,
            dataName = _sceneName,
        })
        {
            SpawnController.SpawnObject(__data, _cb);
        }

    }

    public static void ReleaseCurrentScene()
    {

    }

    public static void ReleaseAllScene()
    {

    }
}
