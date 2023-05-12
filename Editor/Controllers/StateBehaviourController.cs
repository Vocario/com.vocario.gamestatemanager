using Vocario.EventBasedArchitecture.EventFlowStateMachine.Editor.Model;
using Vocario.GameStateManager;
using UnityEngine.UIElements;
using UnityEditor;
using System;
using UnityEngine;
using System.Linq;

namespace Vocario.EventBasedArchitecture.EventFlowStateMachine.Editor
{
    public class StateBehaviourController
    {
        private StateMachine _stateMachine = null;
        private GraphViewData _graphData = null;

        public StateBehaviourController(StateMachine stateMachine, GraphViewData graphViewData)
        {
            _stateMachine = stateMachine;
            _graphData = graphViewData;
        }

        internal VisualElement Create(Guid nodeId, string typeName)
        {
            Undo.RecordObject(_stateMachine, "Added state behaviour through graph view");

            int undoID = Undo.GetCurrentGroup();
            Node nodeData = _graphData.GetNode(nodeId);
            State state = _stateMachine.GetState(nodeData.StateId);
            AStateBehaviour stateBehaviour = state.AddStateBehaviour(typeName);

            stateBehaviour.name = Guid.NewGuid().ToString();
            stateBehaviour.hideFlags = HideFlags.HideInHierarchy;

            Undo.RegisterCreatedObjectUndo(stateBehaviour, "Added state behaviour through graph view");
            Undo.CollapseUndoOperations(undoID);

            AssetDatabase.AddObjectToAsset(stateBehaviour, _stateMachine);
            AssetDatabase.SaveAssets();

            return GetInspectorForStateBehaviour(stateBehaviour);
        }

        internal bool Remove(Guid nodeId, string typeName)
        {
            Undo.RecordObject(_stateMachine, "Removed state behaviour through graph view");

            Node nodeData = _graphData.GetNode(nodeId);
            State state = _stateMachine.GetState(nodeData.StateId);
            AStateBehaviour stateBehaviour = state.RemoveStateBehaviour(typeName);

            if (stateBehaviour == null)
            {
                return false;
            }
            AssetDatabase.RemoveObjectFromAsset(stateBehaviour);

            return stateBehaviour;
        }

        internal VisualElement[] GetElements(Guid nodeId)
        {
            Node nodeData = _graphData.GetNode(nodeId);
            if (nodeData == null)
            {
                return new VisualElement[] { };
            }
            State state = _stateMachine.GetState(nodeData.StateId);
            AStateBehaviour[] stateBehaviour = state.GetStateBehaviours();

            return stateBehaviour.Select(GetInspectorForStateBehaviour).ToArray();
        }


        private VisualElement GetInspectorForStateBehaviour(AStateBehaviour stateBehaviour)
        {
            var editor = UnityEditor.Editor.CreateEditor(stateBehaviour);

            return editor.CreateInspectorGUI();
        }
    }
}
