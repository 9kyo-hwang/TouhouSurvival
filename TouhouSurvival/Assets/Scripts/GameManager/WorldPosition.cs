using UnityEngine;

namespace Unchord
{
    public static class WorldPosition
    {
        public static Vector2 GetRandomPosition(PositionFlag positionFlags, Camera camera, float hiddenZoneWidth = 0.2f, float hiddenZoneHeight = 0.2f)
        {
            UnityEngine.Debug.Assert(camera != null);
            UnityEngine.Debug.Assert(hiddenZoneWidth >= 0.0f);
            UnityEngine.Debug.Assert(hiddenZoneHeight >= 0.0f);

            PositionFlag flag = WorldPosition.GetRandomSingleFlag(positionFlags);

            switch (flag)
            {
                case PositionFlag.None:
                    return RandomOnRectangle(camera, 0.5f, 0.5f, 0.5f, 0.5f);
                case PositionFlag.OutOfL:
                    return RandomOutOfL(camera, hiddenZoneWidth, hiddenZoneHeight);
                case PositionFlag.OutOfT:
                    return RandomOutOfT(camera, hiddenZoneWidth, hiddenZoneHeight);
                case PositionFlag.OutOfR:
                    return RandomOutOfR(camera, hiddenZoneWidth, hiddenZoneHeight);
                case PositionFlag.OutOfB:
                    return RandomOutOfB(camera, hiddenZoneWidth, hiddenZoneHeight);
                case PositionFlag.QuarterOfL:
                    return RandomQuarterOfL(camera);
                case PositionFlag.QuarterOfT:
                    return RandomQuarterOfT(camera);
                case PositionFlag.QuarterOfR:
                    return RandomQuarterOfR(camera);
                case PositionFlag.QuarterOfB:
                    return RandomQuarterOfB(camera);
                case PositionFlag.HalfOfL:
                    return RandomHalfOfL(camera);
                case PositionFlag.HalfOfT:
                    return RandomHalfOfT(camera);
                case PositionFlag.HalfOfR:
                    return RandomHalfOfR(camera);
                case PositionFlag.HalfOfB:
                    return RandomHalfOfB(camera);
                case PositionFlag.LiveZone:
                    return RandomOnLiveZone(camera);
                case PositionFlag.HiddenZone:
                    return RandomOnHiddenZone(camera, hiddenZoneWidth, hiddenZoneHeight);
                default:
                    Debug.Assert(false, "Invalid case occurred. Please debug.");
                    return RandomOnRectangle(camera, 0.5f, 0.5f, 0.5f, 0.5f);
            }
        }

        public static PositionFlag GetRandomSingleFlag(PositionFlag mixedFlag)
        {
            uint flag = (uint)mixedFlag;
            uint value = 0;
            uint mask = 1;
            int count = 0;

            while (mask != 0)
            {
                if ((flag & mask) != 0 && UnityEngine.Random.Range(0, ++count) == 0)
                    value = mask;

                mask <<= 1;
            }

            return (PositionFlag)value;
        }

        public static Vector2 RandomOutOfL(Camera camera, float hiddenZoneWidth, float hiddenZoneHeight)
        {
            UnityEngine.Debug.Assert(camera != null);
            UnityEngine.Debug.Assert(hiddenZoneWidth >= 0.0f);
            UnityEngine.Debug.Assert(hiddenZoneHeight >= 0.0f);

            return RandomOnRectangle(camera, -hiddenZoneWidth, 0.0f, -hiddenZoneHeight, 1.0f + hiddenZoneHeight);
        }

        public static Vector2 RandomOutOfT(Camera camera, float hiddenZoneWidth, float hiddenZoneHeight)
        {
            UnityEngine.Debug.Assert(camera != null);
            UnityEngine.Debug.Assert(hiddenZoneWidth >= 0.0f);
            UnityEngine.Debug.Assert(hiddenZoneHeight >= 0.0f);

            return RandomOnRectangle(camera, -hiddenZoneWidth, 1.0f + hiddenZoneWidth, 1.0f, 1.0f + hiddenZoneHeight);
        }

