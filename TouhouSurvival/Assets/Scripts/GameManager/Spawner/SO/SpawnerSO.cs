using System.Collections.Generic;
using UnityEngine;

namespace Unchord
{
    [CreateAssetMenu(fileName = "New Spawner", menuName = "Scriptable Objects/Game Management/Spawner", order = (int)GameManagerAssetMenuOrder.SpawnerSO)]
    public class SpawnerSO : RuntimeDataSO
    {
        public List<GameObject> prefabs;
        public List<float> prefabRatios;
        public float minCooldown = 1.0f;
        public float maxCooldown = 1.0f;
        public float initLeftCooldown = 0.0f;
        public float spawnRatio = 1.0f;
        public int maxSpawnCount = 0;
        public PositionFlag positionFlag;
        public int spawnCountAtOnce = 1;
        public bool mixEntityAtOnce = false;
        public SpawnShape spawnShape = SpawnShape.Single;

        public IRuntime CreateRuntime(PhaseRuntimeCommons commonData) => new SpawnerRuntime(this, commonData);
    }
}