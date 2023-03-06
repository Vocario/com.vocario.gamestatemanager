using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEditor;
using Vocario.GameStateManager;

public class StateBehaviourSearchWindow : ScriptableObject, ISearchWindowProvider
{
    private Action<string> _addNewItemm;

    public void Init(Action<string> addNewItem) => _addNewItemm = addNewItem;

    // TODO Cache and refetch on change
    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        var header = new List<SearchTreeEntry>() { new SearchTreeGroupEntry(new GUIContent("State Behaviours")) };
        IEnumerable<SearchTreeEntry> searchTreeEntries = AppDomain.CurrentDomain
            .GetAssemblies()
            .Select(assembly => assembly.GetTypes())
            .SelectMany(x => x)
            .Where(type => type.IsSubclassOf(typeof(AStateBehaviour)))
            .Select(type => new SearchTreeEntry(new GUIContent(type.ToString(), EditorGUIUtility.FindTexture("d_cs Script Icon")))
            {
                userData = type,
                level = 1
            });

        return header.Concat(searchTreeEntries).ToList();
    }

    public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
    {
        _addNewItemm?.Invoke(((Type) searchTreeEntry.userData).ToString());
        return true;
    }
}
