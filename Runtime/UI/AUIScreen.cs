using UnityEngine;
using UnityEngine.UIElements;

public abstract class AUIScreen : MonoBehaviour
{
    [Tooltip("String ID from the UXML for this menu panel/screen.")]
    [SerializeField]
    protected string _screenName = null;

    // [Header("UI Management")]
    // [Tooltip("Set the Main Menu here explicitly (or get automatically from current GameObject).")]
    // [SerializeField]
    // protected UIManagerBehaviour _uiManager = null;

    [Tooltip("Set the UI Document here explicitly (or get automatically from current GameObject).")]
    [SerializeField]
    protected UIDocument _document = null;

    // visual elements
    protected VisualElement _screen = null;
    protected VisualElement _root = null;


    [SerializeField]
    private bool _hideOnStarted = true;
    // TODO Validate this with attributes
    [SerializeField]
    private UIScreenId _screenId = null;
    public UIScreenId ScreenId => _screenId;

    protected virtual void OnValidate()
    {
        if (string.IsNullOrEmpty(_screenName))
        {
            _screenName = GetType().Name;
        }
    }

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
