using UnityEngine;

namespace Unchord
{
    public abstract class Singleton<T> : MonoBehaviour where T : Component
    {
        public static T Instance
        {
            get
            {
                if (s_m_destroyed)
                {
                    Debug.LogWarningFormat("Singleton instance '{0}' already destroyed. Returns null.", typeof(T));
                    return null;
                }

                lock (s_m_lock)
                {
                    if (s_m_instance == null && (s_m_instance = (T)FindAnyObjectByType(typeof(T))) == null)
                    {
                        GameObject singletonObject = new GameObject();
                        s_m_instance = singletonObject.AddComponent<T>();
                        singletonObject.name = $"@{typeof(T)}";
                        DontDestroyOnLoad(singletonObject);
                    }

                    return s_m_instance;
                }
            }
        }

        private static T s_m_instance;
        private static object s_m_lock = new object();
        private static bool s_m_destroyed;

        private void OnApplicationQuit()
        {
            s_m_destroyed = true;
        }

        private void OnDestroy()
        {
            s_m_destroyed = true;
        }
    }
}