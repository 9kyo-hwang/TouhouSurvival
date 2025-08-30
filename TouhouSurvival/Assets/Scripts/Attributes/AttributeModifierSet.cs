using System;
using System.Collections.Generic;
using System.IO;
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

        // TODO: 기존에는 객체 생성을 위해 아래 함수를 호출했으나 이제 MultiCSVReader 매개변수를 갖는 생성자를 호출하도록 코드 구조를 변경해야 합니다. 이후 이 함수는 삭제합니다.
        public static AttributeModifierSet LoadFromFile(string csvFilePath)
        {
            AttributeModifierSet set = null;
            //AttributeModifierSet set = new AttributeModifierSet();
            
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
                    set.TryAdd(level, null);

                    float value = float.Parse(tokens[2]);
                    string attributeType = tokens[1];
                    string desc = tokens[4];
                    GameplayAttributeModifier modifier = new GameplayAttributeModifier(attributeType, value, opcode, desc)
                    {
                        next = set[level]
                    };

                    set[level] = modifier;
                }
            }

            return set;
        }

        private AttributeModifierSet()
        : base(capacity: 16)
        {
            
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