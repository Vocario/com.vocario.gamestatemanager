using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using System;
using Vocario.GameStateManager;

[CreateAssetMenu(fileName = "StateMachineGraph_", menuName = "Vocario/StateMachineGraph", order = 11)]
public class StateMachineGraph : ScriptableObject
{
    [SerializeField]
    private StateMachine _stateMachine = new StateMachine();
    private List<NodeMetadata> _nodeMetadata = new List<NodeMetadata>();

    public T CreateState<T>(Node node) where T : State
    {
        T newState = _stateMachine.CreateState<T>();
        _nodeMetadata.Add(new NodeMetadata()
        {
            Title = node.title,
            Position = node.GetPosition(),
            StateId = newState.Id
        });
        Debug.Log($"{newState}");
        // var serializedObject = new SerializedObject(newState);

        // SerializedProperty current = serializedObject.GetIterator();
        // while (current != null)
        // {
        //     Debug.Log($"{current.name}");
        //     _ = current.Next(true);
        // }
        return newState;
    }
}

[Serializable]
public class NodeMetadata
{
    [field: SerializeField] public string Title { get; set; } = "";
    [field: SerializeField] public Rect Position { get; set; } = Rect.zero;
    [field: SerializeField] public Guid StateId { get; set; } = Guid.Empty;
}
