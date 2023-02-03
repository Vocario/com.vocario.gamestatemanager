using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using NodeModel = Vocario.EventBasedArchitecture.EventFlowStateMachine.Editor.Model.Node;
using PortModel = Vocario.EventBasedArchitecture.EventFlowStateMachine.Editor.Model.Port;

public class GSMGraphView : GraphView
{
    private GSMSearchWindow _searchWindow = null;
    private MiniMap _miniMap = null;
    private GraphViewDependencies _dependencies;

    internal Dictionary<Guid, GSMNode> Nodes { get; private set; } = new Dictionary<Guid, GSMNode>();

    public new class UxmlFactory : UxmlFactory<GSMGraphView, GraphView.UxmlTraits> { }

    public GSMGraphView()
    {
        AddManipulators();
        AddGridBackground();
        AddSearchWindow();
        // AddMiniMap();

        AddStyles();
        // AddMiniMapStyles();
    }

    internal void Init(GraphViewDependencies dependencies)
    {
        _dependencies = dependencies;
        NodeModel[] nodes = _dependencies.NodeController.GetAll();
        foreach (NodeModel node in nodes)
        {
            _ = CreateNode(node.GraphId, node.Name, node.X, node.Y, node.IsInitial, node.Ports);
        }
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        if (graphViewChange.movedElements != null)
        {
            foreach (GraphElement item in graphViewChange.movedElements)
            {
                if (item is GSMNode node)
                {
                    Rect position = node.GetPosition();
                    _dependencies.NodeController.Update(node.ID, node.name, position.x, position.y, node.IsInitial);
                }
            }
        }

        if (graphViewChange.elementsToRemove != null)
        {
            foreach (GraphElement item in graphViewChange.elementsToRemove)
            {
                if (item is GSMNode node)
                {
                    _dependencies.NodeController.Remove(node.ID);
                }
            }
        }
        return graphViewChange;
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter) => ports
            .ToList()
            .Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node)
            .ToList();

    private void AddManipulators()
    {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        graphViewChanged = OnGraphViewChanged;
    }

    internal Vector2 GetLocalMousePosition(Vector2 mousePosition, bool isSearchWindow = false)
    {
        GSMEditorWindow editorWindow = EditorWindow.GetWindow<GSMEditorWindow>("Event Flow State Manager");
        Vector2 worldMousePosition = isSearchWindow
            ? editorWindow.rootVisualElement.ChangeCoordinatesTo(editorWindow.rootVisualElement.parent, mousePosition - editorWindow.position.position)
            : mousePosition;
        Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);
        return localMousePosition;
    }

    // TODO Fix code repetition
    internal GSMNode CreateNode(Guid id, string name, float x, float y, bool isInitial, List<PortModel> ports)
    {
        var node = new GSMNode();
        node.Init(id, name, x, y, isInitial, _dependencies.EventInfo, ports, _dependencies.NodeController, _dependencies.PortController);
        AddElement(node);
        Nodes.Add(node.ID, node);
        return node;
    }

    internal GSMNode CreateNode(string name, Vector2 localMousePosition)
    {
        var node = new GSMNode();
        node.Init(null, name, localMousePosition.x, localMousePosition.y, false, _dependencies.EventInfo, new List<PortModel>(), _dependencies.NodeController, _dependencies.PortController);
        _dependencies.NodeController.Create(node.ID, name, localMousePosition.x, localMousePosition.y, false);
        AddElement(node);
        Nodes.Add(node.ID, node);
        return node;
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
        nodeCreationRequest = context =>
        {
            var windowContext = new SearchWindowContext(context.screenMousePosition);
            _ = SearchWindow.Open(windowContext, _searchWindow);
        };
    }
}
