using System;
using UnityEngine;

namespace Unchord
{
    [Serializable]
    public struct LevelUpData
    {
        public float expRequirement;
        public string attributeType;
        public AttributeOperation attributeOperation;
        public float deltaValue;

        [Header("Displays on GUI")]
        public string displayDescription;
    }
}