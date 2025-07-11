using System.Collections.Generic;
using UnityEngine;

namespace Unchord
{
    [CreateAssetMenu(fileName = "New Survival Phase", menuName = "Scriptable Objects/Game Management/Survival Phase", order = (int)GameManagerAssetMenuOrder.SurvivalPhaseSO)]
    public sealed class SurvivalPhaseDataSO : PhaseDataSO
    {
        public List<SpawnerSO> spawnerSO;
        public float phaseDuration = 30.0f;

        public override IRuntime CreateRuntime(PhaseRuntimeCommons commonData) => new SurvivalPhaseRuntime(this, commonData);
    }
}