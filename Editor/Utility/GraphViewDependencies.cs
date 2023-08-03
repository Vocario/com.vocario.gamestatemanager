using Vocario.EventBasedArchitecture.EventFlowStateMachine.Editor;
using System.Collections.Generic;

public class GraphViewDependencies
{
    public NodeController NodeController { get; private set; }
    public PortController PortController { get; private set; }
    public StateBehaviourController StateBehaviourController { get; private set; }
    public EdgeController EdgeController { get; private set; }

    public GraphViewDependencies(NodeController nodeController,
                                PortController portController,
                                StateBehaviourController stateBehaviourController,
                                EdgeController edgeController)
    {
        NodeController = nodeController;
        PortController = portController;
        StateBehaviourController = stateBehaviourController;
        EdgeController = edgeController;
    }
}
