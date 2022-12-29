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

    public State CreateState()
    {
        State newState = _stateMachine.CreateState<State>();
        Metadata.AddNodeMetadata(newState.Id, newState.IsInitial);
        return newState;
    }

    public State CreateState(string name, float x, float y)
    {
        State newState = _stateMachine.CreateState<State>();
        Metadata.AddNodeMetadata(newState.Id, newState.IsInitial, name, x, y);
        return newState;
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

    public void AddNodeMetadata(Guid id, bool isInitial, string name = "Start", float x = 0, float y = 0) => NodeMetadata.Add(new NodeEditorMetadata()
    {
        ID = id,
        IsInitial = isInitial,
        Name = name,
        X = x,
        Y = y
    });
}

[Serializable]
public struct NodeEditorMetadata
{
    public Guid ID;
    public bool IsInitial;
    public string Name;
    public float X;
    public float Y;
}

[Serializable]
public struct EdgeEditorMetadata
{
    public Enum EventName;
    public Guid RightNode;
    public Guid LeftNode;
}
