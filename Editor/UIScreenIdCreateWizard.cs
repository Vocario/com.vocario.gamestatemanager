using UnityEngine;
using UnityEditor;

public class UIScreenIdCreateWizard : ScriptableWizard
{
    // TODO Validate if name exists already
    [SerializeField]
    private string _screenName = "";
    [SerializeField]
    private UIScreen _screen = null;
    private static UIManager _manager = null;

    public static void CreateWizard(UIManager manager)
    {
        _manager = manager;
        _ = DisplayWizard<UIScreenIdCreateWizard>("Create UI Screen Definition", "Create");
    }

    private void OnWizardCreate()
    {
        UIScreenId screenId = CreateInstance<UIScreenId>();
        screenId.name = _screenName;
        screenId.Screen = _screen;
        _manager.AddScreenId(screenId);
        AssetDatabase.AddObjectToAsset(screenId, _manager);
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
}
