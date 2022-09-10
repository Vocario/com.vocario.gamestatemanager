using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System;

public class GSMNode : Node
{
    private GSMGraphView _graphView;

    public string ID { get; set; }
    public string DialogueName { get; set; }

    public virtual void Init(string nodeName, GSMGraphView graphView, Vector2 position)
    {
        ID = Guid.NewGuid().ToString();
        DialogueName = nodeName;

        SetPosition(new Rect(position, Vector2.zero));
        _graphView = graphView;
    }
}
