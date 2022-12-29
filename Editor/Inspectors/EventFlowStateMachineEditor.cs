using UnityEngine;

// TODO Move to editor definition

[UnityEditor.CustomEditor(typeof(EventFlowStateMachine), editorForChildClasses: true)]
public class EventFlowStateMachineEditor : UnityEditor.Editor
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
