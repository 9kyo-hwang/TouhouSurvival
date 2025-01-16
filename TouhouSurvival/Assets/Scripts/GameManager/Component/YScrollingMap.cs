using UnityEngine;

namespace Unchord
{
    public class YScrollingMap : Map
    {
        public static YScrollingMap Create(YScrollingMapSO mapSO)
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