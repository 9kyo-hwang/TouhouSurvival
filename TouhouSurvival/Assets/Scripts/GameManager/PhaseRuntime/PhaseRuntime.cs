namespace Unchord
{
    public abstract class PhaseRuntime
    {
        protected static GameManager s_gameManager;

        protected PhaseSO _phaseSO;

        static PhaseRuntime()
        {
            s_gameManager = GameManager.Instance;
        }
        
        public PhaseRuntime(PhaseSO phaseSO)
        {
            _phaseSO = phaseSO;
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