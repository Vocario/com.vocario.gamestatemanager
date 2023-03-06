using Vocario.EventBasedArchitecture;
using UnityEngine;
using System;
using System.Collections.Generic;
using Vocario.EventBasedArchitecture.EventFlowStateMachine.Editor.Model;
using Vocario.GameStateManager;

[CreateAssetMenu(fileName = "EventFlowStateMachine_", menuName = "Vocario/EventFlowStateMachine", order = 0)]
public class EventFlowStateMachine : StateMachine
{
    [SerializeField]
    private bool _initialized = false;

    private void Awake()
    {
        if (_initialized)
        {
            return;
        }

        NodeData = new List<Node>();
        EdgeData = new List<Edge>();

        State state = CreateState<State>();
        NodeData.Add(new Node()
        {
            GraphId = Guid.NewGuid(),
            IsInitial = true,
            Name = "Start",
            StateId = state.Id
        });
        _initialized = true;
    }


#if UNITY_EDITOR
    public List<Node> NodeData;
    // TODO Maybe change the edge data to per game event selector
    public List<Edge> EdgeData;
#endif
}

public class CreateNodePendingChanges : APendingChanges
{
    public StateMachine Context = null;
    public Node Data = null;
    public List<Node> SavedData = null;

    public override void Commit()
    {
        State state = Context.CreateState<State>();
        Data.StateId = state.Id;
        SavedData.Add(Data);
    }
}

public class UpdateNodePendingChanges : APendingChanges
{
    public Guid Id;
    public string Name;
    public float X;
    public float Y;
    public bool IsInitial;
    public List<Node> SavedData = null;

    public override void Commit()
    {
        Node nodeData = SavedData.Find(SearchClause);
        if (nodeData != null)
        {
            nodeData.Name = Name;
            nodeData.X = X;
            nodeData.Y = Y;
            nodeData.IsInitial = IsInitial;
        }
    }

    private bool SearchClause(Node node) => node.GraphId == Id;
}

public class RemoveNodePendingChanges : APendingChanges
{
    public StateMachine Context = null;
    public Guid Id;
    public List<Node> SavedData = null;

    public override void Commit()
    {
        Node nodeData = SavedData.Find(SearchClause);
        if (nodeData != null)
        {
            _ = SavedData.Remove(nodeData);
            Context.DeleteState(nodeData.StateId);
        }
    }

    private bool SearchClause(Node node) => node.GraphId == Id;
}

public class CreateStateBehaviourPendingChanges : APendingChanges
{
    public Guid NodeId;
    public List<Node> SavedData;
    public string StateBehaviourType = "";

    public override void Commit()
    {
        Node nodeData = SavedData.Find(SearchClause);
        nodeData?.StateBehaviourTypes.Add(StateBehaviourType);
    }

    private bool SearchClause(Node node) => node.GraphId == NodeId;
}

public class RemoveStateBehaviourPendingChanges : APendingChanges
{
    public Guid NodeId;
    public List<Node> SavedData;
    public string StateBehaviourType = "";

    public override void Commit()
    {
        Node nodeData = SavedData.Find(SearchClause);
        _ = (nodeData?.StateBehaviourTypes.Remove(StateBehaviourType));
    }

    private bool SearchClause(Node node) => node.GraphId == NodeId;
}

public class CreatePortPendingChanges : APendingChanges
{
    public Guid NodeId;
    public List<Node> SavedData = null;
    public Port Data = null;

    public override void Commit()
    {
        Node nodeData = SavedData.Find(SearchClause);
        nodeData.Ports.Add(Data);
    }

    private bool SearchClause(Node node) => node.GraphId == NodeId;
}

public class UpdatePortPendingChanges : APendingChanges
{
    public List<Node> SavedData = null;
    public Guid Id;
    public Guid NodeId;
    public int Index = -1;

    public override void Commit()
    {
        Node nodeData = SavedData.Find(NodeSearchClause);
        if (nodeData == null)
        {
            return;
        }

        Port portData = nodeData.Ports.Find(PortSearchClause);
        if (portData == null)
        {
            return;
        }
        portData.Index = Index;
    }

    private bool NodeSearchClause(Node node) => node.GraphId == NodeId;
    private bool PortSearchClause(Port port) => port.ID == Id;
}

