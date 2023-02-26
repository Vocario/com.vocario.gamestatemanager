using UnityEngine;
using System;
using Vocario.EventBasedArchitecture;

[Serializable]
public class Transition : AGameEventListener
{
    [SerializeReference, HideInInspector]
    protected AState _from;
    [SerializeReference, HideInInspector]
    protected AState _to;
    public bool Active = false;

    public Transition(GameEvent gameEvent, AState from, AState to) : base(gameEvent)
    {
        if (from == null || to == null)
        {
            throw new ArgumentNullException($"Attempted to create transition with To: {to} and From: {from}");
        }

        _from = from;
        _to = to;

        _ = _gameEvent.Register(this);
    }

    ~Transition()
    {
        Deregister();
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
