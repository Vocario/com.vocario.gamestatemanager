using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;
using System;

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
        var searchTreeEntries = new List<SearchTreeEntry>();
        return searchTreeEntries;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context) => false;
}
