using UnityEditor;
using System;
using UnityEngine.UIElements;

public class GSMEditorWindow : EditorWindow
{
    public EventFlowStateMachine StateManager { get; private set; }

    public static void Open(EventFlowStateMachine stateManager)
    {
        GSMEditorWindow editorWindow = GetWindow<GSMEditorWindow>("Event Flow State Manager");
        editorWindow.StateManager = stateManager;
        editorWindow.Init();
    }

    private GSMGraphView _graphView = null;

    public void Init() => AddGraphView();// AddToolbar();// AddStyles();

    private void AddGraphView()
    {
        _graphView = new GSMGraphView(this);
        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }

    private void AddToolbar() => throw new NotImplementedException();
    private void AddStyles() => throw new NotImplementedException();
}
