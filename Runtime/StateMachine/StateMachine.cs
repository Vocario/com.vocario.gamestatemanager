using UnityEngine;
using System;
using Vocario.EventBasedArchitecture;
using UnityEditor;

namespace Vocario.GameStateManager
{
    [Serializable]
    public class StateMachine : GameEventManager
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
        //TODO Maybe change to dictionary to avoid lookup
        [SerializeField]
        protected StatesDictionary _states = new StatesDictionary();

        public T CreateState<T>() where T : State
        {
            var newState = (T) Activator.CreateInstance<T>();

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
            State state = _states[ id.ToString() ];
            _states.Remove(state);
        }

        public Transition CreateTransition(int transitionIndex, Guid fromStateId, Guid toStateId)
        {
            var transitionId = (Enum) Enum.Parse(_enumType, transitionIndex.ToString());
            State fromState = _states[ fromStateId.ToString() ];
            State toState = _states[ toStateId.ToString() ];

            return fromState.CreateTransition(GetGameEvent(transitionId), toState);
        }

        public void DeleteTransition(int transitionIndex, Guid stateId)
        {
            var transitionId = (Enum) Enum.Parse(_enumType, transitionIndex.ToString());
            GameEvent gameEvent = GetGameEvent(transitionId);
            _ = _states[ stateId.ToString() ].RemoveTransition(gameEvent);
        }

        public void AddState(State state)
        {
            if (state == null)
            {
                // TODO Add log or exception
                return;
            }
            _states.Add(state.Id.ToString(), state);

        }

        public State GetState(Guid id) => _states[ id.ToString() ];

        public void Clear() => _states.Clear();

        public void StartMachine() => _initialState.Enter();
    }

    public class StateNotFoundException : Exception { }

    [Serializable]
    public class StatesDictionary : SerializableDictionary<string, State> { }
}
