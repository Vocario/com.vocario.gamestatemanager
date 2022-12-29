using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;
using System;
using System.Linq;

public class GSMSearchWindow : ScriptableObject, ISearchWindowProvider
{
    private GSMGraphView _graphView;
    private Texture2D _indentationIcon;

    internal void Init(GSMGraphView gSMGraphView)
    {
        _graphView = gSMGraphView;
        _indentationIcon = new Texture2D(1, 1);
        _indentationIcon.SetPixel(0, 0, Color.clear);
        _indentationIcon.Apply();
    }

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        var header = new List<SearchTreeEntry>()
        {
            new SearchTreeGroupEntry(new GUIContent("Create Elements")),
            new SearchTreeEntry(new GUIContent("New State", _indentationIcon))
            {
                level = 1
            }
        };

        return header;
    }

    public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
    {
        Vector2 localMousePosition = _graphView.GetLocalMousePosition(context.screenMousePosition, true);
        _ = _graphView.CreateNode("DialogueName", localMousePosition);
        return true;
    }
}
