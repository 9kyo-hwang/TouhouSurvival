namespace Unchord
{
    public abstract class PhaseRuntime
    {
        protected PhaseSO _phaseSO;
        protected GameManager.Properties _gameManagerProperties;

        public PhaseRuntime(PhaseSO phaseSO, GameManager.Properties gameManagerProperties)
        {
            _phaseSO = phaseSO;
            _gameManagerProperties = gameManagerProperties;
        }

        public virtual bool TrySearchNextRuntime()
        {
            return false;
        }

        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void Pause() { }
        public virtual void Resume() { }
        public virtual void End() { }

        public virtual PhaseRuntimeState CheckPhaseRuntimeState()
        {
            return PhaseRuntimeState.Pass;
        }
    }
}