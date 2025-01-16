using UnityEngine;

namespace Unchord
{
    public static class ScreenBounds
    {
        private static GameManager.Properties _gm;

        static ScreenBounds()
        {
            _gm = GameManager.Instance.properties;
        }

        public static ScreenZone EvalScreenZone(Vector3 worldPosition, float hiddenZoneWidth = 0.2f, float hiddenZoneHeight = 0.2f)
        {
            Debug.Assert(hiddenZoneWidth >= 0.0f);
            Debug.Assert(hiddenZoneHeight >= 0.0f);

            Vector3 viewportPosition = _gm.MainCamera.WorldToViewportPoint(worldPosition);

            if (viewportPosition.x < -hiddenZoneWidth ||
                viewportPosition.x >= 1.0f + hiddenZoneWidth ||
                viewportPosition.y < -hiddenZoneHeight ||
                viewportPosition.y >= 1.0f + hiddenZoneHeight)
            {
                return ScreenZone.Dead;
            }
            else if (viewportPosition.x < 0.0f ||
                viewportPosition.x > 1.0f ||
                viewportPosition.y < 0.0f ||
                viewportPosition.y > 1.0f)
            {
                return ScreenZone.Hidden;
            }
            else
            {
                return ScreenZone.Live;
            }
        }
    }
}