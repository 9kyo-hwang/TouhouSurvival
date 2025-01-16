using System.Collections.Generic;
using UnityEngine;

namespace Unchord
{
    [CreateAssetMenu(fileName = "New Boss Phase", menuName = "ScriptableObjects/Game Management/Boss Phase", order = (int)GameManagerAssetMenuOrder.BossPhaseSO)]
    public class BossPhaseSO : PhaseSO
    {
        public SpawnerSO bossSpawnerSO;
        public List<SpawnerSO> miscellaneousSpawnerSoList;

        public bool useTimerStop = true;
    }
}