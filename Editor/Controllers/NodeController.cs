using Vocario.EventBasedArchitecture.EventFlowStateMachine.Editor.Model;
using Vocario.GameStateManager;
using System;
using UnityEditor;

namespace Vocario.EventBasedArchitecture.EventFlowStateMachine.Editor
{
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
}
