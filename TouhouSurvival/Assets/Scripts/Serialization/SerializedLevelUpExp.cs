using System;
using System.Collections.Generic;

namespace Unchord
{
    [Serializable]
    public class SerializedLevelUpExp
    {
        public int nextLevel;
        public float expRequirement;

        public static Dictionary<int, float> Convert(List<SerializedLevelUpExp> list)
        {
            Dictionary<int, float> expTable = new Dictionary<int, float>(32);

            for (int i = 0; i < list.Count; ++i)
            {
                UnityEngine.Debug.Assert(!expTable.ContainsKey(list[i].nextLevel));
                UnityEngine.Debug.Assert(list[i].expRequirement >= 0);

                expTable.Add(list[i].nextLevel, list[i].expRequirement);
            }

            return expTable;
        }
    }
}