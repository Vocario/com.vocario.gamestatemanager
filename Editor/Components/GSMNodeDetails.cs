using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

public class GSMNodeDetails : VisualElement
{
    public const string ASSET_PATH = "Packages/com.vocario.gamestatemanager/Editor/Resources/GSMNodeDetails.uxml";
    private Guid _nodeId;
    private Button _addButton = null;
    private VisualElement _container = null;
    private StateBehaviourSearchWindow _searchWindow = null;
    private Func<Guid, string, VisualElement> _createStateBehaviour;
    private Func<Guid, string, bool> _removeStateBehaviour;

    public new class UxmlFactory : UxmlFactory<GSMNodeDetails, VisualElement.UxmlTraits> { }

    public GSMNodeDetails() => CreateGUI();

    ~GSMNodeDetails()
    {
        _addButton.clickable.clicked -= OpenStateBehaviourSearchWindow;
    }

    public void Init(Guid nodeId,
                    Func<Guid, string, VisualElement> createStateBehaviour,
                    Func<Guid, string, bool> removeStateBehaviour,
                    VisualElement[] initialElements)
    {
        _nodeId = nodeId;
        _createStateBehaviour = createStateBehaviour;
        _removeStateBehaviour = removeStateBehaviour;

        if (initialElements != null)
        {
            foreach (VisualElement element in initialElements)
            {
                AddNewItem(element);
            }
        }
    }

    protected void CreateGUI()
    {
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ASSET_PATH);
        visualTree.CloneTree(this);
        _container = this.Q<VisualElement>("behaviour-list");
        _addButton = this.Q<Button>("add-button");
        _addButton.clickable.clicked += OpenStateBehaviourSearchWindow;
    }

    private void OpenStateBehaviourSearchWindow()
    {
        if (_searchWindow == null)
        {
            _searchWindow = ScriptableObject.CreateInstance<StateBehaviourSearchWindow>();
        }
        _searchWindow.Init(CreateNewItem);
        var windowContext = new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition));
        _ = SearchWindow.Open(windowContext, _searchWindow);
    }

    private void CreateNewItem(string name)
    {
        VisualElement element = _createStateBehaviour?.Invoke(_nodeId, name);

        AddNewItem(element);
    }

    private void AddNewItem(VisualElement element)
    {
        Button removeButton = element.Q<Button>("remove-button");

        removeButton.clicked += () =>
        {
            _ = (_removeStateBehaviour?.Invoke(_nodeId, element.name));
            _container.Remove(element);
        };
        _container.Add(element);
    }
}
