using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEditor;
using Vocario.EventBasedArchitecture;

public class GameEventSearchWindow : ScriptableObject, ISearchWindowProvider
{
    private Action<Type> _onSelectionCallback = null;

    internal void Init(Action<Type> onSelectionCallback) => _onSelectionCallback = onSelectionCallback;

    // TODO Cache and refetch on change
    // TODO Only add events that are not handled on the state already
    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        Texture icon = EditorGUIUtility.FindTexture("d_cs Script Icon");
        var header = new List<SearchTreeEntry>() { new SearchTreeGroupEntry(new GUIContent("State Behaviours")) };
        IEnumerable<SearchTreeEntry> searchTreeEntries = GameEventManager.GetGameEventsTypes()
            .Select(eventType => new SearchTreeEntry(new GUIContent(eventType.FullName, icon))
            {
                userData = eventType,
                level = 1
            });

        return header.Concat(searchTreeEntries).ToList();
    }

    public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
    {
        _onSelectionCallback?.Invoke((Type) searchTreeEntry.userData);
        return true;
    }

}
