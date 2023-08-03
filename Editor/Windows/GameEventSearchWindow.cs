using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEditor;
using Vocario.EventBasedArchitecture;

public class GameEventSearchWindow : ScriptableObject, ISearchWindowProvider
{
    private Action<string> _onSelectionCallback = null;

    internal void Init(Action<string> onSelectionCallback) => _onSelectionCallback = onSelectionCallback;

    // TODO Cache and refetch on change
    // TODO Only add events that are not handled on the state already
    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        Texture icon = EditorGUIUtility.FindTexture("d_cs Script Icon");
        var header = new List<SearchTreeEntry>() { new SearchTreeGroupEntry(new GUIContent("State Behaviours")) };
        IEnumerable<SearchTreeEntry> searchTreeEntries = GameEventManager.Instance.GetEventNames()
            .Select(eventName => new SearchTreeEntry(new GUIContent(eventName, icon))
            {
                userData = eventName,
                level = 1
            });

        return header.Concat(searchTreeEntries).ToList();
    }

    public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
    {
        _onSelectionCallback?.Invoke((string) searchTreeEntry.userData);
        return true;
    }

}
