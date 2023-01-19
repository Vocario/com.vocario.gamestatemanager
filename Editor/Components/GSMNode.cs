using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System;
using UnityEngine.UIElements;
using UnityEditor;
using System.Collections.Generic;

public class GSMNode : Node
{
    public readonly Rect INITIAL_NODE_POSITION = new Rect(100, 200, 100, 150);
    private List<GSMEditorWindow.EventInfo> _eventInfo;
    private GSMNodeDetails _nodeDetails;
    private Label _titleLabel;
    private TextField _nodeName;
    private NodeNameChangeButton _nodeNameButton;

    public Guid ID { get; private set; }
    public string DialogueName { get; private set; }
    public bool IsInitial { get; private set; }
    public Port InputPort { get; protected set; } = null;

    public virtual void Init(string name, float x, float y, bool isInitial, List<GSMEditorWindow.EventInfo> eventInfo)
    {
        ID = Guid.NewGuid();
        title = name;
        _eventInfo = eventInfo;
        _nodeDetails = new GSMNodeDetails();
        // TODO Change to render through polimorphism
        if (isInitial)
        {
            SetPosition(INITIAL_NODE_POSITION);
            RenderAsInitial();
        }
        else
        {
            SetPosition(new Rect(x, y, 0.0f, 0.0f));
            Render();
        }
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        Vector2 nodePosition = this.ChangeCoordinatesTo(contentContainer, evt.localMousePosition);

        evt.menu.AppendAction("Add event", a => CreateEventOutput());
    }

    public Port CreateEventOutput()
    {
        Port outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(PortSource<Transition>));
        Label portLabel = outputPort.contentContainer.Q<Label>("type");

        portLabel.text = "";
        var gameEventSelector = new GameEventSelector(_eventInfo);
        outputPort.contentContainer.Add(gameEventSelector);
        var deleteButton = new Button(() => outputContainer.Remove(outputPort))
        {
            text = "X"
        };
        outputPort.contentContainer.Add(deleteButton);
        outputContainer.Add(outputPort);
        RefreshExpandedState();
        _ = RefreshPorts();
        return outputPort;
    }

    private void RenderAsInitial()
    {
        InitializeElements();
        extensionContainer.Add(_nodeDetails);
        title = name;
        capabilities &= ~Capabilities.Movable;
        capabilities &= ~Capabilities.Deletable;
        RefreshExpandedState();
        _ = RefreshPorts();
    }

    public void Render()
    {
        InitializeElements();
        extensionContainer.Add(_nodeDetails);
        title = name;
        InputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(PortSource<Transition>));
        InputPort.portName = "In";
        inputContainer.Add(InputPort);
        RefreshExpandedState();
        _ = RefreshPorts();
    }

    private void InitializeElements()
    {
        _titleLabel = this.Q<Label>("title-label");
        _nodeName = new TextField();
        _nodeName.SetValueWithoutNotify(name);
        _nodeName.style.paddingBottom = 5;
        _nodeName.style.paddingTop = 5;
        _nodeName.Q("unity-text-input").style.paddingRight = 5;
        _nodeName.Q("unity-text-input").style.paddingLeft = 5;
        _nodeNameButton = new NodeNameChangeButton();
        _nodeNameButton.OnEdit += OnEditName;
        _nodeNameButton.OnSave += OnSaveName;
        titleButtonContainer.Add(_nodeNameButton);
    }

    private void OnEditName()
    {
        titleContainer.Insert(0, _nodeName);
        titleContainer.Remove(_titleLabel);
    }

    private void OnSaveName()
    {
        titleContainer.Insert(0, _titleLabel);
        titleContainer.Remove(_nodeName);
        title = _nodeName.text;
        name = title;
    }
}

public class GSMNodeDetails : VisualElement
{
    private Button _addButton = null;
    private VisualElement _container = null;
    private StateBehaviourSearchWindow _searchWindow = null;

    public new class UxmlFactory : UxmlFactory<GSMNodeDetails, VisualElement.UxmlTraits> { }

    public GSMNodeDetails() => CreateGUI();

    protected void CreateGUI()
    {
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.vocario.gamestatemanager/Editor/Resources/GSMNodeDetails.uxml");
        visualTree.CloneTree(this);
        _container = this.Q<VisualElement>("behaviour-list");
        _addButton = this.Q<Button>("add-button");
        _addButton.clickable.clicked += OpenStateBehaviourSearchWindow;
    }

    ~GSMNodeDetails()
    {
        _addButton.clickable.clicked -= OpenStateBehaviourSearchWindow;
    }

    private void OpenStateBehaviourSearchWindow()
    {
        if (_searchWindow == null)
        {
            _searchWindow = ScriptableObject.CreateInstance<StateBehaviourSearchWindow>();
        }
        _searchWindow.Init(AddNewItem);
        var windowContext = new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition));
        _ = SearchWindow.Open(windowContext, _searchWindow);
    }

    private void AddNewItem(string name)
    {
        var newItem = new StateBehaviourItem(name, RemoveItem);
        _container.Add(newItem);
    }

    private void RemoveItem(StateBehaviourItem item) => _container.Remove(item);
}

public class GameEventSelector : VisualElement
{
    public const string DEFAULT_LABEL = "None (Game Event)";
    private List<GSMEditorWindow.EventInfo> _eventInfo = null;
    private Button _addButton = null;
    private VisualElement _imageElement = null;
    private Label _label = null;
    private GameEventSearchWindow _searchWindow = null;

    public GameEventSelector(List<GSMEditorWindow.EventInfo> eventInfo) : base()
    {
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.vocario.gamestatemanager/Editor/Resources/GameEventSelector.uxml");

        visualTree.CloneTree(this);
        _eventInfo = eventInfo;
        _addButton = this.Q<Button>("game-event-field-selector");
        _imageElement = this.Q<VisualElement>("image");
        _label = this.Q<Label>("label");
        style.width = 150.0f;
        _label.text = DEFAULT_LABEL;
        _addButton.clicked += OpenGameEventSearchWindow;
    }

    ~GameEventSelector()
    {
        _addButton.clicked -= OpenGameEventSearchWindow;
    }

    private void UpdateValue(int value)
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

public class StateBehaviourItem : VisualElement
{
    private Button _removeButton = null;
    private Label _label = null;

    public StateBehaviourItem(string name, Action<StateBehaviourItem> removeItem) : base()
    {
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.vocario.gamestatemanager/Editor/Resources/NodeBehaviourItem.uxml");

        visualTree.CloneTree(this);
        _removeButton = this.Q<Button>("remove-button");
        _label = this.Q<Label>("name");

        _label.text = name;
        _removeButton.clicked += () => removeItem(this);
    }
}

public class NodeNameChangeButton : Button
{
    private bool _isEditing = false;
    public event Action OnEdit;
    public event Action OnSave;

    public NodeNameChangeButton() : base()
    {
        text = "Edit Name";
        clicked += OnClicked;
    }

    private void OnClicked()
    {
        if (_isEditing)
        {
            text = "Edit Name";
            OnSave?.Invoke();
        }
        else
        {
            text = "Save Name";
            OnEdit?.Invoke();
        }
        _isEditing = !_isEditing;
    }
}
