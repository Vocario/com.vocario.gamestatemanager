using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System;
using UnityEngine.UIElements;
using UnityEditor;
using System.Collections.Generic;
using PortModel = Vocario.EventBasedArchitecture.EventFlowStateMachine.Editor.Model.Port;
using Vocario.GameStateManager;

public class GSMNode : Node
{
    public readonly Rect INITIAL_NODE_POSITION = new Rect(100, 200, 100, 150);
    private List<EventInfo> _eventInfo;
    private GraphViewDependencies _dependencies;
    private GSMNodeDetails _nodeDetails;
    private Label _titleLabel;
    private TextField _nodeName;
    private NodeNameChangeButton _nodeNameButton;
    public Dictionary<Port, Guid> GameSelectors { get; private set; } = new Dictionary<Port, Guid>();
    public Dictionary<Guid, Port> Ports { get; private set; } = new Dictionary<Guid, Port>();

    public Guid ID { get; private set; }
    public string DialogueName { get; private set; }
    public bool IsInitial { get; private set; }
    public Port InputPort { get; protected set; } = null;

    public virtual void Init(Guid? id,
                            string name,
                            float x,
                            float y,
                            bool isInitial,
                            List<EventInfo> eventInfo,
                            List<PortModel> ports,
                            GraphViewDependencies dependencies)
    {
        ID = id ?? Guid.NewGuid();
        title = name;
        this.name = name;
        _eventInfo = eventInfo;
        _dependencies = dependencies;
        _nodeDetails = new GSMNodeDetails();
        _nodeDetails.Init(ID,
                        _dependencies.StateBehaviourController.Create,
                        _dependencies.StateBehaviourController.Remove,
                        _dependencies.StateBehaviourController.GetElements(ID));
        foreach (PortModel port in ports)
        {
            _ = CreateEventOutput(port.ID, port.Index);
        }
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

    // TODO Remove code repetition
    public Port CreateEventOutput()
    {
        Port outputPort = InstantiatePort(Orientation.Horizontal,
                                        Direction.Output,
                                        Port.Capacity.Single,
                                        typeof(PortSource<Transition>));
        Label portLabel = outputPort.contentContainer.Q<Label>("type");

        portLabel.text = "";
        var gameEventSelector = new GameEventSelector(null,
                                                    null,
                                                    ID,
                                                    _eventInfo,
                                                    _dependencies.PortController.Create,
                                                    _dependencies.PortController.Update);
        outputPort.contentContainer.Add(gameEventSelector);
        var deleteButton = new Button(() => RemovePort(gameEventSelector.Id, outputPort))
        {
            text = "X"
        };
        outputPort.contentContainer.Add(deleteButton);
        outputContainer.Add(outputPort);
        RefreshExpandedState();
        _ = RefreshPorts();
        GameSelectors[ outputPort ] = gameEventSelector.Id;
        Ports[ gameEventSelector.Id ] = outputPort;
        return outputPort;
    }

    public Port CreateEventOutput(Guid id, int index)
    {
        Port outputPort = InstantiatePort(Orientation.Horizontal,
                                        Direction.Output,
                                        Port.Capacity.Single,
                                        typeof(PortSource<Transition>));
        Label portLabel = outputPort.contentContainer.Q<Label>("type");

        portLabel.text = "";
        var gameEventSelector = new GameEventSelector(id,
                                                    index,
                                                    ID,
                                                    _eventInfo,
                                                    _dependencies.PortController.Create,
                                                    _dependencies.PortController.Update);
        outputPort.contentContainer.Add(gameEventSelector);
        var deleteButton = new Button(() => RemovePort(gameEventSelector.Id, outputPort))
        {
            text = "X"
        };
        outputPort.contentContainer.Add(deleteButton);
        outputContainer.Add(outputPort);
        RefreshExpandedState();
        _ = RefreshPorts();
        GameSelectors[ outputPort ] = gameEventSelector.Id;
        Ports[ gameEventSelector.Id ] = outputPort;
        return outputPort;
    }

    public void RemovePort(Guid selectorId, Port outputPort)
    {
        if (outputPort.connected)
        {
            foreach (Edge edge in outputPort.connections)
            {
                edge.parent.Remove(edge);
                if (edge.input.node is GSMNode node)
                {
                    _dependencies.EdgeController.Remove(selectorId, node.ID);
                }
            }
            outputPort.DisconnectAll();
        }
        outputContainer.Remove(outputPort);
        _dependencies.PortController.Remove(selectorId, ID);
        RefreshExpandedState();
        _ = RefreshPorts();
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
        InputPort = InstantiatePort(Orientation.Horizontal,
                                    Direction.Input,
                                    Port.Capacity.Multi,
                                    typeof(PortSource<Transition>));
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
        Rect position = GetPosition();
        _dependencies.NodeController.Update(ID, name, position.x, position.y, IsInitial);
    }
}
