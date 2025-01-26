using System;
using UnityEngine;

namespace Unchord
{
    public static class UnchordUtility
    {
        /// <summary>
        /// 2차원 정수 좌표를 정수 인덱스로 변환합니다.
        /// </summary>
        /// <param name="x">2차원 정수 좌표의 x 좌표, -11584~11585 사이의 정수입니다.</param>
        /// <param name="y">2차원 정수 좌표의 y 좌표, -11584~11585 사이의 정수입니다.</param>
        /// <returns>정수 인덱스, 0~536848899 사이의 정수입니다.</returns>
        public static int PointToIndex(int x, int y)
        {
            Debug.Assert(x >= -11584 && x <= 11585);
            Debug.Assert(y >= -11584 && y <= 11585);

            int transposedFlag = 1;

            if (x > y)
            {
                int temp = x;
                x = y;
                y = temp;
                transposedFlag = -1;
            }

            if (x + y > 0)
            {
                int pivot = y + y;
                pivot = pivot * (pivot - 1);
                return pivot + transposedFlag * (y - x);
            }
            else
            {
                int pivot = x + x;
                pivot = pivot * (pivot - 1);
                return pivot + transposedFlag * (x - y);
            }
        }

        /// <summary>
        /// 정수 인덱스를 2차원 정수 좌표로 변환합니다.
        /// </summary>
        /// <param name="index">정수 인덱스, 0~536848899 사이의 정수입니다.</param>
        /// <param name="x">2차원 정수 좌표의 x 좌표, -11584~11585 사이의 정수입니다.</param>
        /// <param name="y">2차원 정수 좌표의 y 좌표, -11584~11585 사이의 정수입니다.</param>
        public static void IndexToPoint(int index, out int x, out int y)
        {
            // NOTE:
            // 23170 == 11585 - (-11584) + 1
            // 536848899 == (23170)^2 - 1
            Debug.Assert(index >= 0 && index <= 536848899);

            // NOTE: The variable 'v' is derived value from 'index'.
            int v = (int)Math.Floor(Math.Sqrt(4 * index + 1));
            int r = v % 4;
            int n = default;
            int pivot = default;

            switch (r)
            {
                case 0:
                    n = v / 4;
                    pivot = PointToIndex(-n, -n);
                    x = -n;
                    y = -n + pivot - index;
                    break;
                case 1:
                    n = (v - 1) / 4;
                    pivot = PointToIndex(-n, -n);
                    x = -n - pivot + index;
                    y = -n;
                    break;
                case 2:
                    n = (v - 2) / 4;
                    pivot = PointToIndex(n + 1, n + 1);
                    x = n + 1;
                    y = n + 1 - pivot + index;
                    break;
                case 3:
                    n = (v + 1) / 4;
                    pivot = PointToIndex(n, n);
                    x = n + pivot - index;
                    y = n;
                    break;
                default:
                    x = default;
                    y = default;
                    Debug.Assert(false, $"Unknown case found. Please debug. (case {r})");
                    break;
            }
        }
    }
}