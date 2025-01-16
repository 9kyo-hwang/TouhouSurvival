using UnityEngine;

namespace Unchord
{
    public class FixedMap : Map
    {
        public static FixedMap Create(FixedMapSO mapSO)
        {
            return null;
        }

        public override bool IsCameraOutOfMap(Camera camera)
        {
            throw new System.NotImplementedException();
        }

        public override void ScrollMap(Camera camera)
        {
            throw new System.NotImplementedException();
        }
    }
}