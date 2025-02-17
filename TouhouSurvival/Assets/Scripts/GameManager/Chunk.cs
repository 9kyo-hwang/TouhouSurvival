using System.Collections.Generic;
using UnityEngine;

namespace Unchord
{
    public class Chunk
    {
        private static SortedDictionary<int, Chunk> s_chunkDictionary;

        static Chunk()
        {
            s_chunkDictionary = new SortedDictionary<int, Chunk>();
        }

        public static Chunk GetChunkByIndex(int index)
        {
            UnityEngine.Debug.Assert(index >= 0);

            if (!s_chunkDictionary.ContainsKey(index))
                s_chunkDictionary[index] = new Chunk();

            return s_chunkDictionary[index];
        }

        public static Chunk GetChunkByPosition(int chunkPositionX, int chunkPositionY)
        {
            int index = UnchordUtility.PointToIndex(chunkPositionX, chunkPositionY);

            return Chunk.GetChunkByIndex(index);
        }

        public static void Clear(int index)
        {
            UnityEngine.Debug.Assert(s_chunkDictionary.ContainsKey(index));

            s_chunkDictionary[index].Clear();
            s_chunkDictionary.Remove(index);
        }

        public static void ClearAll()
        {
            foreach (int index in s_chunkDictionary.Keys)
            {
                Chunk.Clear(index);
            }
        }

        public GameObject chunkObject { get; private set; }
        public List<GameObject> expOrbs { get; private set; }
        public List<GameObject> items { get; private set; }

        public Chunk()
        {
            chunkObject = new GameObject();
            expOrbs = new List<GameObject>(32);
            items = new List<GameObject>(8);
        }

        public void Enable()
        {

        }

        public void Disable()
        {

        }

        public void Clear()
        {

        }
    }
}