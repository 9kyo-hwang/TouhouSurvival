using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Unchord
{
    public class AttributeModifierSet
    {
        public int MaxLevel
        {
            get
            {
                if (_modifiers.Count == 0)
                    return 1;

                return Mathf.Max(1, _modifiers.Last().Key);
            }
        }

        public GameplayAttributeModifier this[int level]
        {
            get
            {
                if (!_modifiers.ContainsKey(level))
                    return null;

                return _modifiers[level];
            }
        }

        private SortedList<int, GameplayAttributeModifier> _modifiers;

        public static AttributeModifierSet LoadFromFile(string csvFilePath)
        {
            AttributeModifierSet set = new AttributeModifierSet();
            
            using (FileStream fs = new FileStream(csvFilePath, FileMode.Open, FileAccess.Read))
            using (StreamReader rd = new StreamReader(fs))
            {
                rd.ReadLine(); // Ignore header line.

                while (!rd.EndOfStream)
                {
                    string[] tokens = rd.ReadLine().Split(",");

                    if (tokens[0].Equals(string.Empty))
                        continue;

                    GameplayAttributeOperator opcode = GameplayAttributeOperator.Flat;

                    switch (tokens[3].ToLower())
                    {
                        case "flat":
                            opcode = GameplayAttributeOperator.Flat;
                            break;
                        case "percentadd":
                            opcode = GameplayAttributeOperator.PercentAdd;
                            break;
                        case "percentmul":
                            opcode = GameplayAttributeOperator.PercentMul;
                            break;
                        default:
                            UnityEngine.Debug.Assert(false);
                            break;
                    }

                    int level = int.Parse(tokens[0]);
                    set._modifiers.TryAdd(level, null);

                    float value = float.Parse(tokens[2]);
                    string attributeType = tokens[1];
                    string desc = tokens[4];
                    GameplayAttributeModifier modifier = new GameplayAttributeModifier(attributeType, value, opcode, desc)
                    {
                        next = set._modifiers[level]
                    };

                    set._modifiers[level] = modifier;
                }
            }

            return set;
        }

        // only can instantiate class-scope.
        private AttributeModifierSet()
        {
            _modifiers = new SortedList<int, GameplayAttributeModifier>(8);
        }

        public string GetDescription(int level)
        {
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