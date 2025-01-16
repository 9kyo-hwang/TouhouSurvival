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

        public abstract bool IsCameraOutOfMap(Camera camera);

        public abstract void ScrollMap(Camera camera);
    }
}