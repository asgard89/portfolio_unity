
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(PoolingGridLayoutGroup))]
[CanEditMultipleObjects]
public class EditorPoolingGridLayoutGroup : GridLayoutGroupEditor
{
    private SerializedProperty gridItem;
    private SerializedProperty scrollRect;
    protected override void OnEnable()
    {
        base.OnEnable();

        gridItem = serializedObject.FindProperty("gridItem");
        scrollRect = serializedObject.FindProperty("scrollRect");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(gridItem, true);
        EditorGUILayout.PropertyField(scrollRect, true);
       
        serializedObject.ApplyModifiedProperties();
    }
}
