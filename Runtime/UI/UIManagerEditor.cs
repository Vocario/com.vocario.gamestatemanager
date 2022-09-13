using UnityEngine;

// TODO Move to editor definition

[UnityEditor.CustomEditor(typeof(UIManager), editorForChildClasses: true)]
public class UIManagerEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var manager = target as UIManager;

        if (GUILayout.Button("Create New Definition"))
        {
            UIScreenIdCreateWizard.CreateWizard(manager);
        }
    }
}
