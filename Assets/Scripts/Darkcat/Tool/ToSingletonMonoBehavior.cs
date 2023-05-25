using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ToSingletonMonoBehavior<T> : MonoBehaviour where T : MonoBehaviour
{
    static T instance_ = null;
    public static T Instance
    {
        get
        {
            if (instance_ != null)
            {
                return instance_;
            }

            var gameobject = FindObjectOfType(typeof(T));
            if (gameobject != null)
            {
                instance_ = gameobject as T;
                return instance_;
            }

            var newO = new GameObject(nameof(T));
            Instantiate(newO);
            instance_ = newO.AddComponent<T>();
            return instance_;
        }
    }


    void Awake()
    {
        if (instance_ == null) instance_ = this as T;
        if (instance_ == this) DontDestroyOnLoad(this);
        else DestroyImmediate(this.gameObject);

        init();
    }

    protected virtual void init()
    {
    }
}
