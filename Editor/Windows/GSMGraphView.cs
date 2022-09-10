using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
using UnityEditor;
using UnityEngine;

public enum EGSMNodeType
{
    SingleChoice
}

public class GSMGraphView : GraphView
{
    private GSMEditorWindow _editorWindow = null;
    private GSMSearchWindow _searchWindow = null;
    private MiniMap _miniMap = null;

    public GSMGraphView(GSMEditorWindow editorWindow)
    {
        _editorWindow = editorWindow;

        AddManipulators();
        AddGridBackground();
        // AddSearchWindow();
        // AddMiniMap();

        AddSearchWindow();
        // AddMiniMap();

        // OnElementsDeleted();
        // OnGroupElementsAdded();
        // OnGroupElementsRemoved();
        // OnGroupRenamed();
        // OnGraphViewChanged();

        AddStyles();
        // AddMiniMapStyles();
    }


    private void AddManipulators()
    {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
    }

    internal GSMNode CreateNode(string nodeName, EGSMNodeType type, Vector2 position)
    {
        var nodeType = Type.GetType("GSMNode");
        var node = (GSMNode) Activator.CreateInstance(nodeType);
        node.Init(nodeName, this, position);
        AddNode(node);
        return node;
    }

    private void AddNode(GSMNode node)
    {
    }

    internal Vector2 GetLocalMousePosition(Vector2 mousePosition, bool isSearchWindow = false)
    {
        Vector2 worldMousePosition = isSearchWindow
            ? _editorWindow.rootVisualElement.ChangeCoordinatesTo(_editorWindow.rootVisualElement.parent, mousePosition - _editorWindow.position.position)
            : mousePosition;
        Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);
        return localMousePosition;
    }

    private void AddGridBackground()
    {
        var gridBackground = new GridBackground();

        gridBackground.StretchToParentSize();
        Insert(0, gridBackground);
    }

    private void AddStyles()
    {
        var styleSheet = (StyleSheet) EditorGUIUtility.Load("Packages/com.vocario.gamestatemanager/Editor/Resources/GSMGraphView.uss");

        styleSheets.Add(styleSheet);
    }

    private void AddSearchWindow()
    {
        if (_searchWindow == null)
        {
            _searchWindow = ScriptableObject.CreateInstance<GSMSearchWindow>();
        }
        _searchWindow.Init(this);
        nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
    }
}
