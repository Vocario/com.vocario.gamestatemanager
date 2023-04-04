namespace Vocario.GameStateManager
{
    [System.Serializable]
    public class AStateBehaviour : UnityEngine.ScriptableObject
    {
        public virtual void OnEnter() { }

        public virtual void OnExit() { }
    }
}
