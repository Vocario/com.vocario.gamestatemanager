using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public abstract class AState : UnityEngine.Object
{
    protected Guid _id = new Guid();
    [SerializeField]
    protected List<Transition> _transitions = new List<Transition>(1);
    protected StateMachine _parent = null;
    public Guid Id => _id;
    public bool IsInitial = false;
    public Action OnEnterDelegate = null;
    public Action OnExitDelegate = null;

    protected AState(StateMachine parent)
    {
        _parent = parent;
        _parent.AddState(this);
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
