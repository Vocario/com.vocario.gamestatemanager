using UnityEditor.Experimental.GraphView;

using UnityEngine.UIElements;

public class NodeTest : Node
{

    public new class UxmlFactory : UxmlFactory<NodeTest, Node.UxmlTraits> { }
}
