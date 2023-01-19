using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor.UIElements;
using System.Collections.Generic;

public class GSMEditorWindow : EditorWindow
{
    public EventFlowStateMachine StateManager { get; private set; } = null;
    public bool HasUnsavedChanges { get => hasUnsavedChanges; set => hasUnsavedChanges = value; }
    private GSMGraphView _graphView = null;
    private ToolbarButton _saveButton = null;

    public static void Open(EventFlowStateMachine stateManager)
    {
        GSMEditorWindow editorWindow = GetWindow<GSMEditorWindow>("Event Flow State Manager");
        editorWindow.minSize = new Vector2(800, 600);
        editorWindow.Init(stateManager);
    }

    private void Init(EventFlowStateMachine stateManager)
    {
        VisualElement root = rootVisualElement;
        StateManager = stateManager;

        _graphView = root.Q<GSMGraphView>();

        var something = new List<EventInfo>();
        foreach (KeyValuePair<int, Vocario.EventBasedArchitecture.GameEvent> item in StateManager.Events)
        {
            something.Add(new EventInfo() { EnumId = item.Key, Name = item.Value.Name });
        }

        var dependencies = new GraphViewDependencies()
        {
            EventInfo = something
        };
        _graphView.Init(dependencies);

        _saveButton = root.Q<ToolbarButton>();
        _saveButton.clickable.clicked += SaveChanges;
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.vocario.gamestatemanager/Editor/Resources/GSMEditorWindow.uxml");
        visualTree.CloneTree(root);
    }

    public class GraphViewDependencies
    {
        public List<EventInfo> EventInfo;
    }

    public class EventInfo
    {
        public int EnumId = -1;
        public string Name = "Invalid";
    }
}
