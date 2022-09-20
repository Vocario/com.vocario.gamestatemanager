using UnityEngine;
using System;
using Vocario.EventBasedArchitecture;

[Serializable]
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