        public static Vector2 RandomOutOfR(Camera camera, float hiddenZoneWidth, float hiddenZoneHeight)
        {
            UnityEngine.Debug.Assert(camera != null);
            UnityEngine.Debug.Assert(hiddenZoneWidth >= 0.0f);
            UnityEngine.Debug.Assert(hiddenZoneHeight >= 0.0f);

            return RandomOnRectangle(camera, 1.0f, 1.0f + hiddenZoneWidth, -hiddenZoneHeight, 1.0f + hiddenZoneHeight);
        }

        public static Vector2 RandomOutOfB(Camera camera, float hiddenZoneWidth, float hiddenZoneHeight)
        {
            UnityEngine.Debug.Assert(camera != null);
            UnityEngine.Debug.Assert(hiddenZoneWidth >= 0.0f);
            UnityEngine.Debug.Assert(hiddenZoneHeight >= 0.0f);

            return RandomOnRectangle(camera, -hiddenZoneWidth, 1.0f + hiddenZoneWidth, -hiddenZoneHeight, 0.0f);
        }

        public static Vector2 RandomQuarterOfL(Camera camera)
        {
            UnityEngine.Debug.Assert(camera != null);

            return RandomOnRectangle(camera, 0.125f, 0.125f, 0.125f, 0.875f);
        }

        public static Vector2 RandomQuarterOfT(Camera camera)
        {
            UnityEngine.Debug.Assert(camera != null);

            return RandomOnRectangle(camera, 0.125f, 0.875f, 0.875f, 0.875f);
        }

        public static Vector2 RandomQuarterOfR(Camera camera)
        {
            UnityEngine.Debug.Assert(camera != null);

            return RandomOnRectangle(camera, 0.875f, 0.875f, 0.125f, 0.875f);
        }

        public static Vector2 RandomQuarterOfB(Camera camera)
        {
            UnityEngine.Debug.Assert(camera != null);

            return RandomOnRectangle(camera, 0.125f, 0.875f, 0.125f, 0.125f);
        }

        public static Vector2 RandomHalfOfL(Camera camera)
        {
            UnityEngine.Debug.Assert(camera != null);

            return RandomOnRectangle(camera, 0.25f, 0.25f, 0.25f, 0.75f);
        }

        public static Vector2 RandomHalfOfT(Camera camera)
        {
            UnityEngine.Debug.Assert(camera != null);

            return RandomOnRectangle(camera, 0.25f, 0.75f, 0.75f, 0.75f);
        }

        public static Vector2 RandomHalfOfR(Camera camera)
        {
            UnityEngine.Debug.Assert(camera != null);

            return RandomOnRectangle(camera, 0.75f, 0.75f, 0.25f, 0.75f);
        }

        public static Vector2 RandomHalfOfB(Camera camera)
        {
            UnityEngine.Debug.Assert(camera != null);

            return RandomOnRectangle(camera, 0.25f, 0.75f, 0.25f, 0.25f);
        }

        public static Vector2 RandomOnLiveZone(Camera camera)
        {
            UnityEngine.Debug.Assert(camera != null);

            return RandomOnRectangle(camera, 0.0f, 1.0f, 0.0f, 1.0f);
        }

        public static Vector2 RandomOnHiddenZone(Camera camera, float hiddenZoneWidth, float hiddenZoneHeight)
        {
            UnityEngine.Debug.Assert(camera != null);
            UnityEngine.Debug.Assert(hiddenZoneWidth >= 0.0f);
            UnityEngine.Debug.Assert(hiddenZoneHeight >= 0.0f);

            return RandomOnRectangle(camera, -hiddenZoneWidth, 1.0f + hiddenZoneWidth, -hiddenZoneHeight, 1.0f + hiddenZoneHeight);
        }

        public static Vector2 RandomOnRectangle(Camera camera, float xMin, float xMax, float yMin, float yMax)
        {
            UnityEngine.Debug.Assert(camera != null);
            UnityEngine.Debug.Assert(xMin <= xMax);
            UnityEngine.Debug.Assert(yMin <= yMax);

            float x = UnityEngine.Random.Range(xMin, xMax);
            float y = UnityEngine.Random.Range(yMin, yMax);

            return camera.ViewportToWorldPoint(new Vector3(x, y, 0.0f));
        }
    }
}