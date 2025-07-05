using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unchord
{
    public class SpawnerRuntime : Runtime<SpawnerSO>
        , IInterruptableResurrect
    {
        public float LeftCooldown { get; private set; }
        public int SpawnedCount { get; private set; }
        public int SpawnedObjectCount
        {
            get
            {
                EnsureValidObjectsOnArray();

                return _spawnedObjectCount;
            }
        }

        protected PhaseRuntimeCommons CommonData { get; private set; }

        public event EventHandler<SpawnEventArgs> onSpawnSuccess;

        // 성능 향상을 위해 List<GameObject> 컬렉션을 사용하지 않고 동적 배열을 직접 구현함.
        // 생성된 순서에 대해 Unstable한 알고리즘으로 배열을 관리함.
        private GameObject[] _spawnedObjects;
        private int _spawnedObjectCount;

        private GameManager _gm;

        public SpawnerRuntime(SpawnerSO data, PhaseRuntimeCommons commonData)
        : base(data)
        {
            _spawnedObjects = new GameObject[32];
            _spawnedObjectCount = 0;
            _gm = GameManager.Instance;

            LeftCooldown = data.initLeftCooldown;
            SpawnedCount = 0;

            CommonData = commonData;
        }

        public bool TrySpawn()
        {
            if (RuntimeData.maxSpawnCount > 0 && SpawnedCount >= RuntimeData.maxSpawnCount)
                return false;

            if (LeftCooldown > 0.0f)
            {
                LeftCooldown -= Time.deltaTime;
                return false;
            }

            LeftCooldown += GetRandomCooldown();

            UnityEngine.Debug.Assert(RuntimeData.spawnRatio >= 0.0f);
            UnityEngine.Debug.Assert(RuntimeData.spawnRatio <= 1.0f);

            if (RuntimeData.spawnRatio <= 0.0f || RuntimeData.spawnRatio < UnityEngine.Random.value)
                return false;

            EnsureValidObjectsOnArray();

            SpawnedCount++;

            switch (RuntimeData.spawnShape)
            {
                case SpawnShape.Single:
                    SpawnAsSingleShape();
                    return true;

                case SpawnShape.Group:
                    SpawnAsGroupShape();
                    return true;

                case SpawnShape.Circular:
                    SpawnAsCircularShape();
                    return true;

                default:
                    UnityEngine.Debug.Assert(false);
                    return false;
            }
        }

        private float GetRandomCooldown()
        {
            UnityEngine.Debug.Assert(0.0f <= RuntimeData.minCooldown);
            UnityEngine.Debug.Assert(RuntimeData.minCooldown <= RuntimeData.maxCooldown);

            float w = UnityEngine.Random.value;
            float min = RuntimeData.minCooldown;
            float max = RuntimeData.maxCooldown;

            return min + (max - min) * w;
        }

        private int GetRandomPrefabIndex()
        {
            List<GameObject> prefabs = RuntimeData.prefabs;
            List<float> ratios = RuntimeData.prefabRatios;

            UnityEngine.Debug.Assert(prefabs != null && prefabs.Count > 0);
            UnityEngine.Debug.Assert(ratios != null && ratios.Count > 0);
            UnityEngine.Debug.Assert(prefabs.Count == ratios.Count);

            // Weighted Reservoir Sampling (Efraimidis and Spirakis) Algorithm
            int index = -1;
            float maxKey = float.MinValue;

            for (int i = 0; i < prefabs.Count; ++i)
            {
                UnityEngine.Debug.Assert(prefabs[i] != null);
                UnityEngine.Debug.Assert(ratios[i] >= 0.0f);

                if (ratios[i] <= 0.0f)
                    continue;

                float u = 1.0f - UnityEngine.Random.value;
                float key = Mathf.Pow(u, 1.0f / ratios[i]);

                if (key > maxKey)
                {
                    maxKey = key;
                    index = i;
                }
            }

            return index;
        }

        private void SpawnAsSingleShape()
        {
            int count = RuntimeData.spawnCountAtOnce;
            int prefabIndex = -1;

            EnsureObjectListCapacity(count);

            for (int i = 0; i < count; ++i)
            {
                if (RuntimeData.mixEntityAtOnce || i == 0)
                    prefabIndex = GetRandomPrefabIndex();

                UnityEngine.Debug.Assert(prefabIndex >= 0);
                UnityEngine.Debug.Assert(prefabIndex < RuntimeData.prefabs.Count);

                Spawn(RuntimeData.prefabs[prefabIndex]);
            }
        }

        private void SpawnAsGroupShape()
        {
            throw new NotImplementedException();
        }

        private void SpawnAsCircularShape()
        {
            throw new NotImplementedException();
        }

        private void Spawn(GameObject prefab)
        {
            PositionFlag pFlag = RuntimeData.positionFlag;
            Camera camera = GameManager.Instance.MainCamera;

            GameObject instance = GameObject.Instantiate(prefab);
            Vector2 spawnedPosition = WorldPosition.GetRandomPosition(pFlag, camera);

            instance.name = prefab.name;
            instance.transform.position = spawnedPosition;
            instance.transform.parent = _gm.RuntimeContainer;

            SpawnEventArgs args = new SpawnEventArgs();
            args.spawnerRuntime = this;
            args.spawnedInstance = instance;
            args.spawnedPosition = spawnedPosition;

            _spawnedObjects[_spawnedObjectCount++] = instance;
            CommonData.spawnedObjects.Add(instance);
            onSpawnSuccess?.Invoke(this, args);
        }

        #region Dynamic Array Handling
        private void EnsureValidObjectsOnArray()
        {
            // Unstable Algorithm.
            for (int i = _spawnedObjectCount - 1; i >= 0; --i)
            {
                if (_spawnedObjects[i] == null)
                {
                    _spawnedObjects[i] = _spawnedObjects[--_spawnedObjectCount];
                }
            }
        }

        private void EnsureObjectListCapacity(int countToAdd)
        {
            UnityEngine.Debug.Assert(countToAdd >= 0);

            int newCapacity = _spawnedObjects.Length;

            while (_spawnedObjectCount + countToAdd > newCapacity)
                newCapacity *= 2;

            if (newCapacity == _spawnedObjects.Length)
                return;

            GameObject[] newArray = new GameObject[newCapacity];

            Array.Copy(_spawnedObjects, 0, newArray, 0, _spawnedObjectCount);

            _spawnedObjects = newArray;
        }
        #endregion

        public void InterruptResurrect()
        {
            for (int i = _spawnedObjectCount - 1; i >= 0; --i)
            {
                if (_spawnedObjects[i] != null)
                {
                    _spawnedObjects[i].GetComponent<Pawn>()?.Die();
                }
            }

            _spawnedObjectCount = 0;
        }
    }
}