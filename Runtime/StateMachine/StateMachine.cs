using UnityEngine;
using System;
using Vocario.EventBasedArchitecture;

[Serializable]
public class StateMachine : GameEventManager
{
    [SerializeReference]
    protected AState _initialState;
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
    //TODO Maybe change to dictionary to avoid lookup
    [SerializeReference]
    protected StatesDictionary _states;

    public StateMachine() => _states = new StatesDictionary();

    public T CreateState<T>() where T : AState
    {
        var newState = (T) Activator.CreateInstance(typeof(T), new object[] { this });

        if (_initialState == null)
        {
            _initialState ??= newState;
            _initialState.IsInitial = true;
        }
        AddState(newState);
        return newState;
    }

    public void DeleteState(Guid id)
    {
        AState state = _states[ id.ToString() ];
        _states.Remove(state);
    }

    public Transition CreateTransition(int transitionIndex, Guid fromStateId, Guid toStateId)
    {
        var transitionId = (Enum) Enum.Parse(_enumType, transitionIndex.ToString());
        AState fromState = _states[ fromStateId.ToString() ];
        AState toState = _states[ toStateId.ToString() ];

        return fromState.CreateTransition(GetGameEvent(transitionId), toState);
    }

    public void DeleteTransition(int transitionIndex, Guid stateId)
    {
        var transitionId = (Enum) Enum.Parse(_enumType, transitionIndex.ToString());
        GameEvent gameEvent = GetGameEvent(transitionId);
        _states[ stateId.ToString() ].RemoveTransition(gameEvent);
    }

    internal void AddState(AState state)
    {
        if (state == null)
        {
            // TODO Add log or exception
            return;
        }

        _states.Add(state.Id.ToString(), state);
    }

    protected AState GetState(Guid id) => _states[ id.ToString() ];

    public void Clear() => _states.Clear();

    public void StartMachine() => _initialState.Enter();
}

public class StateNotFoundException : Exception { }

[Serializable]
public class StatesDictionary : SerializableDictionary<string, AState> { }
