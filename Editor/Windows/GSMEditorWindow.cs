using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor.UIElements;
using System.Collections.Generic;
using Vocario.EventBasedArchitecture.EventFlowStateMachine.Editor;
using Vocario.EventBasedArchitecture.EventFlowStateMachine;

public class GSMEditorWindow : EditorWindow
{
    public const string ASSET_PATH = "Packages/com.vocario.gamestatemanager/Editor/Resources/GSMEditorWindow.uxml";
    private EventFlowStateMachine _stateManager = null;
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
        _stateManager = stateManager;

        _graphView = root.Q<GSMGraphView>();

        var eventInfo = new List<EventInfo>();
        foreach (KeyValuePair<int, Vocario.EventBasedArchitecture.GameEvent> item in stateManager.Events)
        {
            eventInfo.Add(new EventInfo() { EnumId = item.Key, Name = item.Value.Name });
        }
        var nodeController = new NodeController(_stateManager, _stateManager.GraphViewData);
        var portController = new PortController(_stateManager, _stateManager.GraphViewData);
        var stateBehaviourController = new StateBehaviourController(_stateManager, _stateManager.GraphViewData);
        var edgeController = new EdgeController(_stateManager, _stateManager.GraphViewData);

        var dependencies = new GraphViewDependencies(eventInfo,
                                                    nodeController,
                                                    portController,
                                                    stateBehaviourController,
                                                    edgeController);
        _graphView.Init(dependencies);

        _saveButton = root.Q<ToolbarButton>();
        _saveButton.clickable.clicked += SaveChanges;
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ASSET_PATH);
        visualTree.CloneTree(root);
    }

    public override void SaveChanges() => base.SaveChanges();
}
