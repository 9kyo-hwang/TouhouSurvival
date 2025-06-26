using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Unchord
{
    public class GameplayAttributeModifier : IComparable<GameplayAttributeModifier>
    {
        public string key;
        public float value;
        public GameplayAttributeOperator opcode;
        public string description;

        public GameplayAttributeModifier next;

        public static SortedList<int, GameplayAttributeModifier> LoadAttributeModifiers(string xlsxFilePath)
        {
            string xlsxPath = Application.streamingAssetsPath + xlsxFilePath;
            string xlsxDir = Path.GetDirectoryName(xlsxPath);
            string xlsxName = Path.GetFileNameWithoutExtension(xlsxPath);

            XlsxToCsvConverter.Convert(xlsxDir, xlsxPath, xlsxName);

            SortedList<int, GameplayAttributeModifier> modifiers = new SortedList<int, GameplayAttributeModifier>(16);

            using FileStream fs = new FileStream(xlsxDir + $"\\{xlsxName}+modifiers.csv", FileMode.Open, FileAccess.Read);
            using (StreamReader rd = new StreamReader(fs))
            {
                rd.ReadLine(); // NOTE: Ignore header line.

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
                    modifiers.TryAdd(level, null);

                    float value = float.Parse(tokens[2]);
                    string attributeType = tokens[1];
                    string desc = tokens[4];
                    GameplayAttributeModifier modifier = new GameplayAttributeModifier(attributeType, value, opcode, desc)
                    {
                        next = modifiers[level]
                    };

                    modifiers[level] = modifier;
                }
            }

            return modifiers;
        }

        public GameplayAttributeModifier(string key, float value, GameplayAttributeOperator opcode, string description = "")
        {
            this.key = key;
            this.value = value;
            this.opcode = opcode;
            this.description = description;
        }

        public int CompareTo(GameplayAttributeModifier other)
        {
            if (this.opcode < other.opcode)
                return -1;
            else if (this.opcode > other.opcode)
                return 1;
            else
                return 0;
        }

        public string GetDescription()
        {
            StringBuilder descBuilder = new StringBuilder();
            GameplayAttributeModifier modifier = this;

            while (modifier != null)
            {
                GameplayAttributeModifier mod = modifier;
                modifier = modifier.next;

                if (mod.description == null)
                    continue;
                else if (descBuilder.Length > 0)
                    descBuilder.Append("\n");

                descBuilder.Append(mod.description);
            }

            return descBuilder.ToString();
        }
    }
}