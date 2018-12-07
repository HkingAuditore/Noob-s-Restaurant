using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单例模板
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class MonoSingleton<T> : MonoBehaviour 
    where T:MonoBehaviour  
{
    private static T m_instance;
    public static T Instance
    {
        get { return m_instance; }
    }

    protected virtual void Awake()
    {
        m_instance = this as T;
    }

}
