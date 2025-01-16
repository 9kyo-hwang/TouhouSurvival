using UnityEngine;

namespace Unchord
{
    public class FreeMap : Map
    {
        private Vector2 _size;
        private int _chunkPositionX;
        private int _chunkPositionY;

        public static FreeMap Create(FreeMapSO mapSO)
        {
            GameObject parent = new GameObject("FreeMap");
            FreeMap map = parent.AddComponent<FreeMap>();

            Debug.Assert(mapSO.size.x > 0.0f && mapSO.size.y > 0.0f, "All size of each axis should be greater than 0.");

            map._size = mapSO.size;

            FreeMap.InstantiateTessellation(mapSO, parent, new Vector2(0.0f, 0.0f), "C");
            FreeMap.InstantiateTessellation(mapSO, parent, new Vector2(1.0f, 0.0f), "R");
            FreeMap.InstantiateTessellation(mapSO, parent, new Vector2(1.0f, 1.0f), "RT");
            FreeMap.InstantiateTessellation(mapSO, parent, new Vector2(0.0f, 1.0f), "T");
            FreeMap.InstantiateTessellation(mapSO, parent, new Vector2(-1.0f, 1.0f), "LT");
            FreeMap.InstantiateTessellation(mapSO, parent, new Vector2(-1.0f, 0.0f), "L");
            FreeMap.InstantiateTessellation(mapSO, parent, new Vector2(-1.0f, -1.0f), "LB");
            FreeMap.InstantiateTessellation(mapSO, parent, new Vector2(0.0f, -1.0f), "B");
            FreeMap.InstantiateTessellation(mapSO, parent, new Vector2(1.0f, -1.0f), "RB");

            return map;
        }

        private static GameObject InstantiateTessellation(FreeMapSO mapSO, GameObject parent, Vector2 offset, string directionString)
        {
            GameObject instance = GameObject.Instantiate(mapSO.mapTessellationPrefab);
            instance.transform.position = mapSO.size * offset;
            instance.transform.parent = parent.transform;
            instance.name = string.Format("Map Tessellation {0}", directionString);
            return instance;
        }

        public override bool IsCameraOutOfMap(Camera camera)
        {
            Vector3 cameraPosition = camera.ViewportToWorldPoint(0.5f * Vector2.one);
            Vector2 delta = cameraPosition - transform.position;

            return delta.x < -_size.x || delta.x > _size.x || delta.y < -_size.y || delta.y > _size.y;
        }

        public override void ScrollMap(Camera camera)
        {
            Vector3 cameraPosition = camera.ViewportToWorldPoint(0.5f * Vector2.one);

            int newChunkPositionX = Mathf.FloorToInt(0.5f + cameraPosition.x / _size.x);
            int newChunkPositionY = Mathf.FloorToInt(0.5f + cameraPosition.y / _size.y);

            if (_chunkPositionX == newChunkPositionX && _chunkPositionY == newChunkPositionY)
                return;

            _chunkPositionX = newChunkPositionX;
            _chunkPositionY = newChunkPositionY;

            float worldPositionX = (float)_chunkPositionX * _size.x;
            float worldPositionY = (float)_chunkPositionY * _size.y;
            float worldPositionZ = transform.position.z;

            transform.position = new Vector3(worldPositionX, worldPositionY, worldPositionZ);
        }
    }
}