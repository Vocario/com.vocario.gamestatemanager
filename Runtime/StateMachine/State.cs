using UnityEngine;
using System;
using Vocario.EventBasedArchitecture;

namespace Vocario.GameStateManager
{
    [Serializable]
    public class State
    {
        [SerializeField]
        protected string _id = "";
        public Guid Id => Guid.Parse(_id);
        [SerializeReference]
        protected TransitionsDictionary _transitions;
        protected StateMachine _parent = null;
        [SerializeField]
        public bool IsInitial = false;
        [SerializeField]
        public Action OnEnterDelegate = null;
        [SerializeField]
        public Action OnExitDelegate = null;
        [SerializeField]
        protected StateBehavioursDictionary _stateBehaviours = new StateBehavioursDictionary();

        public State(StateMachine parent)
        {
            _id = Guid.NewGuid().ToString();
            _parent = parent;
            _transitions = new TransitionsDictionary();
        }

        // TODO Add a contains validation
        internal Transition CreateTransition(GameEvent gameEvent, State to)
        {
            if (gameEvent == null || to == null)
            {
                throw new ArgumentNullException();
            }
            var transition = new Transition(gameEvent, this, to);
            _transitions[ gameEvent.Name ] = transition;
            return transition;
        }

        internal bool RemoveTransition(GameEvent gameEvent)
        {
            if (!_transitions.Contains(gameEvent))
            {
                return false;
            }

            _transitions.Remove(gameEvent);
            return true;
        }

        public AStateBehaviour AddStateBehaviour(string typeString)
        {
            if (_stateBehaviours.Contains(typeString))
            {
                return null;
            }

            Type type = GetType(typeString.Trim());
            if (!type.IsSubclassOf(typeof(AStateBehaviour)))
            {
                Debug.Log($"{type.IsSubclassOf(typeof(AStateBehaviour))}");
                return null;
            }
            var behaviourInstance = (AStateBehaviour) ScriptableObject.CreateInstance(type);

            _stateBehaviours.Add(typeString, behaviourInstance);
            return behaviourInstance;
        }

        public static Type GetType(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null)
            {
                return type;
            }

            foreach (System.Reflection.Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = a.GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }
            return null;
        }

        internal bool RemoveStateBehaviour(string typeString) => _stateBehaviours.Contains(typeString) && _stateBehaviours.Remove(typeString);

        public void Enter()
        {
            foreach (Transition transition in _transitions.Values)
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
            foreach (Transition transition in _transitions.Values)
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
