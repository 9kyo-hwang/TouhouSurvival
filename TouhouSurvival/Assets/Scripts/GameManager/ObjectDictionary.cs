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
            int key = UnchordUtility.PointToIndex(chunkPositionX, chunkPositionY);

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
            int key = UnchordUtility.PointToIndex(chunkPositionX, chunkPositionY);

            if (_dictionary.ContainsKey(key) && _dictionary[key].Contains(gameObject))
            {
                _dictionary[key].Remove(gameObject);
            }
        }

        public void Enable(int chunkPositionX, int chunkPositionY)
        {
            int key = UnchordUtility.PointToIndex(chunkPositionX, chunkPositionY);

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
            int index = UnchordUtility.PointToIndex(chunkPositionX, chunkPositionY);

            if (!_enableChunkList.ContainsKey(index) || !_dictionary.ContainsKey(index))
                return;

            _enableChunkList.Remove(index);

            List<GameObject> list = _dictionary[index];

            for (int i = list.Count - 1; i >= 0; --i)
            {
                list[i].SetActive(false);
            }
        }
    }
}