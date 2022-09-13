using System.Collections.Generic;

public class UIManagerBehaviour : SingletonMonoBehaviour<UIManagerBehaviour>
{
    private Dictionary<UIScreenId, UIScreen> _screens = new Dictionary<UIScreenId, UIScreen>();

    public void Show(UIScreenId id)
    {
        if (!_screens.ContainsKey(id))
        {
            // TODO Prefab validation
            UIScreen newScreen = Instantiate(id.Screen, transform);
            _screens.Add(id, newScreen);
        }
        _screens[ id ].Show();
    }

    public void Hide(UIScreenId id) =>
        // TODO Validation
        _screens[ id ].Hide();

    protected override void Awake()
    {
        base.Awake();
        UIScreen[] screens = GetComponentsInChildren<UIScreen>();
        foreach (UIScreen screen in screens)
        {
            _screens.Add(screen.ScreenId, screen);
        }
    }
}
