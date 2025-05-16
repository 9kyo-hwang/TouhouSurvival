using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Unchord
{
    public class SpecialAbilityComponent : MonoBehaviour
    {
        public const int c_TREE_COUNT = 2;

        #region Inspector Properties
        public string attributeModifierPath;
        #endregion

        private SortedList<int, GameplayAttributeModifier>[] _modifiers;
        private int[] _levels;

        private AttributeSet _attrPlayer;
        private AttributeSet _attrWeapon;
        private AttributeSet _attrSpell;

        private void Awake()
        {
            _modifiers = new SortedList<int, GameplayAttributeModifier>[c_TREE_COUNT];
            _levels = new int[c_TREE_COUNT];

            for (int i = 0; i < c_TREE_COUNT; ++i)
            {
                _modifiers[i] = new SortedList<int, GameplayAttributeModifier>(8);
                _levels[i] = 0;
            }

            if (!attributeModifierPath.Equals(string.Empty))
                LoadAttributes(attributeModifierPath);
            else
                UnityEngine.Debug.Assert(false, "attribute modifier path is empty.");
        }

        public void Subscribe(AttributeSet player, AttributeSet mainWeapon, AttributeSet spell)
        {
            _attrPlayer = player;
            _attrWeapon = mainWeapon;
            _attrSpell = spell;
        }

        private void LoadAttributes(string xlsxFilePath)
        {
            string xlsxPath = Application.streamingAssetsPath + xlsxFilePath;
            string xlsxDir = Path.GetDirectoryName(xlsxPath);
            string xlsxName = Path.GetFileNameWithoutExtension(xlsxPath);

            XlsxToCsvConverter.Convert(xlsxDir, xlsxPath, xlsxName);

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

                    switch (tokens[5].ToLower())
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
                    int tree = int.Parse(tokens[1]);
                    _modifiers[tree].TryAdd(level, null);

                    float value = float.Parse(tokens[4]);
                    string desc = tokens[6];
                    string attributeType = tokens[3];
                    GameplayAttributeModifier modifier = new GameplayAttributeModifier(attributeType, value, opcode, desc)
                    {
                        tag = tokens[2].ToLower(),
                        next = _modifiers[tree][level]
                    };

                    _modifiers[tree][level] = modifier;
                }
            }
        }

        public void AddLevel(int treeIndex)
        {
            ++_levels[treeIndex];
            _attrPlayer.ApplyModifiers(_modifiers[treeIndex], _levels[treeIndex], "player");
            _attrWeapon.ApplyModifiers(_modifiers[treeIndex], _levels[treeIndex], "weapon");
            _attrSpell.ApplyModifiers(_modifiers[treeIndex], _levels[treeIndex], "spell");
        }

        public int GetMaxLevel(int treeIndex)
        {
            return _modifiers[treeIndex].Last().Key;
        }

        public int GetLevel(int treeIndex)
        {
            return _levels[treeIndex];
        }

        public string GetDescription(int treeIndex, int level)
        {
            return _modifiers[treeIndex][level].GetDescription();
        }
    }
}