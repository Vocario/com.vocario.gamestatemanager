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
    private List<EventInfo> _eventInfo = null;
    private Button _addButton = null;
    private VisualElement _imageElement = null;
    private Label _label = null;
    private GameEventSearchWindow _searchWindow = null;
    private Guid _nodeId;
    private Action<Guid, Guid, int> _onCreate;
    private Action<Guid, Guid, int> _onUpdate;
    private Guid _id;
    public Guid Id => _id;

    public GameEventSelector(Guid? id,
                            int? index,
                            Guid nodeId,
                            List<EventInfo> eventInfo,
                            Action<Guid, Guid, int> onCreate,
                            Action<Guid, Guid, int> onUpdate) : base()
    {
        _id = id ?? Guid.NewGuid();
        _nodeId = nodeId;
        _onCreate = onCreate;
        _onUpdate = onUpdate;
        if (!id.HasValue)
        {
            _onCreate?.Invoke(_id, nodeId, -1);
        }

        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ASSET_PATH);

        visualTree.CloneTree(this);
        _eventInfo = eventInfo;
        _addButton = this.Q<Button>("game-event-field-selector");
        _imageElement = this.Q<VisualElement>("image");
        _label = this.Q<Label>("label");
        style.width = 150.0f;
        _label.text = DEFAULT_LABEL;
        _addButton.clicked += OpenGameEventSearchWindow;
        if (index.HasValue)
        {
            UpdateValueWithoutNotify(index.Value);
        }
    }

    ~GameEventSelector()
    {
        _addButton.clicked -= OpenGameEventSearchWindow;
    }

    private void UpdateValueWithoutNotify(int value)
    {
        if (value < 0)
        {
            _imageElement.style.backgroundImage = null;
            _label.text = DEFAULT_LABEL;
            return;
        }

        _imageElement.style.backgroundImage = EditorGUIUtility.FindTexture("d_cs Script Icon");
        _label.text = _eventInfo[ value ].Name;
    }

    private void UpdateValue(int value)
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
        _searchWindow.Init(_eventInfo, UpdateValue);
        var windowContext = new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition));
        _ = SearchWindow.Open(windowContext, _searchWindow);
    }
}
