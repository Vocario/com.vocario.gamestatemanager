namespace Vocario.GameStateManager
{
    [System.Serializable]
    public abstract class AStateBehaviour
    {
        public abstract void OnEnter();

        public abstract void OnExit();
    }
}
