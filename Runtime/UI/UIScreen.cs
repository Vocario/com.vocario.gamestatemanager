using UnityEngine;

public class UIScreen : MonoBehaviour
{
    [SerializeField]
    private bool _hideOnStarted = true;
    // TODO Validate this with attributes
    [SerializeField]
    private UIScreenId _screenId = null;
    public UIScreenId ScreenId => _screenId;

    private void Awake()
    {
        _screenId.Instance = this;
        if (_hideOnStarted)
        {
            gameObject.SetActive(false);
        }
    }

    public void Show() => gameObject.SetActive(true);

    public void Hide() => gameObject.SetActive(false);
}
