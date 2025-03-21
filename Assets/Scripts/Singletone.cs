using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///Generic singletone class 
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singletone<T> : MonoBehaviour where T : Singletone<T>
{
    private static T instance;

    public static T Instance { get { return instance; } }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = (T)this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

}
