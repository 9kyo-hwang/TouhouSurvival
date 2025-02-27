using System;
using UnityEngine;

namespace Unchord
{
    [Serializable]
    public struct LevelUpData<T_Enum>
    where T_Enum : System.Enum
    {
        public float expRequirement;
        public T_Enum attributeType;
        public AttributeOperation attributeOperation;
        public float deltaValue;

        [Header("Displays on GUI")]
        public string displayDescription;
    }
}