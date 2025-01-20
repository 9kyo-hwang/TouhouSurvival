using UnityEngine;

namespace Unchord
{
    public abstract class Map : MonoBehaviour
    {
        public static Map Create(MapSO mapSO)
        {
            if (mapSO is FreeMapSO)
                return FreeMap.Create(mapSO as FreeMapSO);
            else if (mapSO is XScrollingMapSO)
                return XScrollingMap.Create(mapSO as XScrollingMapSO);
            else if (mapSO is YScrollingMapSO)
                return YScrollingMap.Create(mapSO as YScrollingMapSO);
            else if (mapSO is FixedMapSO)
                return FixedMap.Create(mapSO as FixedMapSO);
            else
                return null;
        }

        public static void GetChunkPosition(Vector2 worldPosition, Vector2 chunkSize, out int chunkPositionX, out int chunkPositionY)
        {
            chunkPositionX = Mathf.FloorToInt(0.5f + worldPosition.x / chunkSize.x);
            chunkPositionY = Mathf.FloorToInt(0.5f + worldPosition.y / chunkSize.y);
        }

        public abstract bool IsCameraOutOfMap(Camera camera);

        public abstract void ScrollMap(Camera camera);
    }
}