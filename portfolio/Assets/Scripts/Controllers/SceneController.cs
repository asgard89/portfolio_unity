using UnityEngine;
using System;
using ASGA.DS;

/// <summary>
/// �ϳ��� ��(��)�� �����ϴ� Prefab ����
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
