using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor.UIElements;
using System.Collections.Generic;
using Vocario.EventBasedArchitecture.EventFlowStateMachine.Editor;
using Vocario.EventBasedArchitecture.EventFlowStateMachine;

public class GSMEditorWindow : EditorWindow
{
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

        var dependencies = new GraphViewDependencies(eventInfo, nodeController, portController, stateBehaviourController, edgeController);
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

    public override void SaveChanges() => base.SaveChanges();
}

public class GraphViewDependencies
{
    public List<EventInfo> EventInfo { get; private set; }
    public NodeController NodeController { get; private set; }
    public PortController PortController { get; private set; }
    public StateBehaviourController StateBehaviourController { get; private set; }
    public EdgeController EdgeController { get; private set; }

    public GraphViewDependencies(List<EventInfo> eventInfo, NodeController nodeController, PortController portController, StateBehaviourController stateBehaviourController, EdgeController edgeController)
    {
        EventInfo = eventInfo;
        NodeController = nodeController;
        PortController = portController;
        StateBehaviourController = stateBehaviourController;
        EdgeController = edgeController;
    }
}

public class EventInfo
{
    public int EnumId = -1;
    public string Name = "Invalid";
}

namespace Vocario.EventBasedArchitecture.EventFlowStateMachine.Editor
{
    using Model;
    using System;
    using System.Linq;

    using Vocario.GameStateManager;

    public class NodeController
    {
        private StateMachine _stateMachine = null;
        private GraphViewData _graphData = null;

        public NodeController(StateMachine stateMachine, GraphViewData graphData)
        {
            _stateMachine = stateMachine;
            _graphData = graphData;
        }

        internal void Create(Guid id, string name, float x, float y, bool isInitial)
        {
            Undo.RecordObject(_stateMachine, "Created new state through graph view");

            State newState = _stateMachine.CreateState<State>();
            _ = _graphData.CreateNode(id, newState.Id, name, x, y, isInitial);
        }

        internal void Update(Guid id, string name, float x, float y, bool isInitial)
        {
            Undo.RecordObject(_stateMachine, "Updated node through graph view");

            _graphData.UpdateNode(id, name, x, y, isInitial);
        }

        internal void Remove(Guid id)
        {
            Undo.RecordObject(_stateMachine, "Removed node through graph view");

            Node node = _graphData.RemoveNode(id);
            _stateMachine.DeleteState(node.StateId);
        }

        internal Node[] GetAll() => _graphData.GetNodes();
    }

    public class PortController
    {
        private StateMachine _stateMachine = null;
        private GraphViewData _graphData = null;

        public PortController(StateMachine stateMachine, GraphViewData graphViewData)
        {
            _stateMachine = stateMachine;
            _graphData = graphViewData;
        }

        internal void Create(Guid id, Guid nodeId, int index)
        {
            Undo.RecordObject(_stateMachine, "Created port for node through graph view");

            _graphData.CreatePort(id, nodeId, index);
        }

        internal void Update(Guid id, Guid nodeId, int index)
        {
            Undo.RecordObject(_stateMachine, "Updated port for node through graph view");

            _graphData.UpdatePort(id, nodeId, index);
        }

        internal void Remove(Guid id, Guid nodeId)
        {
            Undo.RecordObject(_stateMachine, "Removed port for node through graph view");

            _ = _graphData.RemovePort(id, nodeId);
        }
    }

    public class StateBehaviourController
    {
        private StateMachine _stateMachine = null;
        private GraphViewData _graphData = null;

        public StateBehaviourController(StateMachine stateMachine, GraphViewData graphViewData)
        {
            _stateMachine = stateMachine;
            _graphData = graphViewData;
        }

        internal VisualElement Create(Guid nodeId, string typeName)
        {
            Undo.RecordObject(_stateMachine, "Added state behaviour through graph view");

            int undoID = Undo.GetCurrentGroup();
            Node nodeData = _graphData.GetNode(nodeId);
            State state = _stateMachine.GetState(nodeData.StateId);
            AStateBehaviour stateBehaviour = state.AddStateBehaviour(typeName);

            stateBehaviour.name = Guid.NewGuid().ToString();
            stateBehaviour.hideFlags = HideFlags.HideInHierarchy;

            Undo.RegisterCreatedObjectUndo(stateBehaviour, "Added state behaviour through graph view");
            Undo.CollapseUndoOperations(undoID);

            AssetDatabase.AddObjectToAsset(stateBehaviour, _stateMachine);
            AssetDatabase.SaveAssets();

            return GetInspectorForStateBehaviour(stateBehaviour);
        }

        internal bool Remove(Guid nodeId, string typeName)
        {
            Undo.RecordObject(_stateMachine, "Removed state behaviour through graph view");

            Node nodeData = _graphData.GetNode(nodeId);
            State state = _stateMachine.GetState(nodeData.StateId);
            AStateBehaviour stateBehaviour = state.RemoveStateBehaviour(typeName);

            if (stateBehaviour == null)
            {
                return false;
            }
            AssetDatabase.RemoveObjectFromAsset(stateBehaviour);

            return stateBehaviour;
        }

        internal VisualElement[] GetElements(Guid nodeId)
        {
            Node nodeData = _graphData.GetNode(nodeId);
            if (nodeData == null)
            {
                return new VisualElement[] { };
            }
            State state = _stateMachine.GetState(nodeData.StateId);
            AStateBehaviour[] stateBehaviour = state.GetStateBehaviours();

            return stateBehaviour.Select(GetInspectorForStateBehaviour).ToArray();
        }


        private VisualElement GetInspectorForStateBehaviour(AStateBehaviour stateBehaviour)
        {
            var editor = UnityEditor.Editor.CreateEditor(stateBehaviour);

            return editor.CreateInspectorGUI();
        }
    }

    public class EdgeController
    {
        private StateMachine _stateMachine = null;
        private GraphViewData _graphData = null;

        public EdgeController(StateMachine stateMachine, GraphViewData graphViewData)
        {
            _stateMachine = stateMachine;
            _graphData = graphViewData;
        }

        internal void Create(Guid outPortId, Guid inNodeId, Guid outNodeId)
        {
            Undo.RecordObject(_stateMachine, "Added transition through graph view");

            _ = _graphData.CreateEdge(outPortId, inNodeId, outNodeId);

            Node outNode = _graphData.GetNode(outNodeId);
            Node inNode = _graphData.GetNode(inNodeId);
            Port port = _graphData.GetPort(outPortId, outNodeId);
            _ = _stateMachine.CreateTransition(port.Index, outNode.StateId, inNode.StateId);
        }

        internal void Remove(Guid outPortId, Guid outNodeId)
        {
            Undo.RecordObject(_stateMachine, "Removed transition through graph view");

            _ = _graphData.RemoveEdge(outPortId);

            Node node = _graphData.GetNode(outNodeId);
            Port port = _graphData.GetPort(outPortId, outNodeId);
            _stateMachine.DeleteTransition(port.Index, node.StateId);
        }

        internal Edge[] GetAll() => _graphData.GetEdges();
    }
}
