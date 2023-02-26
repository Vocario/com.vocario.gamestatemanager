using UnityEngine;
using System;
using Vocario.EventBasedArchitecture;

[Serializable]
public abstract class AState
{
    [SerializeField]
    protected string _id = "";
    public Guid Id => Guid.Parse(_id);
    // TODO Maybe also change this to a dictionary or hashset
    [SerializeReference]
    protected TransitionsDictionary _transitions;
    protected StateMachine _parent = null;
    [SerializeField]
    public bool IsInitial = false;
    [SerializeField]
    public Action OnEnterDelegate = null;
    [SerializeField]
    public Action OnExitDelegate = null;

    protected AState(StateMachine parent)
    {
        _id = Guid.NewGuid().ToString();
        _parent = parent;
        _transitions = new TransitionsDictionary();
    }

    // TODO Add a contains validation
    internal Transition CreateTransition(GameEvent gameEvent, AState to)
    {
        if (gameEvent == null || to == null)
        {
            throw new ArgumentNullException();
        }
        var transition = new Transition(gameEvent, this, to);
        _transitions[ gameEvent.Name ] = transition;
        return transition;
    }

    // TODO Add a contains validation
    internal void RemoveTransition(GameEvent gameEvent) => _transitions.Remove(gameEvent);

    public void Enter()
    {
        foreach (Transition transition in _transitions.Values)
        {
            transition.Active = true;
        }

        OnEnter();
        OnEnterDelegate?.Invoke();
    }
    public void Exit()
    {
        foreach (Transition transition in _transitions.Values)
        {
            transition.Active = false;
        }

        OnExit();
        OnExitDelegate?.Invoke();
    }

    protected abstract void OnEnter();

    protected abstract void OnExit();
}

[Serializable]
public class TransitionsDictionary : SerializableDictionary<string, Transition> { }
