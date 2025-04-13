using UnityEngine;

namespace Unchord
{
    public sealed class SurvivalPhaseRuntime : PhaseRuntime<SurvivalPhaseDataSO>
    {
        private GameManager _gm;
        private SpawnerRuntime[] _spawners;

        private float _startTime;

        private bool _interruptedHalt = false;

        public SurvivalPhaseRuntime(SurvivalPhaseDataSO phase)
        : base(phase)
        {
            _gm = GameManager.Instance;
            _spawners = new SpawnerRuntime[RuntimeData.spawnerSO.Count];

            for (int i = 0; i < _spawners.Length; ++i)
            {
                _spawners[i] = RuntimeData.spawnerSO[i].CreateRuntime() as SpawnerRuntime;
                _spawners[i].onSpawnSuccess += OnEnemySpawned;
                _spawners[i].onSpawnSuccess += _gm.OnEnemySpawned;
            }
        }

        public override void Start()
        {
            base.Start();

            _startTime = _gm.ElapsedPlaytime;
            _gm.ShouldUpdateElapsedPlaytime = true;
        }

        public override RuntimeState Update()
        {
            if (_interruptedHalt)
                return RuntimeState.Fail;

            for (int i = 0; i < _spawners.Length; ++i)
            {
                _spawners[i].TrySpawn();
            }

            float execTime = _gm.ElapsedPlaytime - _startTime;

            if (execTime < RuntimeData.phaseDuration)
                return RuntimeState.Continue;
            else
                return RuntimeState.Pass;
        }

        public override void Pause()
        {
            base.Pause();

            _gm.InterruptTimeStop();
        }

        public override void Resume()
        {
            base.Resume();

            _gm.ReleaseTimeStopInterrupt();
        }

        public override void End()
        {
            base.End();

            Debug.Log("Survival Phase End");
        }

        public override void InterruptHalt()
        {
            base.InterruptHalt();

            _interruptedHalt = true;
        }

        private void OnEnemySpawned(object sender, SpawnEventArgs args)
        {

        }
    }
}