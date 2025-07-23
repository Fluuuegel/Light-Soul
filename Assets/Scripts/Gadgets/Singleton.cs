using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

namespace Gadgets.Singleton
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T _instance;
        private static bool _isClosing = false;

        public static T Instance
        {
            get
            {
                if (_isClosing)
                    return null;

                if (_instance == null)
                {
                    // Avoid instantiate multiple gameobject
                    _instance = FindFirstObjectByType<T>();

                    if (_instance == null)
                    {
                        GameObject go = new GameObject(typeof(T).Name);
                        _instance = go.AddComponent<T>();
                    }
                    DontDestroyOnLoad(_instance.gameObject);
                }

                return _instance;
            }
        }

        protected virtual void OnDestroy()
        {
            _isClosing = true;
        }
    }
}