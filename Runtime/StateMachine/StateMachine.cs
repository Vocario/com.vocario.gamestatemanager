using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class StateMachine
{
    [SerializeReference]
    private AState _initialState;
    public AState InitialState
    {
        get => _initialState;
        set
        {
            if (!_states.Contains(value))
            {
                throw new StateNotFoundException();
            }
            _initialState.IsInitial = false;
            _initialState = value;
            _initialState.IsInitial = true;
        }
    }

    // TODO Possible extra validation?
    public AState CurrentState = null;
    [SerializeReference]
    private List<AState> _states;

    public StateMachine() => _states = new List<AState>();

    public T CreateState<T>() where T : AState
    {
        var newState = (T) Activator.CreateInstance(typeof(T), new object[] { this });

        if (_initialState == null)
        {
            _initialState ??= newState;
            _initialState.IsInitial = true;
        }
        return newState;
    }

    public void AddState(AState state)
    {
        if (state == null)
        {
            // TODO Add log or exception
            return;
        }

        _states.Add(state);
    }

    // TODO Change list for seializable dictionary for more efficiency
    public AState GetState(Guid id) => _states.Find(x => x.Id == id);

    public void Clear() => _states.Clear();

    public void StartMachine() => _initialState.Enter();
}

public class StateNotFoundException : Exception { }
