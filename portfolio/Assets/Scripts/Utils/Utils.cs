using UnityEngine;
using System;
using UnityEngine.U2D;
using System.Collections.Generic;
using Unity.VisualScripting;

public static class Utils 
{
    static List<string> __grades = new List<string>()
                    {
                        "F",
                        "C",
                        "B",
                        "A",
                        "S",
                        "U",
                    };
    public static async void RequestAtlas(string _atlasname, Action<SpriteAtlas> _cb)
    {
        await AddressableManager.LoadAssetAsync2<SpriteAtlas>(_atlasname, (sa) =>
        {
            _cb?.Invoke(sa);
        });
    }

    public static string GetItemGradeName(int __grade)
    {
        if (__grade >= 0 && __grade < __grades.Count)
        {
            return __grades[__grade];
        }

        return "";
    }

    public static string GetEnumName(this Enum _value)
    {
        return Enum.GetName(_value.GetType(), _value);
    }
}
