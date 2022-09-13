using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField]
    private bool _shouldDestroyOnLoad = false;
    private static T _instance = null;
    public static T Instance
    {
        get
        {
            if (_instance == null && GameObject.FindObjectOfType<T>() == null)
            {
                var newObject = new GameObject(typeof(T).Name);
                _instance = newObject.AddComponent<T>();
                _ = newObject.AddComponent<Canvas>();
                return _instance;
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_shouldDestroyOnLoad)
        {
            GameObject.DontDestroyOnLoad(_instance);
        }
    }
}
