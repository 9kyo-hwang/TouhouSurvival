using System;
using System.Collections.Generic;

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

        public static AttributeModifierSet Convert(List<SerializedGameplayAttributeModifier> attributes)
        {
            Dictionary<string, GameplayAttributeOperator> opcodeIndex = new Dictionary<string, GameplayAttributeOperator>(8);
            Type opcodeType = typeof(GameplayAttributeOperator);

            AttributeModifierSet modifierSet = new AttributeModifierSet(16);

            for (int i = 0; i < attributes.Count; ++i)
            {
                GameplayAttributeOperator opcode = (GameplayAttributeOperator)Enum.Parse(opcodeType, attributes[i].operationMode);

                GameplayAttributeModifier modifier = new GameplayAttributeModifier(
                    attributes[i].attributeName,
                    attributes[i].value,
                    opcode,
                    attributes[i].description);

                int level = attributes[i].level;

                modifierSet.TryAdd(level, null);
                modifier.next = modifierSet[level];
                modifierSet[level] = modifier;
            }

            return modifierSet;
        }
    }
}