using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEditor;

public class GameEventSearchWindow : ScriptableObject, ISearchWindowProvider
{
    private List<EventInfo> _eventInfo = null;
    private Action<int> _onSelectionCallback = null;

    internal void Init(List<EventInfo> eventInfo, Action<int> onSelectionCallback)
    {
        _eventInfo = eventInfo;
        _onSelectionCallback = onSelectionCallback;
    }

    // TODO Cache and refetch on change
    // TODO Only add events that are not handled on the state already
    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        Texture icon = EditorGUIUtility.FindTexture("d_cs Script Icon");
        var header = new List<SearchTreeEntry>() { new SearchTreeGroupEntry(new GUIContent("State Behaviours")) };
        IEnumerable<SearchTreeEntry> searchTreeEntries = _eventInfo
            .Select(eventInfo => new SearchTreeEntry(new GUIContent(eventInfo.Name, icon))
            {
                userData = eventInfo.EnumId,
                level = 1
            });

        return header.Concat(searchTreeEntries).ToList();
    }

    public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
    {
        _onSelectionCallback?.Invoke((int) searchTreeEntry.userData);
        return true;
    }

}
