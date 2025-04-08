using System;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using ASGA.DS;

/// <summary>
/// 시스템(게임)에 사용되는 리소스데이터를 관리
/// </summary>
[Serializable, CreateAssetMenu(fileName = "SODataManager", menuName = "Scriptable Manager/Data", order = 10005)]
public class DataManager : Manager, IInitializeHandler
{
    const string DBName = "/foo.db";
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
        try
        {
            string __connPath = $"{Application.streamingAssetsPath}{DBName}";

            using (SQLiteConnection __dbConn = new SQLiteConnection(__connPath))
            {
                //NOTE. db test
                string __tableName = "'SpawnInfoData'";
                SQLiteCommand __command = __dbConn.CreateCommand($"SELECT * FROM {__tableName}");

                List<SpawnInfoData> __l = __command.ExecuteQuery<SpawnInfoData>();

                //foreach(SpawnInfoData s in __l)
                //{
                //    Debug.LogError($"#### dataId : {s.dataID} , {s.dataName} , {s.objectType}");
                //}
            }
        }
        catch(Exception e)
        {
            Debug.LogError($"@@ DB Connect Error : {e.Message}");
        }
    }

    void release(object _o, EventArgs _Args)
    {
        Debug.Log($"## OnRelease : {_o.GetType().Name}");
    }

}
