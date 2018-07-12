using UnityEngine;
using System.Collections;

public class Singleton<T> : MonoBehaviour where T:MonoBehaviour 
{
    private static T instance;
    public static T Instance 
    {
        get { return instance; }
    }


    public virtual void Awake() 
    {
        if (instance == null)
        {
            instance = this as T;
        }
    }

    public virtual void OnDestroy() 
    {
        if (instance != null)
        {
            instance = null;
        }
    }

}
