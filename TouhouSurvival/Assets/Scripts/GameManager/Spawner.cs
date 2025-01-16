using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Unchord
{
    public static class Spawner
    {
        private const int INITIAL_INSTANTIATED_GAMEOBJECT_CAPACITY = 32;

        private static System.Random _prng;
        private static Queue<GameObject> _instantiatedGameObjects;
        private static ReadOnlyCollection<GameObject> _instantiatedGameObjectsHandle;
        
        static Spawner()
        {
            Spawner._prng = new System.Random();
            Spawner._instantiatedGameObjects = new Queue<GameObject>(INITIAL_INSTANTIATED_GAMEOBJECT_CAPACITY);
        }

        public static ReadOnlyCollection<GameObject> Spawn(SpawnerSO spawnerSO)
        {
            switch (spawnerSO.spawnShape)
            {
                case SpawnShape.Single:
                    EnqueuePrefabs(spawnerSO);
                    SpawnSingle(spawnerSO);
                    return Spawner._instantiatedGameObjectsHandle;

                case SpawnShape.Group:
                    EnqueuePrefabs(spawnerSO);
                    SpawnGroup(spawnerSO);
                    return Spawner._instantiatedGameObjectsHandle;

                case SpawnShape.Circular:
                    EnqueuePrefabs(spawnerSO);
                    SpawnCircular(spawnerSO);
                    return Spawner._instantiatedGameObjectsHandle;

                default:
                    Debug.Assert(false, "Unknown case handling occured.");
                    return null;
            }
        }

        private static void EnqueuePrefabs(SpawnerSO spawnerSO)
        {
            int prefabIndex = GetRandomPrefabIndex(spawnerSO);

            for (int i = 0; i < spawnerSO.spawnCountAtOnce; ++i)
            {
                if (spawnerSO.mixEntityAtOnce && i > 0)
                    prefabIndex = GetRandomPrefabIndex(spawnerSO);

                GameObject instantiatedGameObject = GameObject.Instantiate(spawnerSO.spawnDataList[prefabIndex].entityPrefab);
                Spawner._instantiatedGameObjects.Enqueue(instantiatedGameObject);
            }

            Spawner._instantiatedGameObjectsHandle = new ReadOnlyCollection<GameObject>(_instantiatedGameObjects.ToArray());
        }

        private static int GetRandomPrefabIndex(SpawnerSO spawnerSO)
        {
            List<SpawnData> spawnDataList = spawnerSO.spawnDataList;
            int selectedIndex = 0;
            int spawnRatioSum = spawnDataList[0].spawnRatio;

            for (int i = 1; i < spawnDataList.Count; ++i)
            {
                spawnRatioSum += spawnDataList[i].spawnRatio;

                if (_prng.Next(spawnRatioSum) < spawnDataList[i].spawnRatio)
                    selectedIndex = i;
            }

            return selectedIndex;
        }

        private static void SpawnSingle(SpawnerSO spawnerSO)
        {
            // TODO: spawnPositionFlag를 이용해 어디애 객체를 생성할지 결정해야 합니다.
            SpawnPositionFlag spawnPositionFlag = spawnerSO.spawnPositionFlag;

            Debug.LogFormat("Spawn with single mode, entity count == {0}", _instantiatedGameObjects.Count);
            SetPositionRandom();
        }

        private static void SpawnGroup(SpawnerSO spawnerSO)
        {
            // TODO: spawnPositionFlag를 이용해 어디애 객체를 생성할지 결정해야 합니다.
            SpawnPositionFlag spawnPositionFlag = spawnerSO.spawnPositionFlag;

            Debug.LogFormat("Spawn with group mode, entity count == {0}", _instantiatedGameObjects.Count);
            SetPositionRandom();
        }

        private static void SpawnCircular(SpawnerSO spawnerSO)
        {
            // TODO: spawnPositionFlag를 이용해 어디애 객체를 생성할지 결정해야 합니다.
            SpawnPositionFlag spawnPositionFlag = spawnerSO.spawnPositionFlag;

            Debug.LogFormat("Spawn with circular mode, entity count == {0}", _instantiatedGameObjects.Count);
            SetPositionRandom();
        }

        private static void SetPositionRandom()
        {
            while (Spawner._instantiatedGameObjects.Count > 0)
            {
                GameObject prefab = Spawner._instantiatedGameObjects.Dequeue();

                float radianAngle = UnityEngine.Random.value * Mathf.PI * 2.0f;
                float distance = 10.0f;
                float cos = Mathf.Cos(radianAngle);
                float sin = Mathf.Sin(radianAngle);
                Vector2 position = new Vector2(cos, sin) * distance;
                prefab.transform.position = position;
            }
        }
    }
}