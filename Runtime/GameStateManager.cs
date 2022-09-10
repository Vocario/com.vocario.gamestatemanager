using UnityEngine;
using System;
using System.Collections.Generic;
using Vocario.EventBasedArchitecture;

// public static class TestStateManager
// {
//     [GameEvents("Main")]
//     public enum EGameEvents
//     {
//         Event1,
//         Event2,
//         Event3
//     }

//     public static void Test()
//     {
//         GameEventManager gameEventManager = ScriptableObject.CreateInstance<GameEventManager>();
//         var manager = new StateMachine();

//         AState firstState = new UIState(manager);
//         AState secondState = new UIState(manager);
//         AState thirdState = new UIState(manager);

//         _ = new Transition(gameEventManager.GetGameEvent(EGameEvents.Event1), firstState, secondState);
//         _ = new Transition(gameEventManager.GetGameEvent(EGameEvents.Event2), secondState, thirdState);
//     }
// }

[CreateAssetMenu(fileName = "GameStateManager_", menuName = "Scriptable/GameStateManager", order = 11)]
public class GameStateManager : ScriptableObject
{

}

public class StateMachine
{
    private List<AState> _states = new List<AState>();

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

public class Transition : AGameEventListener
{
    protected AState _from;
    protected AState _to;
    [field: SerializeField] public bool Active { get; set; } = false;

    public Transition(GameEvent gameEvent, AState from, AState to) : base(gameEvent)
    {
        if (from == null || to == null)
        {
            throw new ArgumentNullException($"Attempted to create transition with To: {to} and From: {from}");
        }

        _from = from;
        _to = to;

        _ = from.AddTransition(this);
        _ = _gameEvent.Register(this);
    }

    public override void RaiseEvent()
    {
        if (Active)
        {
            _from.Exit();
            _to.Enter();
        }
    }
}

public abstract class AState
{
    protected List<Transition> _transitions = new List<Transition>(1);
    protected StateMachine _parent = null;
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

public class UIState : AState
{
    public UIState(StateMachine parent) : base(parent) { }

    protected override void OnEnter() => Debug.Log($"Show UI");

    protected override void OnExit() => Debug.Log($"Hide UI");
}
