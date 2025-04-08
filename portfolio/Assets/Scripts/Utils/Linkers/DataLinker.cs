using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataLinker : MonoBehaviour
{
    [HideInInspector]
    public string class_name;
    [HideInInspector]
    public object data = null;
    
    List<PropertyLinker> targetList = null;

    public void setClass(string _name)
    {
        this.class_name = _name;
    }

    public string getClass()
    {
        return this.class_name;
    }

    public void setDataObject(object o)
    {
        setData(o);
    }

    public T getData<T>() where T : class
    {
        return data as T;
    }

    public void setData<T>(T _any)
    {
        if (_any == null)
        {
            data = null;
            //return;
        }

        data = _any;

        if (targetList == null) initPropertyLinkerList();

        int n = targetList.Count;
        for (int i = 0; i < n; ++i)
        {
            var pl = targetList[i];
            setFieldValue(pl, pl.extractValue(data));
        }
    }

    void initPropertyLinkerList()
    {
        HashSet<PropertyLinker> p = new HashSet<PropertyLinker>();
        GameObject go = gameObject;
        PropertyLinker[] plArr = GetComponentsInChildren<PropertyLinker>(true);
        for (int i = 0; i < plArr.Length; ++i)
        {
            if (plArr[i].gameObject == go)
            {
                continue;
            }
            p.Add(plArr[i]);
        }

        var dlArr = GetComponentsInChildren<DataLinker>(true);
        for (int i = 0; i < dlArr.Length; ++i)
        {
            GameObject dgo = dlArr[i].gameObject;
            if (dgo == go)
            {
                continue;
            }
            PropertyLinker[] subPlArr = dlArr[i].GetComponentsInChildren<PropertyLinker>(true);
            //DataLinker - (PropertyLinker : DataLinker) DataLinker 중첩인 경우의 처리
            for (int k = 0; k < subPlArr.Length; ++k)
            {
                if (dgo != subPlArr[k].gameObject)
                {
                    p.Remove(subPlArr[k]);
                }
            }
        }

        targetList = new List<PropertyLinker>(p);
    }

    void setFieldValue(PropertyLinker _pl, object _v)
    {
        if (ReferenceEquals(_pl, null) == true) return;

        if (_v == null)
        {
            _v = "";
        }

        _pl.setPropertyValue(_v);

    }
}
