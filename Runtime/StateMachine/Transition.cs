using UnityEngine;
using System;
using Vocario.EventBasedArchitecture;

namespace Vocario.GameStateManager
{
    [Serializable]
    public class Transition : AGameEventListener
    {
        [SerializeReference, HideInInspector]
        protected State _from;
        [SerializeReference, HideInInspector]
        protected State _to;
        public bool Active = false;


        public Transition(AGameEvent gameEvent, State from, State to) : base(gameEvent)
        {
            if (from == null || to == null)
            {
                throw new ArgumentNullException($"Attempted to create transition with To: {to} and From: {from}");
            }

            _gameEvent = gameEvent;
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
}
