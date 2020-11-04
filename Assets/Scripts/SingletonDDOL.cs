using UnityEngine;

namespace SoundBarrier
{
    public class SingletonDDOL<T> : MonoBehaviour where T : MonoBehaviour
    {
        static object m_lock = new object();

        protected static T m_singleton;
        public static T singleton
        {
            get
            {
                lock (m_lock)
                {
                    if (m_singleton == null)
                    {
                        m_singleton = FindObjectOfType<T>();

                        if (m_singleton == null)
                        {
                            GameObject newInstanceObject = new GameObject();
                            newInstanceObject.name = "SingletonDDOL[" + typeof(T).ToString() + "]";
                            m_singleton = newInstanceObject.AddComponent<T>();
                        }

                        DontDestroyOnLoad(m_singleton);
                    }
                }

                return m_singleton;
            }
        }

        protected virtual void Awake()
        {
            lock (m_lock)
            {
                if (m_singleton == null)
                {
                    m_singleton = FindObjectOfType<T>();

                    if (m_singleton == null)
                    {
                        GameObject newInstance = new GameObject();
                        newInstance.name = "SingletonDDOL[" + typeof(T).ToString() + "]";
                        m_singleton = newInstance.AddComponent<T>();
                    }
                    if (m_singleton != this)
                    {
                        Destroy(this.gameObject);
                    }
                    else
                    {
                        DontDestroyOnLoad(m_singleton);
                    }
                }
            }
        }
    }
}
