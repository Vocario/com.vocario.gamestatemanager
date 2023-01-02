using UnityEditor;
using System;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor.UIElements;
using System.Collections.Generic;

public class GSMEditorWindow : EditorWindow
{
    public EventFlowStateMachine StateManager => _stateManager;
    public bool HasUnsavedChanges { get => hasUnsavedChanges; set => hasUnsavedChanges = value; }
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
        _graphView.Init(this);

        _saveButton = root.Q<ToolbarButton>();
        _saveButton.clickable.clicked += SaveChanges;
    }

    public override void SaveChanges()
    {
        base.SaveChanges();
        foreach (KeyValuePair<Guid, GSMNode> value in _graphView.Nodes)
        {
            Guid id = value.Key;
            GSMNode node = value.Value;
            Rect position = node.GetPosition();
            _stateManager.Metadata.UpdateNodeMetadata(id, null, node.name, position.x, position.y);
            EditorUtility.SetDirty(_stateManager);
            AssetDatabase.SaveAssets();
        }
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.vocario.gamestatemanager/Editor/Resources/GSMEditorWindow.uxml");

        visualTree.CloneTree(root);
    }
}
