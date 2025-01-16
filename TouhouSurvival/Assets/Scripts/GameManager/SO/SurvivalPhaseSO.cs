using System.Collections.Generic;
using UnityEngine;

namespace Unchord
{
    [CreateAssetMenu(fileName = "New Survival Phase", menuName = "ScriptableObjects/Game Management/Survival Phase", order = (int)GameManagerAssetMenuOrder.SurvivalPhaseSO)]
    public class SurvivalPhaseSO : PhaseSO
    {
        public List<SpawnerSO> spawnerSoList;
        public float phaseDuration = 10.0f;
    }
}