public class RemovePortPendingChanges : APendingChanges
{
    public List<Node> SavedData = null;
    public Guid Id;
    public Guid NodeId;

    public override void Commit()
    {
        Node nodeData = SavedData.Find(NodeSearchClause);
        if (nodeData == null)
        {
            return;
        }

        Port portData = nodeData.Ports.Find(PortSearchClause);
        if (portData == null)
        {
            return;
        }
        _ = nodeData.Ports.Remove(portData);
    }

    private bool NodeSearchClause(Node node) => node.GraphId == NodeId;
    private bool PortSearchClause(Port port) => port.ID == Id;
}

public class CreateEdgePendingChanges : APendingChanges
{
    public StateMachine Context = null;
    public List<Node> NodeData = null;
    public List<Edge> EdgeData = null;
    public Guid InNodeId;
    public Guid OutNodeId;
    public Guid OutPortId;

    public override void Commit()
    {
        var edge = new Edge()
        {
            InNodeId = InNodeId,
            OutNodeId = OutNodeId,
            OutPortId = OutPortId,
        };
        EdgeData.Add(edge);
        Node inNode = NodeData.Find(InNodeSearchClause);
        Node outNode = NodeData.Find(OutNodeSearchClause);
        Port port = outNode.Ports.Find(PortSearchClause);
        Debug.Log($"inNode {inNode}, outNode {outNode}, port {port}");
        _ = Context.CreateTransition(port.Index, outNode.StateId, inNode.StateId);
    }

    private bool InNodeSearchClause(Node node) => node.GraphId == InNodeId;
    private bool OutNodeSearchClause(Node node) => node.GraphId == OutNodeId;

    private bool PortSearchClause(Port port) => port.ID == OutPortId;
}

public class RemoveEdgePendingChanges : APendingChanges
{
    public StateMachine Context = null;
    public List<Edge> EdgeData = null;
    public List<Node> NodeData = null;
    public Guid InNodeId;
    public Guid OutPortId;

    public override void Commit()
    {
        Node node = NodeData.Find(NodeSearchClause);
        if (node == null)
        {
            return;
        }
        Port port = node.Ports.Find(PortSearchClause);
        if (port == null)
        {
            return;
        }
        Context.DeleteTransition(port.Index, node.StateId);
        Edge edge = EdgeData.Find(EdgeSearchClause);
        if (edge != null)
        {
            _ = EdgeData.Remove(edge);
        }
    }

    private bool EdgeSearchClause(Edge edge) => edge.InNodeId == InNodeId && edge.OutPortId == OutPortId;
    private bool NodeSearchClause(Node node) => node.GraphId == InNodeId;
    private bool PortSearchClause(Port port) => port.ID == OutPortId;
}

public abstract class APendingChanges
{
    public abstract void Commit();
}

namespace Vocario.EventBasedArchitecture.EventFlowStateMachine.Editor.Model
{
    [Serializable]
    public class Node
    {
        [SerializeField]
        private string _graphId;
        public Guid GraphId
        {
            get => Guid.Parse(_graphId);
            set => _graphId = value.ToString();
        }
        [SerializeField]
        private string _stateId;
        public Guid StateId
        {
            get => Guid.Parse(_stateId);
            set => _stateId = value.ToString();
        }
        public bool IsInitial;
        public string Name;
        public float X;
        public float Y;
        public List<Port> Ports = new List<Port>();
        public List<string> StateBehaviourTypes = new List<string>();
    }

    [Serializable]
    public class Port
    {
        [SerializeField]
        private string _id;
        public Guid ID
        {
            get => Guid.Parse(_id);
            set => _id = value.ToString();
        }
        public string Name = "";
        public int Index = -1;
    }

    [Serializable]
    public class Edge
    {
        [SerializeField]
        private string _outPortId;
        public Guid OutPortId
        {
            get => Guid.Parse(_outPortId);
            set => _outPortId = value.ToString();
        }
        [SerializeField]
        private string _inNodeId;
        public Guid InNodeId
        {
            get => Guid.Parse(_inNodeId);
            set => _inNodeId = value.ToString();
        }
        [SerializeField]
        private string _outNodeId;
        public Guid OutNodeId
        {
            get => Guid.Parse(_outNodeId);
            set => _outNodeId = value.ToString();
        }
    }
}
