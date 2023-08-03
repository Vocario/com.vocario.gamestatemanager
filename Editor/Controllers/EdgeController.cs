using Vocario.EventBasedArchitecture.EventFlowStateMachine.Editor.Model;
using Vocario.GameStateManager;
using System;
using UnityEditor;

namespace Vocario.EventBasedArchitecture.EventFlowStateMachine.Editor
{
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
            _ = _stateMachine.CreateTransition(port.Name, outNode.StateId, inNode.StateId);
        }

        internal void Remove(Guid outPortId, Guid outNodeId)
        {
            Undo.RecordObject(_stateMachine, "Removed transition through graph view");

            _ = _graphData.RemoveEdge(outPortId);

            Node node = _graphData.GetNode(outNodeId);
            Port port = _graphData.GetPort(outPortId, outNodeId);
            _stateMachine.DeleteTransition(port.Name, node.StateId);
        }

        internal Edge[] GetAll() => _graphData.GetEdges();
    }
}
