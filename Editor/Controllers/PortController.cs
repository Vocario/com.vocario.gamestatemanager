using Vocario.EventBasedArchitecture.EventFlowStateMachine.Editor.Model;
using Vocario.GameStateManager;
using System;
using UnityEditor;

namespace Vocario.EventBasedArchitecture.EventFlowStateMachine.Editor
{
    public class PortController
    {
        private StateMachine _stateMachine = null;
        private GraphViewData _graphData = null;

        public PortController(StateMachine stateMachine, GraphViewData graphViewData)
        {
            _stateMachine = stateMachine;
            _graphData = graphViewData;
        }

        internal void Create(Guid id, Guid nodeId, Type eventType)
        {
            Undo.RecordObject(_stateMachine, "Created port for node through graph view");
            string typeName = eventType != null ? eventType.AssemblyQualifiedName : "";

            _graphData.CreatePort(id, nodeId, typeName);
        }

        internal void Update(Guid id, Guid nodeId, Type eventType)
        {
            Undo.RecordObject(_stateMachine, "Updated port for node through graph view");
            string typeName = eventType != null ? eventType.AssemblyQualifiedName : "";

            _graphData.UpdatePort(id, nodeId, typeName);
        }

        internal void Remove(Guid id, Guid nodeId)
        {
            Undo.RecordObject(_stateMachine, "Removed port for node through graph view");

            _ = _graphData.RemovePort(id, nodeId);
        }
    }
}
