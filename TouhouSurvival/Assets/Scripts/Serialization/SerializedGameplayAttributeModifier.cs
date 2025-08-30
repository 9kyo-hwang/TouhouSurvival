using System;

namespace Unchord
{
    [Serializable]
    public class SerializedGameplayAttributeModifier
    {
        public int level;
        public string attributeName;
        public float value;
        public string operationMode;
        public string description;
    }
}