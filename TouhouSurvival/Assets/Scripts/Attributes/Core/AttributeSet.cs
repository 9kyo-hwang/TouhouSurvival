using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Unchord
{
    public abstract class AttributeSet : MonoBehaviour
    {
        public int Level
        {
            get => _level;
            set
            {
                if (value < 0)
                    value = 0;

                if (value == _level)
                    return;

                int prevLevel = _level;
                int nextLevel = value;

                _level = value;

                for (int i = Math.Max(prevLevel + 1, 1); i <= Math.Min(nextLevel, this.MaxLevel - 1); ++i) // 레벨 상승 시 적용
                {
                    LevelUpData data = null;

                    if (this.LevelUpData.ContainsKey(i))
                        data = this.LevelUpData[i];

                    while (data != null)
                    {
                        AttachLevelUpData(data);
                        data = data.next;
                    }
                }

                for (int i = Math.Min(prevLevel - 1, this.MaxLevel - 1); i >= Math.Max(nextLevel, 1); --i) // 레벨 감소 시 적용
                {
                    LevelUpData data = null;

                    if (this.LevelUpData.ContainsKey(i))
                        data = this.LevelUpData[i];

                    while (data != null)
                    {
                        DetachLevelUpData(data);
                        data = data.next;
                    }
                }
            }
        }

        public int MaxLevel => LevelUpData.Count + 1;

        public bool IsReachedMaxLevel => (int)Level > LevelUpData.Count;

        public string attributeAssetPath;

        public Dictionary<string, GameplayAttribute> Attributes { get; private set; }
        public SortedList<int, LevelUpData> LevelUpData { get; private set; }

        private int _level;

        public GameplayAttribute this[string attributeType]
        {
            get => Attributes[attributeType];
            set => Attributes[attributeType] = value;
        }

        protected virtual void Awake()
        {
            Attributes = new Dictionary<string, GameplayAttribute>();
            LevelUpData = new SortedList<int, LevelUpData>();

            //AttributeSetBuilder.LoadAttributes(this);
            LoadAttributes();
        }

        protected virtual void Start()
        {

        }

        private void LoadAttributes()
        {
            string xlsxPath = Application.streamingAssetsPath + attributeAssetPath;
            string xlsxDir = Path.GetDirectoryName(xlsxPath);
            string xlsxName = Path.GetFileNameWithoutExtension(xlsxPath);

            XlsxToCsvConverter converter = XlsxToCsvConverter.Convert(xlsxDir, xlsxPath, xlsxName);
            string[] csvFiles = converter.ConvertedCsvFiles;
            
            foreach (string csvFile in csvFiles)
            {
                MatchCollection collection = Regex.Matches(Path.GetFileName(csvFile), @"\d+.csv");
                Console.WriteLine(csvFile);
                Match match = collection[0];
                int level = int.Parse(match.Value.Substring(0, match.Value.Length - 4));

                this.LevelUpData.Add(level, null);

                using (FileStream fs = new FileStream(csvFile, FileMode.Open, FileAccess.Read))
                using (StreamReader rd = new StreamReader(fs))
                {
                    rd.ReadLine(); // NOTE: Ignore header line.

                    while (!rd.EndOfStream)
                    {
                        string[] tokens = rd.ReadLine().Split(",");

                        if (tokens[0].Equals(string.Empty))
                            continue;

                        LevelUpData levelUpData = new LevelUpData();
                        levelUpData.attributeType = tokens[0];
                        levelUpData.deltaValue = float.Parse(tokens[1]);
                        levelUpData.attributeOperation = ParseOperation(tokens[2]);
                        levelUpData.displayDescription = tokens[3];

                        levelUpData.next = this.LevelUpData[level];
                        this.LevelUpData[level] = levelUpData;
                    }
                }
            }
        }

        private AttributeOperation ParseOperation(string token)
        {
            switch (token)
            {
                case "Addition":
                case "addition":
                case "Add":
                case "add":
                case "+":
                    return AttributeOperation.Add;

                case "Multiply":
                case "multiply":
                case "Mul":
                case "mul":
                case "*":
                    return AttributeOperation.Multiply;

                case "Subtract":
                case "subtract":
                case "Sub":
                case "sub":
                case "-":
                    return AttributeOperation.Subtract;

                case "Divide":
                case "divide":
                case "Div":
                case "div":
                case "/":
                    return AttributeOperation.Divide;

                default:
                    UnityEngine.Debug.Assert(false);
                    return AttributeOperation.Add;
            }
        }

        private void AttachLevelUpData(LevelUpData data)
        {
            string type = data.attributeType;
            float deltaValue = data.deltaValue;

            if (!Attributes.ContainsKey(type))
                Attributes.Add(type, new GameplayAttribute(0));

            switch (data.attributeOperation)
            {
                case AttributeOperation.Add:
                    Attributes[type].BaseValue += deltaValue;
                    break;
                case AttributeOperation.Subtract:
                    Attributes[type].BaseValue -= deltaValue;
                    break;
                case AttributeOperation.Multiply:
                    Attributes[type].BaseValue *= deltaValue;
                    break;
                case AttributeOperation.Divide:
                    Attributes[type].BaseValue /= deltaValue;
                    break;
                default:
                    UnityEngine.Debug.Assert(false);
                    break;
            }
        }

        private void DetachLevelUpData(LevelUpData data)
        {
            string type = data.attributeType;
            float deltaValue = data.deltaValue;

            switch (data.attributeOperation)
            {
                case AttributeOperation.Add:
                    Attributes[type].BaseValue -= deltaValue;
                    break;
                case AttributeOperation.Subtract:
                    Attributes[type].BaseValue += deltaValue;
                    break;
                case AttributeOperation.Multiply:
                    Attributes[type].BaseValue /= deltaValue;
                    break;
                case AttributeOperation.Divide:
                    Attributes[type].BaseValue *= deltaValue;
                    break;
                default:
                    UnityEngine.Debug.Assert(false);
                    break;
            }
        }
    }
}