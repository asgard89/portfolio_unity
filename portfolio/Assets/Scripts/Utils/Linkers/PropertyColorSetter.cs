using UnityEngine;
using System.Collections.Generic;

public class PropertyColorSetter : MonoBehaviour
{
    public List<SerializableKeyValue<string, Color>> colorTable;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public Color GetColor(string _key)
    {
        Color __c = Color.white;

        if (colorTable != null)
        {
            SerializableKeyValue<string, Color> __getValue = colorTable.Find(x => x.Key == _key);
            if (__getValue != null)
            {
                __c = __getValue.Value;
            }
        }

        return __c;
    }
}
