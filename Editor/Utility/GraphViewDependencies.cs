using Vocario.EventBasedArchitecture.EventFlowStateMachine.Editor;
using System.Collections.Generic;

public class GraphViewDependencies
{
    public List<EventInfo> EventInfo { get; private set; }
    public NodeController NodeController { get; private set; }
    public PortController PortController { get; private set; }
    public StateBehaviourController StateBehaviourController { get; private set; }
    public EdgeController EdgeController { get; private set; }

    public GraphViewDependencies(List<EventInfo> eventInfo,
                                NodeController nodeController,
                                PortController portController,
                                StateBehaviourController stateBehaviourController,
                                EdgeController edgeController)
    {
        EventInfo = eventInfo;
        NodeController = nodeController;
        PortController = portController;
        StateBehaviourController = stateBehaviourController;
        EdgeController = edgeController;
    }
}
