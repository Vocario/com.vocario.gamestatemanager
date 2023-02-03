using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor.UIElements;
using System.Collections.Generic;
using Vocario.EventBasedArchitecture.EventFlowStateMachine.Editor;

public class GSMEditorWindow : EditorWindow
{
    private EventFlowStateMachine _stateManager = null;
    private GSMGraphView _graphView = null;
    private ToolbarButton _saveButton = null;
    private ChangeManager _changeManager = null;

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
        _changeManager = new ChangeManager(() => hasUnsavedChanges = true, () => hasUnsavedChanges = false);
        var nodeController = new NodeController(_changeManager, _stateManager.NodeData);
        var portController = new PortController(_changeManager, _stateManager.NodeData);
        var stateBehaviourController = new StateBehaviourController(_changeManager, _stateManager.NodeData);

        var dependencies = new GraphViewDependencies(eventInfo, nodeController, portController, stateBehaviourController);
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

    public override void SaveChanges()
    {
        base.SaveChanges();
        _changeManager.Commit();
    }
}

public class GraphViewDependencies
{
    public List<EventInfo> EventInfo { get; private set; }
    public NodeController NodeController { get; private set; }
    public PortController PortController { get; private set; }
    public StateBehaviourController StateBehaviourController { get; private set; }

    public GraphViewDependencies(List<EventInfo> eventInfo, NodeController nodeController, PortController portController, StateBehaviourController stateBehaviourController)
    {
        EventInfo = eventInfo;
        NodeController = nodeController;
        PortController = portController;
        StateBehaviourController = stateBehaviourController;
    }
}

public class EventInfo
{
    public int EnumId = -1;
    public string Name = "Invalid";
}

public class ChangeManager
{
    // TODO Change to delegate
    private System.Action _onChangeAdded;
    private System.Action _onChangesCommited;
    private Queue<APendingChanges> _changesQueue = new Queue<APendingChanges>();

    internal ChangeManager(System.Action OnChangeAdded, System.Action OnChangesCommited)
    {
        _onChangeAdded = OnChangeAdded;
        _onChangesCommited = OnChangesCommited;
    }

    internal void Commit()
    {
        while (_changesQueue.Count > 0)
        {
            APendingChanges change = _changesQueue.Dequeue();
            change.Commit();
        }
        _onChangesCommited?.Invoke();
    }

    internal void AddChange(APendingChanges change)
    {
        _changesQueue.Enqueue(change);
        _onChangeAdded?.Invoke();
    }
}

namespace Vocario.EventBasedArchitecture.EventFlowStateMachine.Editor
{
    using Model;
    using System;

    public class NodeController
    {
        private ChangeManager _changeManager = null;
        private List<Node> _nodeData = null;

        public NodeController(ChangeManager changeManager, List<Node> nodeData)
        {
            _changeManager = changeManager;
            _nodeData = nodeData;
        }

        internal void Create(Guid id, string name, float x, float y, bool isInitial)
        {
            var change = new CreateNodePendingChanges()
            {
                SavedData = _nodeData,
                Data = new Node()
                {
                    GraphId = id,
                    Name = name,
                    X = x,
                    Y = y,
                    IsInitial = isInitial
                },
            };
            _changeManager.AddChange(change);
        }

        internal void Update(Guid id, string name, float x, float y, bool isInitial)
        {
            var change = new UpdateNodePendingChanges()
            {
                SavedData = _nodeData,
                Id = id,
                Name = name,
                X = x,
                Y = y,
                IsInitial = isInitial
            };
            _changeManager.AddChange(change);
        }

        internal void Remove(Guid id)
        {
            var change = new RemoveNodePendingChanges()
            {
                SavedData = _nodeData,
                Id = id
            };
            _changeManager.AddChange(change);
        }

        internal Node[] GetAll() => _nodeData.ToArray();
    }

    public class PortController
    {
        private ChangeManager _changeManager = null;
        private List<Node> _nodeData = null;

        public PortController(ChangeManager changeManager, List<Node> nodeData)
        {
            _changeManager = changeManager;
            _nodeData = nodeData;
        }

        internal void Create(Guid id, Guid nodeId, int index)
        {
            var change = new CreatePortPendingChanges()
            {
                NodeId = nodeId,
                SavedData = _nodeData,
                Data = new Port()
                {
                    ID = id,
                    Index = index
                },
            };
            _changeManager.AddChange(change);
        }

        internal void Update(Guid id, Guid nodeId, int index)
        {
            var change = new UpdatePortPendingChanges()
            {
                SavedData = _nodeData,
                Id = id,
                NodeId = nodeId,
                Index = index
            };
            _changeManager.AddChange(change);
        }

        internal void Remove(Guid id, Guid nodeId)
        {
            var change = new RemovePortPendingChanges()
            {
                SavedData = _nodeData,
                Id = id,
                NodeId = nodeId
            };
            _changeManager.AddChange(change);
        }
    }

    public class StateBehaviourController
    {
        private ChangeManager _changeManager = null;
        private List<Node> _nodeData = null;

        public StateBehaviourController(ChangeManager changeManager, List<Node> nodeData)
        {
            _changeManager = changeManager;
            _nodeData = nodeData;
        }

        internal void Create(Guid nodeId, string typeName)
        {
            var change = new CreateStateBehaviourPendingChanges()
            {
                NodeId = nodeId,
                SavedData = _nodeData,
                StateBehaviourType = typeName
            };
            _changeManager.AddChange(change);
        }

        internal void Remove(Guid nodeId, string typeName)
        {
            var change = new RemoveStateBehaviourPendingChanges()
            {
                SavedData = _nodeData,
                NodeId = nodeId,
                StateBehaviourType = typeName
            };
            _changeManager.AddChange(change);
        }

        internal List<string> GetList(Guid nodeId)
        {
            Node nodeData = _nodeData.Find(node => node.GraphId == nodeId);
            return nodeData?.StateBehaviourTypes;
        }
    }
}
