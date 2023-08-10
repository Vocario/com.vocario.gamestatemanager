using UnityEngine;
using System;
using Vocario.EventBasedArchitecture;
using UnityEditor;
using System.Collections.Generic;

namespace Vocario.GameStateManager
{
    [Serializable]
    public class StateMachine : ScriptableObject
    {
        [SerializeReference]
        protected State _initialState;
        public State InitialState
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
        public State CurrentState = null;
        // TODO Maybe change to dictionary to avoid lookup
        [SerializeReference]
        private List<State> _states = new List<State>();

        public T CreateState<T>() where T : State
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
            State state = _states.Find(x => x.Id == id);
            if (state != null)
            {
                _ = _states.Remove(state);
            }
        }

        // TODO Validations for find
        public Transition CreateTransition(string portEventName, Guid fromStateId, Guid toStateId)
        {
            State fromState = _states.Find(x => x.Id == fromStateId);
            State toState = _states.Find(x => x.Id == toStateId);

            return fromState.CreateTransition(Type.GetType(portEventName), toState);
        }

        // TODO Validations for find
        public void DeleteTransition(string portEventName, Guid stateId) => _ = _states
            .Find(x => x.Id == stateId)
            .RemoveTransition(Type.GetType(portEventName));

        public void AddState(State state)
        {
            if (state == null)
            {
                // TODO Add log or exception
                return;
            }
            _states.Add(state);

        }

        // TODO Change list for seializable dictionary for more efficiency
        public State GetState(Guid id) => _states.Find(x => x.Id == id);

        public void Clear() => _states.Clear();

        public void StartMachine() => _initialState.Enter();
    }

    public class StateNotFoundException : Exception { }

    [Serializable]
    public class StatesDictionary : SerializableDictionary<string, State> { }
}
