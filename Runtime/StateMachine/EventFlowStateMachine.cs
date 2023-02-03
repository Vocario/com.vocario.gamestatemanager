using Vocario.EventBasedArchitecture;
using UnityEngine;
using System;
using System.Collections.Generic;
using Vocario.EventBasedArchitecture.EventFlowStateMachine.Editor.Model;

[CreateAssetMenu(fileName = "EventFlowStateMachine_", menuName = "Vocario/EventFlowStateMachine", order = 0)]
public class EventFlowStateMachine : GameEventManager
{
    [SerializeField]
    private StateMachine _stateMachine;

#if UNITY_EDITOR
    public List<Node> NodeData;
#endif
}

public class State : AState
{
    public State(StateMachine parent) : base(parent) { }

    protected override void OnEnter() { }

    protected override void OnExit() { }
}


public class CreateNodePendingChanges : APendingChanges
{
    public Node Data = null;
    public List<Node> SavedData = null;

    public override void Commit() => SavedData.Add(Data);
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
    public Guid Id;
    public List<Node> SavedData = null;

    public override void Commit()
    {
        Node nodeData = SavedData.Find(SearchClause);
        if (nodeData != null)
        {
            _ = SavedData.Remove(nodeData);
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
        public int EventEnumIndex;
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
    }
}
