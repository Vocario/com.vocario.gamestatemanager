using UnityEngine;
using System;
using Vocario.EventBasedArchitecture;
using System.Linq;
using System.Collections.Generic;

namespace Vocario.GameStateManager
{
    [Serializable]
    public class State
    {
        [SerializeField]
        protected string _id = "";
        public Guid Id => Guid.Parse(_id);
        // TODO Create a dictionary serialized properly by serialized reference
        [SerializeReference]
        protected List<Transition> _transitions;
        [SerializeReference, HideInInspector]
        protected StateMachine _stateMachine;
        [SerializeField]
        public bool IsInitial = false;
        [SerializeField]
        public Action OnEnterDelegate = null;
        [SerializeField]
        public Action OnExitDelegate = null;
        [SerializeField]
        protected StateBehavioursDictionary _stateBehaviours;

        public State(StateMachine stateMachine)
        {
            _id = Guid.NewGuid().ToString();
            _transitions = new List<Transition>();
            _stateMachine = stateMachine;
        }

        // TODO Add a contains validation
        internal Transition CreateTransition(Type gameEvent, State to)
        {
            if (gameEvent == null || to == null)
            {
                throw new ArgumentNullException();
            }
            Transition transition = ScriptableObject.CreateInstance<Transition>();
            transition.Initiate(gameEvent, this, to);
            _transitions.Add(transition);
            return transition;
        }

        internal Transition RemoveTransition(Type gameEvent)
        {
            Transition transition = _transitions.Find(x => x.GameEventName == gameEvent.ToString());
            if (transition == null)
            {
                return null;
            }

            transition.Deregister();
            _ = _transitions.Remove(transition);
            return transition;
        }

        public AStateBehaviour AddStateBehaviour(string typeString)
        {
            if (_stateBehaviours.Contains(typeString))
            {
                return null;
            }

            Type type = Utility.GetType(typeString.Trim());
            if (!type.IsSubclassOf(typeof(AStateBehaviour)))
            {
                Debug.Log($"{type.IsSubclassOf(typeof(AStateBehaviour))}");
                return null;
            }
            var behaviourInstance = (AStateBehaviour) ScriptableObject.CreateInstance(type);


            _stateBehaviours.Add(typeString, behaviourInstance);
            return behaviourInstance;
        }

        public AStateBehaviour RemoveStateBehaviour(string typeString)
        {
            if (!_stateBehaviours.Contains(typeString))
            {
                return null;
            }
            AStateBehaviour stateBehaviour = _stateBehaviours[ typeString ];

            _ = _stateBehaviours.Remove(typeString);
            return stateBehaviour;
        }

        public AStateBehaviour[] GetStateBehaviours() => _stateBehaviours.Values.ToArray();

        public void Enter()
        {
            foreach (Transition transition in _transitions)
            {
                transition.Active = true;
            }

            OnEnter();
            OnEnterDelegate?.Invoke();
            foreach (AStateBehaviour stateBehaviour in _stateBehaviours.Values)
            {
                stateBehaviour.OnEnter();
            }
        }
        public void Exit()
        {
            foreach (Transition transition in _transitions)
            {
                transition.Active = false;
            }

            OnExit();
            OnExitDelegate?.Invoke();
            foreach (AStateBehaviour stateBehaviour in _stateBehaviours.Values)
            {
                stateBehaviour.OnExit();
            }
        }

        protected virtual void OnEnter() { }

        protected virtual void OnExit() { }
    }

    [Serializable]
    public class TransitionsDictionary : SerializableDictionary<string, Transition> { }

    [Serializable]
    public class StateBehavioursDictionary : SerializableDictionary<string, AStateBehaviour> { }
}
