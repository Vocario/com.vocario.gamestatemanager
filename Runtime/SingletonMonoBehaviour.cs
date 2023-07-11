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
            T foundInScene = GameObject.FindObjectOfType<T>();

            if (_instance == null && foundInScene == null)
            {
                var newObject = new GameObject(typeof(T).Name);

                _instance = newObject.AddComponent<T>();
                return _instance;
            }
            if (foundInScene != null)
            {
                _instance = foundInScene;
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (!_shouldDestroyOnLoad)
        {
            GameObject.DontDestroyOnLoad(this);
        }
    }
}
