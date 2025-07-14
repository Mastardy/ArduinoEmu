using UnityEngine;

namespace Mastardy.Runtime.Utils
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        public static T Instance { get; private set;  }
        public static bool IsInitialized => Instance != null;
        
        protected virtual void Awake()
        {
            if (Instance == null) Instance = (T)this;
            else Destroy(gameObject);
        }
        
        protected virtual void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }
    }
}