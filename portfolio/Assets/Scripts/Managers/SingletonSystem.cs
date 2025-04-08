using Unity.VisualScripting;
using UnityEngine;

namespace ASGA.Single
{
    public class MonoSingletone<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static bool OnStartInit = false;
        protected static object instancelock = new object();
        protected static T instance;
        protected static bool isAlive = true;

        public static T Instance
        {
            get
            {
                lock (instancelock)
                {
                    if (false == isAlive)
                    {
                        return null;
                    }

                    CreateSingletonInstance();

                    return instance;
                }
            }
        }

        private static void CreateSingletonInstance()
        {
            if (instance == null && false == OnStartInit)
            {
                string singletonName = typeof(T).Name;
                instance = FindAnyObjectByType(typeof(T)) as T; //
    
                if (instance != null)
                {
                    GameObject obj = new GameObject();
                    instance = obj.AddComponent<T>();
                }

                instance.gameObject.name = singletonName;

                instance.SendMessage("Initialize");

                DontDestroyOnLoad(instance.gameObject);
            }
        }

        public virtual void Initialize()
        {

        }

        public virtual void Release()
        {
            isAlive = false;
            instance = null;
        }

        public virtual void OnUpdate()
        {

        }

        private void OnDisable()
        {
            Release();
        }
    }
}
