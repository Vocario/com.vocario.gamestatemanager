using UnityEditor;
using System;
using UnityEngine.UIElements;

public class GSMEditorWindow : EditorWindow
{
    public StateMachineGraph StateManager;

    public static void Open(StateMachineGraph stateManager)
    {
        GSMEditorWindow editorWindow = GetWindow<GSMEditorWindow>("Game State Manager");
        editorWindow.StateManager = stateManager;
    }

    private GSMGraphView _graphView = null;

    private void OnEnable() => AddGraphView();// AddToolbar();// AddStyles();

    private void AddGraphView()
    {
        _graphView = new GSMGraphView(this);
        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }

    private void AddToolbar() => throw new NotImplementedException();
    private void AddStyles() => throw new NotImplementedException();
}
