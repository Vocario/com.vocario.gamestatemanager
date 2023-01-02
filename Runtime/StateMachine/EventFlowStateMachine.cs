using Vocario.EventBasedArchitecture;
using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EventFlowStateMachine_", menuName = "Vocario/EventFlowStateMachine", order = 0)]
public class EventFlowStateMachine : GameEventManager
{
    [SerializeField]
    private StateMachine _stateMachine;

#if UNITY_EDITOR
    [SerializeField]
    public EventFlowStateMachineEditorMetadata Metadata;

    public void Init() => CreateState();

    public NodeEditorMetadata CreateState()
    {
        State newState = _stateMachine.CreateState<State>();
        return Metadata.AddNodeMetadata(newState.Id, newState.IsInitial);
    }

    public NodeEditorMetadata CreateState(string name, float x, float y)
    {
        State newState = _stateMachine.CreateState<State>();
        return Metadata.AddNodeMetadata(newState.Id, newState.IsInitial, name, x, y);
    }
#endif
}

public class State : AState
{
    public State(StateMachine parent) : base(parent) { }

    protected override void OnEnter() { }

    protected override void OnExit() { }
}

[Serializable]
public class EventFlowStateMachineEditorMetadata
{
    [field: SerializeField]
    public List<NodeEditorMetadata> NodeMetadata { get; private set; } = new List<NodeEditorMetadata>();
    [field: SerializeField]
    public List<EdgeEditorMetadata> EdgeMetadata { get; private set; } = new List<EdgeEditorMetadata>();

    public NodeEditorMetadata AddNodeMetadata(Guid id, bool isInitial, string name = "Start", float x = 0, float y = 0)
    {
        var data = new NodeEditorMetadata()
        {
            ID = id,
            IsInitial = isInitial,
            Name = name,
            X = x,
            Y = y
        };
        NodeMetadata.Add(data);

        return data;
    }

    public void UpdateNodeMetadata(Guid id, bool? isInitial, string name, float x, float y)
    {
        NodeEditorMetadata data = NodeMetadata.Find(x => x.ID == id);
        data.ID = id;
        data.IsInitial = isInitial ?? data.IsInitial;
        data.Name = name;
        data.X = x;
        data.Y = y;
    }
}

[Serializable]
public class NodeEditorMetadata
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
}

[Serializable]
public class EdgeEditorMetadata
{
    public Enum EventName;
    public Guid RightNode;
    public Guid LeftNode;
}
