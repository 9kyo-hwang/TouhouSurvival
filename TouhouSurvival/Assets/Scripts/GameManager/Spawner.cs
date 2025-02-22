using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Unchord
{
    public static class Spawner
    {
        private const int INITIAL_INSTANTIATED_GAMEOBJECT_CAPACITY = 32;
        private const int INITIAL_SPAWNED_OBJECT_POOL_CAPACITY = 256;

        private static System.Random s_prng;
        private static List<GameObject> s_instances;

        public static List<GameObject> SpawnedObjects { get; private set; }

        // TODO:
        // Player의 구현이 일정 수준 이상 되었을 때 Player 클래스로 이 함수를 옮겨 작성하도록 합니다. 자세한 지침은 아래를 따르세요.
        //      1. 함수에 static 제거해 멤버 함수로 작성해야 합니다.
        //      2. originPosition 매개변수를 제거하세요.
        //      3. 함수 내부에 주석 처리한 originPosition 선언문을 활성화합니다.
        //      4. diffTarget, diffSelected 계산식의 우변에 존재하는 Vector2로의 형변환 연산자를 제거하세요.
        public static GameObject GetNearestEnemyOrNull(Vector2 originPosition)
        {
            // Vector3 originPosition = transform.position;
            GameObject selected = null;

            for (int i = Spawner.SpawnedObjects.Count - 1; i >= 0; --i)
            {
                if (Spawner.SpawnedObjects[i] == null)
                {
                    Spawner.SpawnedObjects.RemoveAt(i);
                    continue;
                }
                else if (selected == null)
                {
                    selected = Spawner.SpawnedObjects[i];
                    continue;
                }

                Vector2 diffTarget = (Vector2)Spawner.SpawnedObjects[i].transform.position - originPosition;
                Vector2 diffSelected = (Vector2)selected.transform.position - originPosition;

                if (diffTarget.sqrMagnitude < diffSelected.sqrMagnitude)
                {
                    selected = Spawner.SpawnedObjects[i];
                    continue;
                }
            }

            return selected;
        }

        static Spawner()
        {
            Spawner.s_prng = new System.Random();
            Spawner.s_instances = new List<GameObject>(INITIAL_INSTANTIATED_GAMEOBJECT_CAPACITY);
            Spawner.SpawnedObjects = new List<GameObject>(INITIAL_SPAWNED_OBJECT_POOL_CAPACITY);
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

                GameObject instance = Object.Instantiate(spawnerSO.spawnDataList[prefabIndex].entityPrefab, GameManager.Instance.RuntimeContainer, true);
                Vector2 randomPosition = GetRandomPosition(spawnerSO.spawnPositionFlag);
                instance.transform.position = randomPosition;
                s_instances.Add(instance);
                SpawnedObjects.Add(instance);
            }

            return new ReadOnlyCollection<GameObject>(s_instances);
        }

        private static ReadOnlyCollection<GameObject> CreateInstances_Group(SpawnerSO spawnerSO)
        {
            SpawnPositionFlag spawnPositionFlag = spawnerSO.spawnPositionFlag;

            // Debug.LogFormat("Spawn with group mode, entity count == {0}", _instantiatedGameObjects.Count);
            // SetPositionRandom();
            throw new NotImplementedException();
        }

        private static ReadOnlyCollection<GameObject> CreateInstances_Circular(SpawnerSO spawnerSO)
        {
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