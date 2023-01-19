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

public class RemoveNodePendingChanges : APendingChanges
{
    public Node Data = null;
    public List<Node> SavedData = null;

    public override void Commit() => SavedData.Remove(Data);
}

public class MoveNodePendingChanges : APendingChanges
{
    public Node Data = null;
    public float DeltaX = 0.0f;
    public float DeltaY = 0.0f;

    public override void Commit()
    {
        Data.X += DeltaX;
        Data.Y += DeltaY;
    }
}

public class RenameNodePendingChanges : APendingChanges
{
    public Node Data = null;
    public string OldName = "";
    public string NewName = "";

    public override void Commit() => Data.Name = NewName;
}

public class AddStateBehaviourPendingChanges : APendingChanges
{
    public Node Data = null;
    public string StateBehaviourType = "";

    public override void Commit() => Data.StateBehaviourTypes.Add(StateBehaviourType);
}

public class RemoveStateBehaviourPendingChanges : APendingChanges
{
    public Node Data = null;
    public string StateBehaviourType = "";

    public override void Commit() => Data.StateBehaviourTypes.Remove(StateBehaviourType);
}

public class AddOutputPendingChanges : APendingChanges
{
    public Node Data = null;
    public Port NewPort = null;

    public override void Commit() => Data.Ports.Add(NewPort);
}

public class UpdateOutputPendingChanges : APendingChanges
{
    public Port Port = null;
    public string Name = null;
    public int? Index = null;

    public override void Commit()
    {
        Port.Name = Name ?? Port.Name;
        Port.Index = Index ?? Port.Index;
    }
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
        private string _id;
        public Guid ID
        {
            get => Guid.Parse(_id);
            set => _id = value.ToString();
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
        public Guid OutPortId;
        public Guid InNodeId;
    }
}
