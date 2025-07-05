using System.Collections.Generic;
using UnityEngine;

namespace Unchord
{
    [CreateAssetMenu(fileName = "New Boss Phase", menuName = "Scriptable Objects/Game Management/Boss Phase", order = (int)GameManagerAssetMenuOrder.BossPhaseSO)]
    public sealed class BossPhaseDataSO : PhaseDataSO
    {
        public List<SpawnerSO> bossSpawnerSO;
        public List<SpawnerSO> additionalSpawnerSO;

        public bool useTimerStop = true;

        public override IRuntime CreateRuntime(PhaseRuntimeCommons commonData) => new BossPhaseRuntime(this, commonData);
    }
}