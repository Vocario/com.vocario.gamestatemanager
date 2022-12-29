using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GSMGraphView : GraphView
{
    private GSMEditorWindow _editorWindow = null;
    private EventFlowStateMachine _stateManager = null;
    private GSMSearchWindow _searchWindow = null;
    private MiniMap _miniMap = null;
    private Dictionary<Guid, GSMNode> _nodes = new Dictionary<Guid, GSMNode>();

    public GSMGraphView(GSMEditorWindow editorWindow)
    {
        _editorWindow = editorWindow;
        _stateManager = editorWindow.StateManager;

        AddManipulators();
        AddGridBackground();
        AddSearchWindow();
        // AddMiniMap();

        // OnElementsDeleted();
        // OnGroupElementsAdded();
        // OnGroupElementsRemoved();
        // OnGroupRenamed();
        // OnGraphViewChanged();

        AddStyles();
        // AddMiniMapStyles();
        Load();
    }

    private void Load()
    {

        List<NodeEditorMetadata> nodeMetadata = _stateManager.Metadata.NodeMetadata;

        if (nodeMetadata == null || nodeMetadata.Count == 0)
        {
            // TODO Remove requirement to init on first open, or open upon creation?
            _stateManager.Init();
        }
        foreach (NodeEditorMetadata nodeData in nodeMetadata)
        {
            _ = CreateNode(nodeData.ID, nodeData.IsInitial, nodeData.Name, new Vector2(nodeData.X, nodeData.Y));
        }
        List<EdgeEditorMetadata> edgeMetadata = _stateManager.Metadata.EdgeMetadata;

        foreach (EdgeEditorMetadata edgeData in edgeMetadata)
        {
            Port port = _nodes[ edgeData.LeftNode ].AddEventOutput();
            Edge edge = port.ConnectTo(_nodes[ edgeData.RightNode ].InputPort);
            Add(edge);
        }
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
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

    internal GSMNode CreateNode(Guid id, bool isInitial, string nodeName, Vector2 position)
    {
        var node = new GSMNode();
        node.Init(id, isInitial, nodeName, this, position);
        AddElement(node);
        _nodes.Add(id, node);
        return node;
    }

    internal GSMNode CreateNode(string name, Vector2 localMousePosition)
    {
        State state = _stateManager.CreateState(name, localMousePosition.x, localMousePosition.y);
        return CreateNode(state.Id, state.IsInitial, name, localMousePosition);
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
        nodeCreationRequest = context =>
        {
            var windowContext = new SearchWindowContext(context.screenMousePosition);
            _ = SearchWindow.Open(windowContext, _searchWindow);
        };
    }
}
