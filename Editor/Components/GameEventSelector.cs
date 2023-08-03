using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

public class GameEventSelector : VisualElement
{
    public const string DEFAULT_LABEL = "None (Game Event)";
    public const string ASSET_PATH = "Packages/com.vocario.gamestatemanager/Editor/Resources/GameEventSelector.uxml";
    private Button _addButton = null;
    private VisualElement _imageElement = null;
    private Label _label = null;
    private GameEventSearchWindow _searchWindow = null;
    private Guid _nodeId;
    private Action<Guid, Guid, string> _onCreate;
    private Action<Guid, Guid, string> _onUpdate;
    private Guid _id;
    public Guid Id => _id;

    public GameEventSelector(Guid? id,
                            string name,
                            Guid nodeId,
                            Action<Guid, Guid, string> onCreate,
                            Action<Guid, Guid, string> onUpdate) : base()
    {
        _id = id ?? Guid.NewGuid();
        _nodeId = nodeId;
        _onCreate = onCreate;
        _onUpdate = onUpdate;
        if (!id.HasValue)
        {
            _onCreate?.Invoke(_id, nodeId, "");
        }

        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ASSET_PATH);

        visualTree.CloneTree(this);
        _addButton = this.Q<Button>("game-event-field-selector");
        _imageElement = this.Q<VisualElement>("image");
        _label = this.Q<Label>("label");
        style.width = 150.0f;
        _label.text = DEFAULT_LABEL;
        _addButton.clicked += OpenGameEventSearchWindow;
        if (name != null)
        {
            UpdateValueWithoutNotify(name);
        }
    }

    ~GameEventSelector()
    {
        _addButton.clicked -= OpenGameEventSearchWindow;
    }

    private void UpdateValueWithoutNotify(string value)
    {
        if (value == "")
        {
            _imageElement.style.backgroundImage = null;
            _label.text = DEFAULT_LABEL;
            return;
        }

        _imageElement.style.backgroundImage = EditorGUIUtility.FindTexture("d_cs Script Icon");
        _label.text = value;
    }

    private void UpdateValue(string value)
    {
        UpdateValueWithoutNotify(value);
        _onUpdate?.Invoke(_id, _nodeId, value);

    }

    private void OpenGameEventSearchWindow()
    {
        if (_searchWindow == null)
        {
            _searchWindow = ScriptableObject.CreateInstance<GameEventSearchWindow>();
        }
        _searchWindow.Init(UpdateValue);
        var windowContext = new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition));
        _ = SearchWindow.Open(windowContext, _searchWindow);
    }
}
