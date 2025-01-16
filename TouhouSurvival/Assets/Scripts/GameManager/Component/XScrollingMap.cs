using UnityEngine;

namespace Unchord
{
    public class XScrollingMap : Map
    {
        public static XScrollingMap Create(XScrollingMapSO mapSO)
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