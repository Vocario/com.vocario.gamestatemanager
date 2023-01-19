using Vocario.EventBasedArchitecture;
using UnityEngine;

[UnityEditor.CustomEditor(typeof(EventFlowStateMachine))]
public class EventFlowStateMachineEditor : GameEventManagerEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var manager = target as EventFlowStateMachine;

        if (GUILayout.Button("Edit"))
        {
            GSMEditorWindow.Open(manager);
        }
    }
}
