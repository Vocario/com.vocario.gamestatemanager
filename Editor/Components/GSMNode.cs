using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Vocario.EventBasedArchitecture;
using UnityEditor;

public class GSMNode : Node
{
    public readonly Rect INITIAL_NODE_POSITION = new Rect(100, 200, 100, 150);

    private GSMEditorWindow _graphWindow;
    private GSMNodeDetails _nodeDetails;
    private Label _titleLabel;
    private TextField _nodeName;
    private NodeNameChangeButton _nodeNameButton;

    public Guid ID { get; private set; }
    public string DialogueName { get; private set; }
    public bool IsInitial { get; private set; }
    public Port InputPort { get; protected set; } = null;

    public delegate void OnNodeNameChange();

    public virtual void Init(NodeEditorMetadata nodeData, GSMEditorWindow graphWindow)
    {
        ID = nodeData.ID;
        DialogueName = nodeData.Name;
        IsInitial = nodeData.IsInitial;
        name = nodeData.Name;

        _graphWindow = graphWindow;
        _nodeDetails = new GSMNodeDetails();
        // TODO Change to render through polimorphism
        if (IsInitial)
        {
            SetPosition(INITIAL_NODE_POSITION);
            RenderAsInitial();
        }
        else
        {
            SetPosition(new Rect(nodeData.X, nodeData.Y, 0.0f, 0.0f));
            Render();
        }
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        Vector2 nodePosition = this.ChangeCoordinatesTo(contentContainer, evt.localMousePosition);

        evt.menu.AppendAction("Add event", a => AddEventOutput());
    }

    public Port AddEventOutput(GameEvent gameEvent = null)
    {
        Port outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(PortSource<Transition>));
        Label portLabel = outputPort.contentContainer.Q<Label>("type");

        portLabel.text = "";
        var objectField = new ObjectField
        {
            objectType = typeof(GameEvent)
        };
        objectField.style.width = 150.0f;
        outputPort.contentContainer.Add(objectField);
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
        _graphWindow.HasUnsavedChanges = true;
    }
}

public class GSMNodeDetails : VisualElement
{
    private Button _addButton = null;
    private GameEventSearchWindow _searchWindow = null;

    public new class UxmlFactory : UxmlFactory<GSMNodeDetails, VisualElement.UxmlTraits> { }

    public GSMNodeDetails() => CreateGUI();

    protected void CreateGUI()
    {
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.vocario.gamestatemanager/Editor/Resources/GSMNodeDetails.uxml");
        visualTree.CloneTree(this);
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
            _searchWindow = ScriptableObject.CreateInstance<GameEventSearchWindow>();
        }
        var windowContext = new SearchWindowContext(Event.current.mousePosition);
        _ = SearchWindow.Open(windowContext, _searchWindow);
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
