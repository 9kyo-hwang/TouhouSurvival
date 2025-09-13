using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unchord
{
    public class AttributeModifierSet : SortedList<int, GameplayAttributeModifier>
    {
        public int MaxLevel
        {
            get
            {
                if (this.Count == 0)
                    return 1;

                return Mathf.Max(1, this.Last().Key);
            }
        }

        private AttributeModifierSet()
        : base(capacity: 16)
        {
            
        }

        public AttributeModifierSet(List<SerializedGameplayAttributeModifier> modifiers) : this()
        {
            for (int i = 0; i < modifiers.Count; ++i)
            {
                GameplayAttributeOperator opcode;

                if (!Enum.TryParse<GameplayAttributeOperator>(modifiers[i].operationMode, out opcode))
                {
                    UnityEngine.Debug.Assert(false, "Invalid enum value of type GameplayAttributeOperator.");
                    break;
                }

                GameplayAttributeModifier modifier = new GameplayAttributeModifier(
                    modifiers[i].attributeName,
                    modifiers[i].value,
                    opcode,
                    modifiers[i].description);

                int level = modifiers[i].level;

                // linked list data structure.
                this.TryAdd(level, null);
                modifier.next = this[level];
                this[level] = modifier;
            }
        }

        public AttributeModifierSet(MultiCSVReader reader, string aliasOrNull = null)
        : this()
        {
            List<SerializedGameplayAttributeModifier> attrMods;

            if (!reader.TryParseTable<SerializedGameplayAttributeModifier>(out attrMods, aliasOrNull))
            {
                UnityEngine.Debug.Assert(false, "Parsing SerializedGameplayAttributeModifier type failed.");
                return;
            }

            for (int i = 0; i < attrMods.Count; ++i)
            {
                GameplayAttributeOperator opcode;

                if (!Enum.TryParse<GameplayAttributeOperator>(attrMods[i].operationMode, out opcode))
                {
                    UnityEngine.Debug.Assert(false, "Invalid enum value of type GameplayAttributeOperator.");
                    break;
                }

                GameplayAttributeModifier modifier = new GameplayAttributeModifier(
                    attrMods[i].attributeName,
                    attrMods[i].value,
                    opcode,
                    attrMods[i].description);

                int level = attrMods[i].level;

                // linked list data structure.
                this.TryAdd(level, null);
                modifier.next = this[level];
                this[level] = modifier;
            }
        }

        public string GetDescription(int level)
        {
            if (!this.ContainsKey(level))
                return string.Empty;

            GameplayAttributeModifier mod = this[level];

            string message = string.Empty;

            while (mod != null && mod.description.Equals(string.Empty))
                mod.next = mod;

            while (mod != null)
            {
                message += mod.description;

                if (mod.next != null && !mod.next.description.Equals(string.Empty))
                    message += "\n";

                mod = mod.next;
            }

            return message;
        }
    }
}