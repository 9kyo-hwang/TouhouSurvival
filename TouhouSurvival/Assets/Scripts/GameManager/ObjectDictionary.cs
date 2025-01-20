using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unchord
{
    public class ObjectDictionary
    {
        private const int INITIAL_CHUNK_CAPACITY = 128;

        private SortedDictionary<int, List<GameObject>> _dictionary;
        private SortedList<int, int> _enableChunkList;

        public ObjectDictionary()
        {
            _dictionary = new SortedDictionary<int, List<GameObject>>();
            _enableChunkList = new SortedList<int, int>(9);
        }

        public void Add(GameObject gameObject, int chunkPositionX, int chunkPositionY)
        {
            int key = PointToIndex(chunkPositionX, chunkPositionY);

            if (!_dictionary.ContainsKey(key))
                _dictionary.Add(key, new List<GameObject>(INITIAL_CHUNK_CAPACITY));

            if (!_dictionary[key].Contains(gameObject))
            {
                _dictionary[key].Add(gameObject);
                gameObject.SetActive(_enableChunkList.ContainsKey(key));
            }
        }

        public void Remove(GameObject gameObject, int chunkPositionX, int chunkPositionY)
        {
            int key = PointToIndex(chunkPositionX, chunkPositionY);

            if (_dictionary.ContainsKey(key) && _dictionary[key].Contains(gameObject))
            {
                _dictionary[key].Remove(gameObject);
            }
        }

        public void Enable(int chunkPositionX, int chunkPositionY)
        {
            int key = PointToIndex(chunkPositionX, chunkPositionY);

            if (_enableChunkList.ContainsKey(key) || !_dictionary.ContainsKey(key))
                return;

            _enableChunkList.Add(key, key);

            List<GameObject> list = _dictionary[key];

            for (int i = list.Count - 1; i >= 0; --i)
            {
                list[i].SetActive(true);
            }
        }

        public void Disable(int chunkPositionX, int chunkPositionY)
        {
            int index = PointToIndex(chunkPositionX, chunkPositionY);

            if (!_enableChunkList.ContainsKey(index) || !_dictionary.ContainsKey(index))
                return;

            _enableChunkList.Remove(index);

            List<GameObject> list = _dictionary[index];

            for (int i = list.Count - 1; i >= 0; --i)
            {
                list[i].SetActive(false);
            }
        }

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

            switch(r)
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