using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unity Inspector 에서 KeyValuePair를 보여주기 위해 만든 클래스
/// </summary>

#region KeyValuePair
[Serializable]
public class SerializableKeyValue <T1, T2>
{
    public T1 Key;
    public T2 Value;
}

[Serializable]
public class SerializableKeyPriority <T1>
{
    public T1 Key;
    public int Priority;
}

[Serializable]
public class SerializableKeyValuePriority<T1, T2>
{
    public T1 Key;
    public T2 Value;
    public int Priority;
}
#endregion

/// <summary>
/// 불러올 매니저 데이터
/// </summary>
[Serializable, CreateAssetMenu(fileName = "managerDatas", menuName = "Scriptable Datas/Manager", order = 10002)]
public class SOManagerDatas : ScriptableObject
{
    [SerializeField] public List<SerializableKeyValuePriority<MGR, Manager>> managers;
}
