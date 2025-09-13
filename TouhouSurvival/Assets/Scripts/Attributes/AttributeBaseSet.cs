using System.Collections.Generic;

namespace Unchord
{
    public class AttributeBaseSet : Dictionary<string, GameplayAttribute>
    {
        private AttributeBaseSet()
        : base(capacity: 32)
        {

        }

        public AttributeBaseSet(MultiCSVReader reader, string aliasOrNull = null)
        : this()
        {
            List<SerializedGameplayAttributeBase> attrBase;

            if (!reader.TryParseTable<SerializedGameplayAttributeBase>(out attrBase, aliasOrNull))
            {
                UnityEngine.Debug.Assert(false, "Parsing SerializedGameplayAttributeBase type failed.");
                return;
            }

            for (int i = 0; i < attrBase.Count; ++i)
            {
                base.Add(attrBase[i].attributeName, new GameplayAttribute(attrBase[i].baseValue));
            }
        }

        public void ApplyModifiers(GameplayAttributeModifier modifier)
        {
            while (modifier != null)
            {
                this[modifier.key].AddModifier(modifier);
                modifier = modifier.next;
            }
        }
    }
}