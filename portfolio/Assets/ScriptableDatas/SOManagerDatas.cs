using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unity Inspector ���� KeyValuePair�� �����ֱ� ���� ���� Ŭ����
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
/// �ҷ��� �Ŵ��� ������
/// </summary>
[Serializable, CreateAssetMenu(fileName = "managerDatas", menuName = "Scriptable Datas/Manager", order = 10002)]
public class SOManagerDatas : ScriptableObject
{
    [SerializeField] public List<SerializableKeyValuePriority<MGR, Manager>> managers;
}
