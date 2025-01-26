using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Unchord
{
    public static class Spawner
    {
        private const int INITIAL_INSTANTIATED_GAMEOBJECT_CAPACITY = 32;

        private static System.Random s_prng;
        private static List<GameObject> s_instances;

        static Spawner()
        {
            Spawner.s_prng = new System.Random();
            Spawner.s_instances = new List<GameObject>(INITIAL_INSTANTIATED_GAMEOBJECT_CAPACITY);
        }

        public static ReadOnlyCollection<GameObject> Spawn(SpawnerSO spawnerSO)
        {
            switch (spawnerSO.spawnShape)
            {
                case SpawnShape.Single:
                    return CreateInstances_Single(spawnerSO);

                case SpawnShape.Group:
                    return CreateInstances_Group(spawnerSO);

                case SpawnShape.Circular:
                    return CreateInstances_Circular(spawnerSO);

                default:
                    Debug.Assert(false, "Unknown case handling occured.");
                    return null;
            }
        }

        private static ReadOnlyCollection<GameObject> CreateInstances_Single(SpawnerSO spawnerSO)
        {
            int prefabIndex = GetRandomPrefabIndex(spawnerSO);

            s_instances.Clear();

            for (int i = 0; i < spawnerSO.spawnCountAtOnce; ++i)
            {
                if (spawnerSO.mixEntityAtOnce && i > 0)
                    prefabIndex = GetRandomPrefabIndex(spawnerSO);

                GameObject instance = GameObject.Instantiate(spawnerSO.spawnDataList[prefabIndex].entityPrefab, GameManager.Instance.RuntimeContainer, true);
                Vector2 randomPosition = GetRandomPosition(spawnerSO.spawnPositionFlag);
                instance.transform.position = randomPosition;
                s_instances.Add(instance);
            }

            Debug.LogFormat("Spawn with single mode, entity count == {0}", s_instances.Count);

            return new ReadOnlyCollection<GameObject>(s_instances);
        }

        private static ReadOnlyCollection<GameObject> CreateInstances_Group(SpawnerSO spawnerSO)
        {
            // TODO: spawnPositionFlag�� �̿��� ���� ��ü�� �������� �����ؾ� �մϴ�.
            SpawnPositionFlag spawnPositionFlag = spawnerSO.spawnPositionFlag;

            // Debug.LogFormat("Spawn with group mode, entity count == {0}", _instantiatedGameObjects.Count);
            // SetPositionRandom();
            throw new NotImplementedException();
        }

        private static ReadOnlyCollection<GameObject> CreateInstances_Circular(SpawnerSO spawnerSO)
        {
            // TODO: spawnPositionFlag�� �̿��� ���� ��ü�� �������� �����ؾ� �մϴ�.
            SpawnPositionFlag spawnPositionFlag = spawnerSO.spawnPositionFlag;

            // Debug.LogFormat("Spawn with circular mode, entity count == {0}", _instantiatedGameObjects.Count);
            // SetPositionRandom();
            throw new NotImplementedException();
        }

        private static int GetRandomPrefabIndex(SpawnerSO spawnerSO)
        {
            List<SpawnData> spawnDataList = spawnerSO.spawnDataList;
            int selectedIndex = 0;
            int spawnRatioSum = spawnDataList[0].spawnRatio;

            for (int i = 1; i < spawnDataList.Count; ++i)
            {
                spawnRatioSum += spawnDataList[i].spawnRatio;

                if (s_prng.Next(spawnRatioSum) < spawnDataList[i].spawnRatio)
                    selectedIndex = i;
            }

            return selectedIndex;
        }

        private static Vector2 GetRandomPosition(SpawnPositionFlag positionFlags, float hiddenZoneWidth = 0.2f, float hiddenZoneHeight = 0.2f)
        {
            Debug.Assert(hiddenZoneWidth >= 0.0f && hiddenZoneHeight >= 0.0f);

            SpawnPositionFlag selectedFlag = SpawnPositionFlag.None;
            int flagCount = 0;

            foreach (SpawnPositionFlag flag in Enum.GetValues(typeof(SpawnPositionFlag)))
            {
                if ((positionFlags & flag) == SpawnPositionFlag.None)
                    continue;

                if (UnityEngine.Random.Range(0, ++flagCount) == 0)
                    selectedFlag = flag;
            }

            Camera camera = GameManager.Instance.MainCamera;
            Debug.Log(selectedFlag.ToString());
            switch (selectedFlag)
            {
                case SpawnPositionFlag.None:
                    Debug.Assert(false, "SpawnPositionFlag cannot be None.");
                    return Vector2.zero;
                case SpawnPositionFlag.OutOfL:
                    return camera.ViewportToWorldPoint(GetRandomPosition_OutOfL(hiddenZoneWidth, hiddenZoneHeight));
                case SpawnPositionFlag.OutOfT:
                    return camera.ViewportToWorldPoint(GetRandomPosition_OutOfT(hiddenZoneWidth, hiddenZoneHeight));
                case SpawnPositionFlag.OutOfR:
                    return camera.ViewportToWorldPoint(GetRandomPosition_OutOfR(hiddenZoneWidth, hiddenZoneHeight));
                case SpawnPositionFlag.OutOfB:
                    return camera.ViewportToWorldPoint(GetRandomPosition_OutOfB(hiddenZoneWidth, hiddenZoneHeight));
                case SpawnPositionFlag.QuarterOfL:
                    return camera.ViewportToWorldPoint(GetRandomPosition_QuarterOfL(hiddenZoneWidth, hiddenZoneHeight));
                case SpawnPositionFlag.QuarterOfT:
                    return camera.ViewportToWorldPoint(GetRandomPosition_QuarterOfT(hiddenZoneWidth, hiddenZoneHeight));
                case SpawnPositionFlag.QuarterOfR:
                    return camera.ViewportToWorldPoint(GetRandomPosition_QuarterOfR(hiddenZoneWidth, hiddenZoneHeight));
                case SpawnPositionFlag.QuarterOfB:
                    return camera.ViewportToWorldPoint(GetRandomPosition_QuarterOfB(hiddenZoneWidth, hiddenZoneHeight));
                case SpawnPositionFlag.HalfOfL:
                    return camera.ViewportToWorldPoint(GetRandomPosition_HalfOfL(hiddenZoneWidth, hiddenZoneHeight));
                case SpawnPositionFlag.HalfOfT:
                    return camera.ViewportToWorldPoint(GetRandomPosition_HalfOfT(hiddenZoneWidth, hiddenZoneHeight));
                case SpawnPositionFlag.HalfOfR:
                    return camera.ViewportToWorldPoint(GetRandomPosition_HalfOfR(hiddenZoneWidth, hiddenZoneHeight));
                case SpawnPositionFlag.HalfOfB:
                    return camera.ViewportToWorldPoint(GetRandomPosition_HalfOfB(hiddenZoneWidth, hiddenZoneHeight));
                case SpawnPositionFlag.OriginOfMap:
                    return camera.ViewportToWorldPoint(0.5f * Vector2.one);
                case SpawnPositionFlag.RandomOfMap:
                    return camera.ViewportToWorldPoint(GetRandomPosition_OnMap(hiddenZoneWidth, hiddenZoneHeight));
                default:
                    Debug.Assert(false, "Invalid case occurred. Please debug.");
                    return Vector2.zero;
            }
        }

        private static Vector2 GetRandomPosition_OutOfL(float hiddenZoneWidth, float hiddenZoneHeight)
        {
            Vector2 viewportPositionMin = new Vector2(-hiddenZoneWidth, -hiddenZoneHeight);
            Vector2 viewportPositionMax = new Vector2(0.0f, 1.0f + hiddenZoneHeight);
            
            return GetRandomPositionOnSquare(viewportPositionMin, viewportPositionMax);
        }

        private static Vector2 GetRandomPosition_OutOfT(float hiddenZoneWidth, float hiddenZoneHeight)
        {
            Vector2 viewportPositionMin = new Vector2(-hiddenZoneWidth, 1.0f);
            Vector2 viewportPositionMax = new Vector2(1.0f + hiddenZoneWidth, 1.0f + hiddenZoneHeight);

            return GetRandomPositionOnSquare(viewportPositionMin, viewportPositionMax);
        }

        private static Vector2 GetRandomPosition_OutOfR(float hiddenZoneWidth, float hiddenZoneHeight)
        {
            Vector2 viewportPositionMin = new Vector2(1.0f, -hiddenZoneHeight);
            Vector2 viewportPositionMax = new Vector2(1.0f + hiddenZoneWidth, 1.0f + hiddenZoneHeight);

            return GetRandomPositionOnSquare(viewportPositionMin, viewportPositionMax);
        }

        private static Vector2 GetRandomPosition_OutOfB(float hiddenZoneWidth, float hiddenZoneHeight)
        {
            Vector2 viewportPositionMin = new Vector2(-hiddenZoneWidth, -hiddenZoneHeight);
            Vector2 viewportPositionMax = new Vector2(1.0f + hiddenZoneWidth, 0.0f);

            return GetRandomPositionOnSquare(viewportPositionMin, viewportPositionMax);
        }

        private static Vector2 GetRandomPosition_QuarterOfL(float hiddenZoneWidth, float hiddenZoneHeight)
        {
            Vector2 viewportPositionMin = new Vector2(0.125f, 0.125f);
            Vector2 viewportPositionMax = new Vector2(0.125f, 0.875f);

            return GetRandomPositionOnSquare(viewportPositionMin, viewportPositionMax);
        }

        private static Vector2 GetRandomPosition_QuarterOfT(float hiddenZoneWidth, float hiddenZoneHeight)
        {
            Vector2 viewportPositionMin = new Vector2(0.125f, 0.875f);
            Vector2 viewportPositionMax = new Vector2(0.875f, 0.875f);

            return GetRandomPositionOnSquare(viewportPositionMin, viewportPositionMax);
        }

        private static Vector2 GetRandomPosition_QuarterOfR(float hiddenZoneWidth, float hiddenZoneHeight)
        {
            Vector2 viewportPositionMin = new Vector2(0.875f, 0.125f);
            Vector2 viewportPositionMax = new Vector2(0.875f, 0.875f);

            return GetRandomPositionOnSquare(viewportPositionMin, viewportPositionMax);
        }

        private static Vector2 GetRandomPosition_QuarterOfB(float hiddenZoneWidth, float hiddenZoneHeight)
        {
            Vector2 viewportPositionMin = new Vector2(0.125f, 0.125f);
            Vector2 viewportPositionMax = new Vector2(0.875f, 0.125f);

            return GetRandomPositionOnSquare(viewportPositionMin, viewportPositionMax);
        }

        private static Vector2 GetRandomPosition_HalfOfL(float hiddenZoneWidth, float hiddenZoneHeight)
        {
            Vector2 viewportPositionMin = new Vector2(0.25f, 0.25f);
            Vector2 viewportPositionMax = new Vector2(0.25f, 0.75f);

            return GetRandomPositionOnSquare(viewportPositionMin, viewportPositionMax);
        }

        private static Vector2 GetRandomPosition_HalfOfT(float hiddenZoneWidth, float hiddenZoneHeight)
        {
            Vector2 viewportPositionMin = new Vector2(0.25f, 0.75f);
            Vector2 viewportPositionMax = new Vector2(0.75f, 0.75f);

            return GetRandomPositionOnSquare(viewportPositionMin, viewportPositionMax);
        }

        private static Vector2 GetRandomPosition_HalfOfR(float hiddenZoneWidth, float hiddenZoneHeight)
        {
            Vector2 viewportPositionMin = new Vector2(0.75f, 0.25f);
            Vector2 viewportPositionMax = new Vector2(0.75f, 0.75f);

            return GetRandomPositionOnSquare(viewportPositionMin, viewportPositionMax);
        }

        private static Vector2 GetRandomPosition_HalfOfB(float hiddenZoneWidth, float hiddenZoneHeight)
        {
            Vector2 viewportPositionMin = new Vector2(0.25f, 0.25f);
            Vector2 viewportPositionMax = new Vector2(0.75f, 0.25f);

            return GetRandomPositionOnSquare(viewportPositionMin, viewportPositionMax);
        }

        private static Vector2 GetRandomPosition_OnMap(float hiddenZoneWidth, float hiddenZoneHeight)
        {
            Vector2 viewportPositionMin = new Vector2(-hiddenZoneWidth, -hiddenZoneHeight);
            Vector2 viewportPositionMax = Vector2.one - viewportPositionMin;

            return GetRandomPositionOnSquare(viewportPositionMin, viewportPositionMax);
        }

        private static Vector2 GetRandomPositionOnSquare(Vector2 leftBottomPoint, Vector2 rightTopPoint)
        {
            Debug.Assert(leftBottomPoint.x <= rightTopPoint.x && leftBottomPoint.y <= rightTopPoint.y);

            Vector2 randomPoint = rightTopPoint - leftBottomPoint;
            randomPoint.x *= UnityEngine.Random.value;
            randomPoint.y *= UnityEngine.Random.value;

            return leftBottomPoint + randomPoint;
        }
    }
}