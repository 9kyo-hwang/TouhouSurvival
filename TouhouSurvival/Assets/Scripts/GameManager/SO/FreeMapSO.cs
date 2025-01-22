using UnityEngine;

namespace Unchord
{
    [CreateAssetMenu(fileName = "New Free Map", menuName = "Scriptable Objects/Game Management/Free Map", order = (int)GameManagerAssetMenuOrder.FreeMapSO)]
    public class FreeMapSO : MapSO
    {
        public GameObject mapTessellationPrefab;
        public Vector2 size;

    }
}