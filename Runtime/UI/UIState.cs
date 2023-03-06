using UnityEngine;
using System.Collections.Generic;
using System;
using Vocario.GameStateManager;

[Serializable]
public class UIState : State
{
    private UIManager _uIManager = null;
    public List<UIScreenId> ScreensToShowOnEnter = new List<UIScreenId>();
    public List<UIScreenId> ScreensToHideOnEnter = new List<UIScreenId>();
    public List<UIScreenId> ScreensToShowOnExit = new List<UIScreenId>();
    public List<UIScreenId> ScreensToHideOnExit = new List<UIScreenId>();

    public UIState(StateMachine parent) : base(parent) => _uIManager = null;

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
