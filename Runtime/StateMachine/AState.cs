using UnityEngine;
using System;
using System.Collections.Generic;
using Vocario.EventBasedArchitecture;

[Serializable]
public abstract class AState
{
    [SerializeField]
    protected Guid _id = new Guid();
    [SerializeReference]
    protected List<AGameEventListener> _transitions;
    protected StateMachine _parent = null;
    public Guid Id => _id;
    [SerializeField]
    public bool IsInitial = false;
    [SerializeField]
    public Action OnEnterDelegate = null;
    [SerializeField]
    public Action OnExitDelegate = null;

    protected AState(StateMachine parent)
    {
        _parent = parent;
        _parent.AddState(this);
        _transitions = new List<AGameEventListener>();
    }

    internal bool AddTransition(Transition transition)
    {
        if (transition == null)
        {
            Debug.LogError($"Attempted to add null transition to state");
            return false;
        }
        _transitions.Add(transition);
        return true;
    }

    public void Enter()
    {
        foreach (Transition transition in _transitions)
        {
            transition.Active = true;
        }

        OnEnter();
        OnEnterDelegate?.Invoke();
    }
    public void Exit()
    {
        foreach (Transition transition in _transitions)
        {
            transition.Active = false;
        }

        OnExit();
        OnExitDelegate?.Invoke();
    }

    protected abstract void OnEnter();

    protected abstract void OnExit();
}
