
using UnityEngine;

namespace Manager
{
    public abstract class ManagerBase<T> : MonoBehaviour where T : ManagerBase<T>
    {
        protected static T _instance;
        public static T Instance => _instance;     

        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = (T)this;
            DontDestroyOnLoad(gameObject);
        }
    }
}