using UnityEngine;

public class UIBase : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        
    }

    protected virtual void InitializeUI()
    {

    }

    public virtual T GetData<T>() where T : class
    {
        return null;
    }

    public virtual void SetData<T>(T _data)where T : class
    {

    }

    public virtual void Refresh()
    {

    }
}
