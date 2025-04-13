namespace Unchord
{
    public class StageRuntime : PhaseRuntime<StageDataSO>
    {
        private GameManager _gm;

        private Map _map;
        private IPhase _compositeRuntime;

        public StageRuntime(StageDataSO phase)
        : base(phase)
        {
            _gm = GameManager.Instance;

            _map = Map.Create(RuntimeData.mapSO);
            _compositeRuntime = new CompositePhaseRuntime(RuntimeData.phaseList);

            _map.transform.parent = _gm.RuntimeContainer;
        }

        public override void Start()
        {
            _compositeRuntime.Start();
        }

        public override RuntimeState Update()
        {
            _map.ScrollMap(_gm.MainCamera);

            return _compositeRuntime.Update();
        }

        public override void Pause()
        {
            _compositeRuntime.Pause();
        }

        public override void Resume()
        {
            _compositeRuntime.Resume();
        }

        public override void End()
        {
            _compositeRuntime.End();
        }

        public override void InterruptHalt()
        {
            _compositeRuntime.InterruptHalt();
        }
    }
}