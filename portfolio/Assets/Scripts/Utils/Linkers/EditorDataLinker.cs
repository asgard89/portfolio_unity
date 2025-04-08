using System.Collections.Generic;
using UnityEditor;
using System.Reflection;
using UnityEditor.SceneManagement;


#if UNITY_EDITOR
[CustomEditor(typeof(DataLinker))]
public class EditorDataLinker : Editor
{
    List<string> class_list = null;
    string sClassName = "";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DataLinker linker = target as DataLinker;

        string className = linker.getClass();
        int selectIdx = 0;

        sClassName = EditorGUILayout.TextField("Search Class: ", sClassName);

        List<string> c_list = new List<string>
        {
            "NONE"
        };

        class_list = EditorDataLinker.getDataStructList();

        foreach (string c_name in class_list)
        {
            if (c_name.ToLower().Contains(sClassName.ToLower()))
            {
                c_list.Add(c_name);
            }
        }


        string[] selectArr = c_list.ToArray();
        selectIdx = -1;
        for (int i = 0; i < selectArr.Length; i++)
        {
            if (selectArr[i] == className)
            {
                selectIdx = i;
            }
        }

        EditorGUI.BeginChangeCheck();

        selectIdx = EditorGUILayout.Popup(selectIdx, selectArr);

        if (selectIdx >= 0)
        {
            linker.setClass(selectArr[selectIdx]);
        }

        EditorGUILayout.LabelField("Selected Data Class", linker.getClass());

        if (EditorGUI.EndChangeCheck())
        {

            EditorUtility.SetDirty(linker);

            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

            if (prefabStage != null)
            {
                EditorSceneManager.MarkSceneDirty(prefabStage.scene);
            }

        }
    }

    static List<string> dsNameList;
    static bool isSort = false;

    public static List<string> getDataStructList()
    {
        if (dsNameList == null)
        {

            dsNameList = new List<string>();
            Assembly asm = Assembly.GetExecutingAssembly();


            foreach (var t in asm.GetTypes())
            {
                if (t.IsClass && t.Namespace == "ASGA.DS")
                {
                    dsNameList.Add(t.Name);
                }
            }
        }

        if (!isSort)
        {
            dsNameList.Sort();
            isSort = true;
        }

        return dsNameList;
    }
}
#endif
