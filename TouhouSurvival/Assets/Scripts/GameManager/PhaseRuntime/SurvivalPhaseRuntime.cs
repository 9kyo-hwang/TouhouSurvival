using System.Collections.Generic;
using UnityEngine;

namespace Unchord
{
    public class SurvivalPhaseRuntime : PhaseRuntime
    {
        private float _createdPlaytime;

        private float[] _spawnerNextSpawnTimes;

        public SurvivalPhaseRuntime(SurvivalPhaseSO phaseSO)
        : base(phaseSO)
        {
            _createdPlaytime = s_gameManager.ElapsedPlaytime;
            _spawnerNextSpawnTimes = new float[phaseSO.spawnerSoList.Count];
        }

        public override void Start()
        {
            Debug.Log("Survival phase starts.");

            SurvivalPhaseSO phaseSO = (SurvivalPhaseSO)_phaseSO;

            for (int i = 0; i < phaseSO.spawnerSoList.Count; ++i)
            {
                MarkNextSpawnTime(phaseSO.spawnerSoList, i);
            }

            s_gameManager.ShouldUpdateElapsedPlaytime = true;
        }

        public override void Update()
        {
            SurvivalPhaseSO phaseSO = (SurvivalPhaseSO)_phaseSO;

            for (int i = 0; i < phaseSO.spawnerSoList.Count; ++i)
            {
                if (s_gameManager.ElapsedPlaytime < _spawnerNextSpawnTimes[i])
                    continue;

                Spawner.Spawn(phaseSO.spawnerSoList[i]);
                MarkNextSpawnTime(phaseSO.spawnerSoList, i);
            }
        }

        public override void Pause()
        {
            s_gameManager.InterruptTimeStop();
        }

        public override void Resume()
        {
            s_gameManager.ReleaseTimeStopInterrupt();
        }

        public override void End()
        {
            Debug.Log("Survival phase ends.");
        }

        public override PhaseRuntimeState CheckPhaseRuntimeState()
        {
            SurvivalPhaseSO survivalPhaseSO = (SurvivalPhaseSO)_phaseSO;
            float runtimeExecutionTime = s_gameManager.ElapsedPlaytime - _createdPlaytime;

            // TODO: Add following code here; if, player's health is lower than or equal 0, then, return PhaseRuntimeState.Fail;

            if (runtimeExecutionTime < survivalPhaseSO.phaseDuration)
                return PhaseRuntimeState.Continue;
            else
                return PhaseRuntimeState.Pass;
        }

        private void MarkNextSpawnTime(List<SpawnerSO> spawnerSoList, int index)
        {
            float min = spawnerSoList[index].minSpawnCooltime;
            float max = spawnerSoList[index].maxSpawnCooltime;
            float cooltime = (max - min) * UnityEngine.Random.value + min;
            _spawnerNextSpawnTimes[index] = s_gameManager.ElapsedPlaytime + cooltime;
        }
    }
}