using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
[CreateAssetMenu(fileName = "GameStateManager_", menuName = "Scriptable/GameStateManager", order = 11)]
public class StateMachine
{
    [SerializeField]
    private AState _initialState = null;
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
    [SerializeField]
    private List<AState> _states = new List<AState>();

    public T CreateState<T>() where T : AState
    {
        var newState = (T) Activator.CreateInstance(typeof(T));

        Debug.Log($"{typeof(T)}");
        Debug.Log($"{newState}");
        if (_initialState == null)
        {
            _initialState ??= newState;
            _initialState.IsInitial = true;
        }
        AddState(newState);
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

    public AState GetState(Guid id) => _states.Find(x => x.Id == id);

    public void StartMachine() => _initialState.Enter();
}

public class StateNotFoundException : Exception { }
