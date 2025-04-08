using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class EditorUtils : EditorWindow
{
    static EditorWindow instance;
    Vector2 scrollPos;
    List<string> csvFileList = null;
    Dictionary<string, bool> csvListToogle = new Dictionary<string, bool>();

    [MenuItem("ASGA/Utils/Update DB", priority = 1)]
    static void init()
    {
        ScriptableObject.CreateInstance<EditorUtils>();
        instance = EditorWindow.GetWindow<EditorUtils>();
    }

    void OnGUI()
    {
        EditorGUILayout.Space();

        if (GUILayout.Button("[UPDATE] Selected CSV Files to DB"))
        {
            List<string> selectedlist = new List<string>();
            foreach (string s in csvFileList)
            {
                if (false == csvListToogle[s])
                    continue;
                selectedlist.Add(s);
            }

            updateDB(selectedlist);
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.MaxHeight(600));
        foreach (string v in csvFileList)
        {
            csvListToogle[v] = EditorGUILayout.Toggle(v, csvListToogle[v], GUILayout.Height(16));
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    void OnFocus()
    {
        if (csvFileList == null)
        {
            csvFileList = getCSVFileList();
            csvListToogle.Clear();

            if (csvFileList != null && csvFileList.Count > 0)
            {
                foreach (string v in csvFileList)
                {
                    csvListToogle.Add(v, false);
                }
            }

        }
    }

    List<string> getCSVFileList()
    {
        List<string> __ret = new List<string>
        {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
        };
        return __ret;
    }
    void updateDB(List<string> _updateList)
    {

    }
}
