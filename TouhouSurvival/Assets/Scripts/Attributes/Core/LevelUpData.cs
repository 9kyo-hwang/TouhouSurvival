using System;
using UnityEngine;

namespace Unchord
{
    [Serializable]
    public class LevelUpData
    {
        public string attributeType;
        public AttributeOperation attributeOperation;
        public float deltaValue;

        [Header("Displays on GUI")]
        public string displayDescription;

        [HideInInspector]
        public LevelUpData next;
    }
}