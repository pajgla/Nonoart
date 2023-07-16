using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T m_Instance = null;

    protected virtual bool Awake()
    {
        if (m_Instance != null)
        {
            Destroy(this.gameObject);
            return false;
        }

        m_Instance = this as T;
        DontDestroyOnLoad(m_Instance);
        return true;
    }

    public static T Get()
    {
        return m_Instance;
    }
}
