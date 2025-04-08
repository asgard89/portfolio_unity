using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class PropertyLinker : MonoBehaviour
{
    [HideInInspector]
    public string defaultValue;
    [HideInInspector]
    public string fieldName = "";
    [HideInInspector]
    public string propertyName = "";

    public string format;

    public interface IReflectGet
    {
        string GetType();
        object GetValue(object o);
    }

    //필드 타입 반환
    class FieldGet : IReflectGet
    {
        public FieldInfo fi;
        string type = "FIELD";
        public object GetValue(object o)
        {
            return fi.GetValue(o);
        }
        public new string GetType()
        {
            return type;
        }
    }

    //프로퍼티 타입 반환
    class PropertyGet : IReflectGet
    {
        public PropertyInfo pi;
        public string type = "PROPERTY";

        public object GetValue(object o)
        {
            return pi.GetValue(o, null);
        }
        public new string GetType()
        {
            return type;
        }
    }

    public static IReflectGet getReflectGet(Type ty, string name)
    {
        IReflectGet r;

        var fi = ty.GetField(name);
        if (fi != null)
        {
            r = new FieldGet { fi = fi };
            return r;
        }

        var pi = ty.GetProperty(name);
        if (pi != null)
        {
            r = new PropertyGet { pi = pi };
            return r;
        }

        return null;
    }

    //PropertyLinker에 설정된 값 반환
    public object extractValue(object data)
    {
        if (data == null) { return null; }
        if (string.IsNullOrEmpty(fieldName) && string.IsNullOrEmpty(propertyName))
        {
            return null;
        }
        object ret_value = _extractValue(data);
        return ret_value;
    }

    object _extractValue(object data)
    {
        object refValue = null;

        Type ty = data.GetType();

        string _proPertyName = (fieldName == "") ? propertyName : fieldName;

        var propertyObj = getReflectGet(ty, _proPertyName);
        if (propertyObj == null)
        {
            Debug.LogError(_proPertyName);
            return null;
        }

        if (propertyObj.GetType() == "FIELD")
        {
            var refInfo = propertyObj as FieldGet;
            if (refInfo != null)
            {
                refValue = refInfo.GetValue(data);
            }
        }
        else if (propertyObj.GetType() == "PROPERTY")
        {
            var refInfo = propertyObj as PropertyGet;
            if (refInfo != null)
            {
                refValue = refInfo.GetValue(data);
            }
        }

        return refValue;
    }

    public string getDataLinkerName()
    {
        DataLinker dl = null;
        Transform t = transform;
        while (t != null && dl == null)
        {
            t = t.parent;
            if (t) dl = t.GetComponent<DataLinker>();
        }
        if (t == null)
        {
            Debug.LogError("No DataLinker (this): " + gameObject.name, this);
        }

        string ret = "";
        if (dl != null)
        {
            ret = dl.getClass();
        }
        else
        {
            Debug.LogError("No DataLinker : " + gameObject.name);
        }
        return ret;
    }

    public void setFieldName(string name)
    {
        name = (name == "NONE") ? "" : name;
        this.fieldName = name;
    }

    public string getFieldName()
    {
        return this.fieldName;
    }

    public void setPropertyName(string name)
    {
        name = (name == "NONE") ? "" : name;
        this.propertyName = name;
    }

    public string getPropertyName()
    {
        return this.propertyName;
    }

    public virtual string getFormat()
    {
        return this.format;
    }

    public string getFormattedString(object o)
    {
        string ret = o.ToString();

        if (string.IsNullOrEmpty(ret))
        {
            ret = getDefaultValue();
        }

        if (string.IsNullOrEmpty(ret))
        {
            return string.Empty;
        }

        var fmt = getFormat();
        if (!string.IsNullOrEmpty(fmt))
        {
            ret = string.Format(fmt, ret);
        }
        return ret;
    }

    public string getDefaultValue()
    {
        return defaultValue;
    }

    public void setPropertyValue(object v)
    {
        if (!this) return;

        var coms = GetComponents<MonoBehaviour>();
        for (int i = 0; i < coms.Length; i += 1)
        {
            MonoBehaviour c = coms[i];
            if (c is DataLinker __datalinker)
            {
                __datalinker.setDataObject(v);
                return;
            }
            if (c is Text __text)
            {
                setText(__text, v);
            }
            else if (c is Image __image)
            {
                setImage(__image, v);
            }
            else if (c is TMPro.TextMeshProUGUI __textMeshPro)
            {
                setTextMeshPro(__textMeshPro, v);
            }
            else if (c is PropertyDisplayer __diplayer)
            {
                __diplayer.SetEnable((bool)v);
            }
        }        
    }
    #region 컴포넌트별 데이터 세팅
    void setTextMeshPro(TMPro.TextMeshProUGUI tmp, object o)
    {
        string v = o.ToString();
        if (v == "")
        {
            v = getDefaultValue();
        }
        else
        {
            string format = getFormat();
            if (!string.IsNullOrEmpty(format))
            {
                v = string.Format(format, o);
            }
        }

        tmp.text = v;

        PropertyColorSetter pcs = tmp.gameObject.GetComponent<PropertyColorSetter>();

        if (pcs != null)
        {
            tmp.color = pcs.GetColor(v);
        }    
    }

    void setText(Text text, object o)
    {

        string v = o.ToString();

        if (v == "")
        {
            v = getDefaultValue();
        }
        else
        {
            string format = getFormat();
            if (!string.IsNullOrEmpty(format))
            {
                v = string.Format(format, o);
            }
        }

        text.text = v;
    }

    void setImage(Image img, object o)
    {
        string spriteName = o.ToString(); //아틀라스 이름과 스프라이트이름을 함께 넘겨준다. ','로 구분

        string[] strArr = spriteName.Split(',');
        if (strArr.Length < 2)
        {
            img.sprite = null;
            img.enabled = false;
            return;
        }

        string atlas_name = strArr[0];
        string sprite_name = strArr[1];

        if (!string.IsNullOrEmpty(getFormat()))
        {
            sprite_name = string.Format(getFormat(), sprite_name);
        }

        Utils.RequestAtlas(atlas_name, sa =>
        {
            Sprite sprite = sa.GetSprite(sprite_name);
            img.enabled = true;
            img.sprite = sprite;
        });

    }
    #endregion
}
