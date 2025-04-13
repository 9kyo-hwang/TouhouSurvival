using System.Collections.Generic;
using UnityEngine;

namespace Unchord
{
    public sealed class BossPhaseRuntime : PhaseRuntime<BossPhaseDataSO>
    {
        private GameManager _gm;

        private List<GameObject> _spawnedBossList;
        private List<GameObject> _spawnedOtherEnemyList;

        private SpawnerRuntime[] _bossSpawners;
        private SpawnerRuntime[] _otherEnemySpawners;

        private bool _interruptedHalt = false;

        public BossPhaseRuntime(BossPhaseDataSO phase)
        : base(phase)
        {
            _gm = GameManager.Instance;

            _spawnedBossList = new List<GameObject>(2);
            _spawnedOtherEnemyList = new List<GameObject>(32);

            _bossSpawners = new SpawnerRuntime[RuntimeData.bossSpawnerSO.Count];
            _otherEnemySpawners = new SpawnerRuntime[RuntimeData.additionalSpawnerSO.Count];
        }

        public override void Start()
        {
            base.Start();

            for (int i = 0; i < RuntimeData.bossSpawnerSO.Count; ++i)
            {
                _bossSpawners[i] = RuntimeData.bossSpawnerSO[i].CreateRuntime() as SpawnerRuntime;
                _bossSpawners[i].onSpawnSuccess += OnBossSpawned;
                _bossSpawners[i].onSpawnSuccess += _gm.OnEnemySpawned;
            }

            for (int i = 0; i < RuntimeData.additionalSpawnerSO.Count; ++i)
            {
                _otherEnemySpawners[i] = RuntimeData.additionalSpawnerSO[i].CreateRuntime() as SpawnerRuntime;
                _otherEnemySpawners[i].onSpawnSuccess += OnOtherEnemySpawned;
                _otherEnemySpawners[i].onSpawnSuccess += _gm.OnEnemySpawned;
            }
        }

        public override RuntimeState Update()
        {
            if (_interruptedHalt)
                return RuntimeState.Fail;

            _gm.ShouldUpdateElapsedPlaytime = !RuntimeData.useTimerStop;

            bool canPassRuntime = true;


            for (int i = 0; i < _bossSpawners.Length; ++i)
            {
                canPassRuntime &= _bossSpawners[i].SpawnedCount > 0 || _bossSpawners[i].TrySpawn();
            }

            for (int i = _spawnedBossList.Count - 1; i >= 0; --i)
            {
                if (_spawnedBossList[i] == null)
                    _spawnedBossList.RemoveAt(i);
            }

            canPassRuntime &= (_spawnedBossList.Count == 0);

            return canPassRuntime ? RuntimeState.Pass : RuntimeState.Continue;
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

            Debug.Log("Boss Phase End");
        }

        public override void InterruptHalt()
        {
            base.InterruptHalt();

            _interruptedHalt = true;
        }

        private void OnBossSpawned(object sender, SpawnEventArgs args)
        {
            _spawnedBossList.Add(args.spawnedInstance);
        }

        private void OnOtherEnemySpawned(object sender, SpawnEventArgs args)
        {
            _spawnedOtherEnemyList.Add(args.spawnedInstance);
        }
    }
}