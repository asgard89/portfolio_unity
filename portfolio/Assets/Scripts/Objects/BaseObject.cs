using Unity.VisualScripting;
using UnityEngine;

public class BaseObject : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void SetPosition(Vector3 _position)
    {
        transform.position = _position;
    }

    public void SetLocalPosition(Vector3 _localPosition)
    {
        transform.localPosition = _localPosition;
    }

    public void SetRotation(Quaternion _rotation) 
    {  
        transform.rotation = _rotation; 
    }

    public void SetLocalRotation(Quaternion _localRotation)
    {
        transform.localRotation = _localRotation;
    }

    public void SetRotation(Vector3 _eulerAngles)
    {
        transform.eulerAngles = _eulerAngles;
    }

    public void SetLocalRotation(Vector3 _localEulerAngles)
    {
        transform.localEulerAngles = _localEulerAngles;
    }

    public T AddObjectHandler<T>() where T : ObjectHandler
    {
        return gameObject.AddComponent<T>();
    }
}
