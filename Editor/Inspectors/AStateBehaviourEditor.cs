using Vocario.GameStateManager;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Vocario.EventBasedArchitecture.EventFlowStateMachine.Editor
{
    [CustomEditor(typeof(AStateBehaviour), true, isFallback = true)]
    public class AStateBehaviourEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();

            SerializedProperty iterator = serializedObject.GetIterator();
            if (iterator.NextVisible(true))
            {
                do
                {
                    var propertyField = new PropertyField(iterator.Copy(), iterator.displayName) { name = "PropertyField:" + iterator.propertyPath };

                    if (iterator.propertyPath == "m_Script" && serializedObject.targetObject != null)
                    {
                        propertyField.SetEnabled(value: false);
                    }

                    propertyField.Bind(serializedObject);
                    container.Add(propertyField);
                }
                while (iterator.NextVisible(false));
            }

            return container;
        }
    }
}
