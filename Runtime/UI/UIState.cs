using UnityEngine;
using System.Collections.Generic;

public class UIState : AState
{
    private UIManager _uIManager = null;
    public List<UIScreenId> ScreensToShowOnEnter = new List<UIScreenId>();
    public List<UIScreenId> ScreensToHideOnEnter = new List<UIScreenId>();
    public List<UIScreenId> ScreensToShowOnExit = new List<UIScreenId>();
    public List<UIScreenId> ScreensToHideOnExit = new List<UIScreenId>();

    public UIState(StateMachine parent, UIManager uiManager) : base(parent) => _uIManager = uiManager;

    protected override void OnEnter()
    {
        foreach (UIScreenId screen in ScreensToShowOnEnter)
        {
            _uIManager.Show(screen);
        }
        foreach (UIScreenId screen in ScreensToHideOnEnter)
        {
            _uIManager.Hide(screen);
        }
    }

    protected override void OnExit()
    {
        foreach (UIScreenId screen in ScreensToShowOnExit)
        {
            _uIManager.Show(screen);
        }
        foreach (UIScreenId screen in ScreensToHideOnExit)
        {
            _uIManager.Hide(screen);
        }
    }
}
