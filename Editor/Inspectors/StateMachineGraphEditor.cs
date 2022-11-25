using UnityEngine;

// TODO Move to editor definition

[UnityEditor.CustomEditor(typeof(StateMachineGraph), editorForChildClasses: true)]
public class StateMachineGraphEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var manager = target as StateMachineGraph;

        if (GUILayout.Button("Edit"))
        {
            GSMEditorWindow.Open(manager);
        }
    }
}
