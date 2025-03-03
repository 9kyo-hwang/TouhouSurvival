using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Unchord
{
    public class Chunk
    {
        public static ReadOnlyCollection<string> directionStrings => s_directionStrings;

        private static ReadOnlyCollection<string> s_directionStrings;
        private static SortedDictionary<int, Chunk> s_chunkDictionary;
        private static Transform s_chunkParent;

        static Chunk()
        {
            s_directionStrings = new ReadOnlyCollection<string>(new string[] { "C", "R", "RT", "T", "LT", "L", "LB", "B", "RB" });
            s_chunkDictionary = new SortedDictionary<int, Chunk>();
            s_chunkParent = new GameObject("@Chunks").transform;

            UnityEngine.Object.DontDestroyOnLoad(s_chunkParent);
        }

        public static void MoveChunkPosition(int prevChunkPositionX, int prevChunkPositionY, int nextChunkPositionX, int nextChunkPositionY)
        {
            Dictionary<int, int> dict = new Dictionary<int, int>(18);

            void MarkFlag(int x, int y, int flag)
            {
                int i = UnchordUtility.PointToIndex(x, y);

                if (!dict.ContainsKey(i))
                    dict.Add(i, 0);

                dict[i] += flag;
            }

            for (int i = 0; i < 9; ++i)
            {
                int dx, dy;
                UnchordUtility.IndexToPoint(i, out dx, out dy);
                MarkFlag(prevChunkPositionX + dx, prevChunkPositionY + dy, -1);
                MarkFlag(nextChunkPositionX + dx, nextChunkPositionY + dy, 1);
            }

            foreach (int key in dict.Keys)
            {
                if (dict[key] > 0)
                    GetChunkByIndex(key).Enable();
                else if (dict[key] < 0)
                    GetChunkByIndex(key).Disable();
                else
                    continue;
            }
        }

        public static Chunk GetChunkByIndex(int index)
        {
            UnityEngine.Debug.Assert(index >= 0);

            if (!s_chunkDictionary.ContainsKey(index))
                s_chunkDictionary[index] = new Chunk(index);

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

        public int chunkIndex { get; private set; }
        public GameObject chunkObject { get; private set; }
        public List<GameObject> expOrbs { get; private set; }
        public List<GameObject> items { get; private set; }

        private Chunk(int index)
        {
            chunkIndex = index;
            chunkObject = new GameObject();
            expOrbs = new List<GameObject>(32);
            items = new List<GameObject>(8);

            int cx, cy;
            UnchordUtility.IndexToPoint(index, out cx, out cy);

            chunkObject.name = $"Chunk ({cx}, {cy})";
            chunkObject.transform.SetParent(s_chunkParent);
        }

        public void Enable()
        {
            //UnityEngine.Debug.Log($"Chunk #{chunkIndex} Enabled.");
        }

        public void Disable()
        {
            //UnityEngine.Debug.Log($"Chunk #{chunkIndex} Disabled.");
        }

        public void Clear()
        {

        }
    }
}