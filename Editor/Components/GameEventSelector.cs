using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class GameEventSelector : ObjectField
{
    private Button _addButton = null;

    public new class UxmlFactory : UxmlFactory<GSMNodeDetails, VisualElement.UxmlTraits> { }

    public GameEventSelector() => CreateGUI();

    protected void CreateGUI()
    {
        // VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.vocario.gamestatemanager/Editor/Resources/GSMNodeDetails.uxml");
        // visualTree.CloneTree(this);
        // _addButton = this.Q<Button>("add-button");
    }
}
