using UnityEngine;
using System;
using Vocario.EventBasedArchitecture;

namespace Vocario.GameStateManager
{
    [Serializable]
    public class Transition
    {
        [SerializeReference, HideInInspector]
        protected State _from;
        [SerializeReference, HideInInspector]
        protected State _to;
        public bool Active = false;
        [field: SerializeField]
        public string GameEventName { get; protected set; }

        public Transition(Type gameEvent, State from, State to)
        {
            if (from == null || to == null)
            {
                throw new ArgumentNullException($"Attempted to create transition with To: {to} and From: {from}");
            }

            _from = from;
            _to = to;
            GameEventName = gameEvent.AssemblyQualifiedName;

            _ = GameEventManager.TryAddListenerByType(gameEvent, this, RaiseEvent);
        }

        public void Deregister() => _ = GameEventManager.TryRemoveListenerByType(Type.GetType(GameEventName), this, RaiseEvent);

        protected void RaiseEvent()
        {
            if (Active)
            {
                _from.Exit();
                _to.Enter();
            }
        }
    }
}
