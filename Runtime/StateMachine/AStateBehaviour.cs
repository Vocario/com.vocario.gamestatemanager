namespace Vocario.GameStateManager
{
    [System.Serializable]
    public abstract class AStateBehaviour : UnityEngine.ScriptableObject
    {
        public abstract void OnEnter();

        public abstract void OnExit();
    }
}
