namespace Unchord
{
    public class PhaseCompositeRuntime : PhaseRuntime
    {
        private PhaseRuntime _currentRuntime;
        private int _ptrPhase;

        public PhaseCompositeRuntime(PhaseCompositeSO phaseSO, GameManager.Properties gameManagerProperties)
        : base(phaseSO, gameManagerProperties)
        {
            _currentRuntime = PhaseRuntimeFactory.CreateRuntime(phaseSO.phaseSoList[0], gameManagerProperties);
            _ptrPhase = 0;
        }

        public override bool TrySearchNextRuntime()
        {
            if (_currentRuntime.TrySearchNextRuntime())
                return true;

            PhaseCompositeSO phaseSO = (PhaseCompositeSO)_phaseSO;
            bool canGetNextRuntime = _ptrPhase < phaseSO.phaseSoList.Count - 1;

            if (canGetNextRuntime)
                _currentRuntime = PhaseRuntimeFactory.CreateRuntime(phaseSO.phaseSoList[++_ptrPhase], _gameManagerProperties);
            else
                _currentRuntime = null;

            return canGetNextRuntime;
        }

        public override void Start()
        {
            _currentRuntime.Start();
        }

        public override void Update()
        {
            _currentRuntime.Update();
        }

        public override void Pause()
        {
            _currentRuntime.Pause();
        }

        public override void Resume()
        {
            _currentRuntime.Resume();
        }

        public override void End()
        {
            _currentRuntime.End();
        }

        public override PhaseRuntimeState CheckPhaseRuntimeState()
        {
            return _currentRuntime.CheckPhaseRuntimeState();
        }
    }
}