using System.Collections.Generic;

public class StateMachine
{
    private List<AState> _states = new List<AState>();

    // TODO Keep track of the current node
    // TODO Specify initial node
    // TODO Start state machine
    // TODO Test serialization

    public void AddState(AState state)
    {
        if (state == null)
        {
            // TODO Add log or exception
            return;
        }

        _states.Add(state);
    }
}
