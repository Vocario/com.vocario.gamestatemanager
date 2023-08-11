using Vocario.EventBasedArchitecture.EventFlowStateMachine.Editor.Model;
using Vocario.GameStateManager;
using System;
using UnityEditor;
using UnityEngine;

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
            Port port = _graphData.GetPort(outPortId, outNodeId);
            if (string.IsNullOrEmpty(port.Name))
            {
                return;
            }
            Undo.RecordObject(_stateMachine, "Added transition through graph view");
            int undoID = Undo.GetCurrentGroup();

            _ = _graphData.CreateEdge(outPortId, inNodeId, outNodeId);

            Node outNode = _graphData.GetNode(outNodeId);
            Node inNode = _graphData.GetNode(inNodeId);
            Transition transtion = _stateMachine.CreateTransition(port.Name, outNode.StateId, inNode.StateId);
            transtion.name = Guid.NewGuid().ToString();
            transtion.hideFlags = HideFlags.HideInHierarchy;

            Undo.RegisterCreatedObjectUndo(transtion, "Added state behaviour through graph view");
            Undo.CollapseUndoOperations(undoID);

            AssetDatabase.AddObjectToAsset(transtion, _stateMachine);
            AssetDatabase.SaveAssets();

        }

        internal void Remove(Guid outPortId, Guid outNodeId)
        {
            Undo.RecordObject(_stateMachine, "Removed transition through graph view");

            _ = _graphData.RemoveEdge(outPortId);

            Node node = _graphData.GetNode(outNodeId);
            Port port = _graphData.GetPort(outPortId, outNodeId);
            Transition transition = _stateMachine.DeleteTransition(port.Name, node.StateId);

            if (transition == null)
            {
                return;
            }
            AssetDatabase.RemoveObjectFromAsset(transition);
        }

        internal Edge[] GetAll() => _graphData.GetEdges();
    }
}
