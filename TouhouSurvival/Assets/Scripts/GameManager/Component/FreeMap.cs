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

            for (int i = 0; i < 9; ++i)
            {
                int dx, dy;
                UnchordUtility.IndexToPoint(i, out dx, out dy);
                FreeMap.InstantiateTessellation(mapSO, parent, new Vector2(dx, dy), Chunk.directionStrings[i]);
                Chunk.GetChunkByIndex(i).Enable();
            }

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

            int newChunkPositionX;
            int newChunkPositionY;
            Map.GetChunkPosition(cameraPosition, _size, out newChunkPositionX, out newChunkPositionY);

            if (_chunkPositionX == newChunkPositionX && _chunkPositionY == newChunkPositionY)
                return;

            Chunk.MoveChunkPosition(_chunkPositionX, _chunkPositionY, newChunkPositionX, newChunkPositionY);
            _chunkPositionX = newChunkPositionX;
            _chunkPositionY = newChunkPositionY;

            float worldPositionX = (float)_chunkPositionX * _size.x;
            float worldPositionY = (float)_chunkPositionY * _size.y;
            float worldPositionZ = transform.position.z;

            transform.position = new Vector3(worldPositionX, worldPositionY, worldPositionZ);
        }
    }
}