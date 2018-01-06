/*
  备 注：
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#region singleton
/// <summary>
/// Singleton Class
/// </summary>
public abstract class Singleton<T> where T : new()
{
    private static T singleton;
    public static T Instance
    {
        get
        {
            if (singleton == null)
            {
                singleton = new T();
            }
            return singleton;
        }
    }
    public static T instance
    {
        get
        {
            if (singleton == null)
            {
                singleton = new T();
            }
            return singleton;
        }
    }
}
#endregion

#region SingletonMono
/// <summary>
/// Single Ton Monobehavior. unique in a scene
/// </summary>
public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T singleton;

    public static bool IsNull() { return singleton == null; }

    private void Awake()
    {
        if (Application.isPlaying)
        {
            singleton = (this as T);
            this.OnAwake();
        }
    }

    protected virtual void OnAwake()
    {
    }

    private void Start()
    {
        this.OnStart();
    }

    protected virtual void OnStart()
    {
    }

    public static T Instance
    {
        get
        {
            if (singleton == null)
            {
                GameObject gameObject = GameObject.Find("SingleMonoBehaviour");
                if (gameObject == null)
                    gameObject = new GameObject("SingleMonoBehaviour");
                GameObject obj = new GameObject();
                obj.name = "[" + typeof(T).Name + "]";
                obj.SetActive(true);
                obj.transform.SetParent(gameObject.transform);
                singleton = obj.AddComponent<T>();
            }
            return singleton;
        }
    }
    public static T instance
    {
        get
        {
            if (singleton == null)
            {
                GameObject gameObject = GameObject.Find("SingleMonoBehaviour");
                if (gameObject == null)
                    gameObject = new GameObject("SingleMonoBehaviour");
                GameObject obj = new GameObject();
                obj.name = "[" + typeof(T).Name + "]";
                obj.SetActive(true);
                obj.transform.SetParent(gameObject.transform);
                singleton = obj.AddComponent<T>();
            }
            return singleton;
        }
    }
}
#endregion
