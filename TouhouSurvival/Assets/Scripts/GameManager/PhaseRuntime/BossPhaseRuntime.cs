using System.Collections.ObjectModel;
using UnityEngine;

namespace Unchord
{
    public class BossPhaseRuntime : PhaseRuntime
    {
        private int _ptrHandle = 0;
        private ReadOnlyCollection<GameObject> _bossHandle;

        public BossPhaseRuntime(BossPhaseSO phaseSO)
        : base(phaseSO)
        {

        }

        public override void Start()
        {
            BossPhaseSO phaseSO = (BossPhaseSO)_phaseSO;

            _bossHandle = Spawner.Spawn(phaseSO.bossSpawnerSO);

            Debug.Assert(phaseSO.miscellaneousSpawnerSoList != null);

            for (int i = 0; i < phaseSO.miscellaneousSpawnerSoList.Count; ++i)
            {
                Debug.Assert(phaseSO.miscellaneousSpawnerSoList[i] != null);
                Spawner.Spawn(phaseSO.miscellaneousSpawnerSoList[i]);
            }

            s_gameManager.ShouldUpdateElapsedPlaytime = phaseSO.useTimerStop;
        }

        public override void Update()
        {
            BossPhaseSO phaseSO = (BossPhaseSO)_phaseSO;

            while (_ptrHandle < _bossHandle.Count && _bossHandle[_ptrHandle] == null)
            {
                _ptrHandle++;
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
            Debug.Log("Boss phase ends.");
        }

        public override PhaseRuntimeState CheckPhaseRuntimeState()
        {
            BossPhaseSO bossPhaseSO = (BossPhaseSO)_phaseSO;

            // TODO: Add following code here; if, player's health is lower than or equal 0, then, return PhaseRuntimeState.Fail;

            if (_ptrHandle == _bossHandle.Count)
                return PhaseRuntimeState.Pass;
            else
                return PhaseRuntimeState.Continue;
        }
    }
}