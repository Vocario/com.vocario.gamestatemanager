using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;

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
        var searchTreeEntries = new List<SearchTreeEntry>()
        {
            new SearchTreeGroupEntry(new GUIContent("Create Elements")),
            new SearchTreeGroupEntry(new GUIContent("Dialogue Nodes"), 1),
            new SearchTreeEntry(new GUIContent("Single Choice", _indentationIcon))
            {
                userData = "1",
                level = 2
            },
            new SearchTreeEntry(new GUIContent("Multiple Choice", _indentationIcon))
            {
                userData = "2",
                level = 2
            },
            new SearchTreeGroupEntry(new GUIContent("Dialogue Groups"), 1),
            new SearchTreeEntry(new GUIContent("Single Group", _indentationIcon))
            {
                userData = new Group(),
                level = 2
            }
        };
        return searchTreeEntries;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        Vector2 localMousePosition = _graphView.GetLocalMousePosition(context.screenMousePosition, true);

        switch (SearchTreeEntry.userData)
        {
            case "1":
                {
                    var singleChoiceNode = (GSMNode) _graphView.CreateNode("DialogueName", EGSMNodeType.SingleChoice, localMousePosition);
                    _graphView.AddElement(singleChoiceNode);
                    return true;
                }
            case "2":
                {
                    var multipleChoiceNode = (GSMNode) _graphView.CreateNode("DialogueName", EGSMNodeType.SingleChoice, localMousePosition);
                    _graphView.AddElement(multipleChoiceNode);
                    return true;
                }

            case Group _:
                {
                    // _graphView.CreateGroup("DialogueGroup", localMousePosition);

                    return true;
                }
            default:
                {
                    return false;
                }
        }
    }
}
