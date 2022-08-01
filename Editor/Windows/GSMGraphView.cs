using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
using UnityEditor;
using UnityEngine;

public class GSMGraphView : GraphView
{
    private GSMEditorWindow _editorWindow = null;
    private GSMSearchWindow _searchWindow = null;

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
