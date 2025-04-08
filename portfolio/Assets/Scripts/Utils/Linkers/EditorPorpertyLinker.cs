using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

#if UNITY_EDITOR
[CustomEditor(typeof(PropertyLinker))]
public class EditorPorpertyLinker : Editor
{
    PropertyLinker linker;
    string dataLinkerName = "";
    string fieldName = "";
    string propertyName = "";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        linker = target as PropertyLinker;
        dataLinkerName = linker.getDataLinkerName();

        EditorGUI.BeginChangeCheck();
        if (!string.IsNullOrEmpty(dataLinkerName))
        {
            setFeildName();
            setPropertyName();
        }

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(linker);
        }

    }

    void setFeildName()
    {
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Field Name", GUILayout.Width(90f));
        EditorGUILayout.BeginVertical();

        EditorGUILayout.LabelField(linker.getFieldName());
        fieldName = EditorGUILayout.TextField("Search Field: ", fieldName);

        List<string> list = new List<string>();
        //c_list.Add("NONE");

        Assembly asm = Assembly.GetExecutingAssembly();
        Type type;

        foreach (Type t in asm.GetTypes())
        {
            if (t.IsClass && t.Namespace == "ASGA.DS" && t.Name == dataLinkerName)
            {
                type = t;
                foreach (var fieldInfo in type.GetFields())
                {
                    list.Add(fieldInfo.Name);
                }
            }
        }

        List<string> list2 = new List<string>();
        if (string.IsNullOrEmpty(fieldName))
        {
            list2 = list;
        }
        else
        {
            foreach (string c_name in list)
            {
                if (c_name.ToLower().Contains(fieldName.ToLower()))
                {
                    list2.Add(c_name);
                }
            }
        }


        list2.Sort(delegate (string a, string b) {
            return a.CompareTo(b);
        });
        list2.Insert(0, "NONE");

        EditorGUI.BeginChangeCheck();

        string selectName = editorDrawOptions("", list2, linker.getFieldName());
        linker.setFieldName(selectName);

        if (EditorGUI.EndChangeCheck())
        {
            if (selectName != "NONE")
            {
                linker.setPropertyName("NONE");
            }
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }

    string editorDrawOptions(string title, List<string> list, string check)
    {
        string ret = "";

        if (list.Count > 0)
        {

            int _choiceIndex = 0;
            string[] _choices = list.ToArray();
            for (int i = 0; i < _choices.Length; i++)
            {
                if (_choices[i] == check)
                {
                    _choiceIndex = i;
                }
            }

            _choiceIndex = EditorGUILayout.Popup(title, _choiceIndex, _choices);

            ret = _choices[_choiceIndex];
        }

        return ret;
    }

    void setPropertyName()
    {
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Property Name", GUILayout.Width(90f));
        EditorGUILayout.BeginVertical();


        EditorGUILayout.LabelField(linker.getPropertyName());

        this.propertyName = EditorGUILayout.TextField("Search Property: ", this.propertyName);

        List<string> c_list = new List<string>();
        //c_list.Add("NONE");

        Assembly asm = Assembly.GetExecutingAssembly();
        Type type;

        Dictionary<string, PropertyInfo> propertyMap = new Dictionary<string, PropertyInfo>();
        foreach (Type t in asm.GetTypes())
        {
            if (t.IsClass && t.Namespace == "ASGA.DS" && t.Name == dataLinkerName)
            {
                type = t;
                foreach (var propertyInfo in type.GetProperties())
                {
                    c_list.Add(propertyInfo.Name);
                    propertyMap[propertyInfo.Name] = propertyInfo;
                }
            }
        }


        List<string> pList = new List<string>();
        if (string.IsNullOrEmpty(this.propertyName))
        {
            pList = c_list;
        }
        else
        {
            foreach (string c_name in c_list)
            {
                if (c_name.ToLower().Contains(this.propertyName.ToLower()))
                {
                    pList.Add(c_name);
                }
            }
        }

        pList.Sort(delegate (string a, string b) {
            return a.CompareTo(b);
        });
        pList.Insert(0, "NONE");

        EditorGUI.BeginChangeCheck();

        string selectName = editorDrawOptions("", pList, linker.getPropertyName());
        linker.setPropertyName(selectName);

        if (EditorGUI.EndChangeCheck())
        {
            if (selectName != "NONE")
            {
                linker.setFieldName("NONE");
            }
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

    }
}
#endif