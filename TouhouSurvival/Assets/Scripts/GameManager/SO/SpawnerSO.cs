using System.Collections.Generic;
using UnityEngine;

namespace Unchord
{
    [CreateAssetMenu(fileName = "New Spawner", menuName = "ScriptableObjects/Game Management/Spawner", order = (int)GameManagerAssetMenuOrder.SpawnerSO)]
    public class SpawnerSO : ScriptableObject
    {
        public List<SpawnData> spawnDataList;
        public float minSpawnCooltime = 1.0f;
        public float maxSpawnCooltime = 1.0f;
        public SpawnPositionFlag spawnPositionFlag;
        public int spawnCountAtOnce = 1;
        public bool mixEntityAtOnce = false;
        public SpawnShape spawnShape = SpawnShape.Single;
    }
}