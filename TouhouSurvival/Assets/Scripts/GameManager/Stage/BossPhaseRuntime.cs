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

        public BossPhaseRuntime(BossPhaseDataSO phase, PhaseRuntimeCommons commonData)
        : base(phase, commonData)
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
                _bossSpawners[i] = RuntimeData.bossSpawnerSO[i].CreateRuntime(base.CommonData) as SpawnerRuntime;
                _bossSpawners[i].onSpawnSuccess += OnBossSpawned;
                _bossSpawners[i].onSpawnSuccess += _gm.OnEnemySpawned;
            }

            for (int i = 0; i < RuntimeData.additionalSpawnerSO.Count; ++i)
            {
                _otherEnemySpawners[i] = RuntimeData.additionalSpawnerSO[i].CreateRuntime(base.CommonData) as SpawnerRuntime;
                _otherEnemySpawners[i].onSpawnSuccess += OnOtherEnemySpawned;
                _otherEnemySpawners[i].onSpawnSuccess += _gm.OnEnemySpawned;
            }
        }

        public override RuntimeState Update()
        {
            _gm.ShouldUpdateElapsedPlaytime = !RuntimeData.useTimerStop;

            bool canPassRuntime = true;

            for (int i = 0; i < _bossSpawners.Length; ++i)
            {
                if (_bossSpawners[i].SpawnedCount == 0)
                {
                    canPassRuntime = false;
                    _bossSpawners[i].TrySpawn();
                }
                else
                {
                    canPassRuntime &= (_bossSpawners[i].SpawnedObjectCount == 0);
                }
            }

            for (int i = 0; i < _otherEnemySpawners.Length; ++i)
            {
                _otherEnemySpawners[i].TrySpawn();
            }

            return canPassRuntime ? RuntimeState.Pass : RuntimeState.Continue;
        }

        public override void End()
        {
            base.End();

            Debug.Log("Boss Phase End");
        }

        public override void InterruptResurrect()
        {
            base.InterruptResurrect();
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