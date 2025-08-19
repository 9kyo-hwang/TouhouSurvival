using System;
using System.Collections.Generic;

namespace Unchord
{
    [Serializable]
    public class SerializedGameplayAttributeBase
    {
        public string attributeName;
        public float baseValue;

        public static AttributeBaseSet Convert(List<SerializedGameplayAttributeBase> attributes)
        {
            AttributeBaseSet dict = new AttributeBaseSet(attributes.Count);

            for (int i = 0; i < attributes.Count; ++i)
            {
                dict.Add(attributes[i].attributeName, new GameplayAttribute(attributes[i].baseValue));
            }

            return dict;
        }
    }
}