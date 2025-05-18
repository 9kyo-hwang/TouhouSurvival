using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Unchord
{
    public abstract class AttributeSet : MonoBehaviour
    {
        public string attributeAssetPath;
        public string attributeModifierPath;

        protected Dictionary<string, GameplayAttribute> Attributes { get; private set; }
        protected SortedList<int, GameplayAttributeModifier> Modifiers { get; set; }
        public int MaxLevel => Modifiers.Last().Key;
        
        public GameplayAttribute this[string attributeType]
        {
            get => Attributes[attributeType];
            set => Attributes[attributeType] = value;
        }
        
        //public void Initialize(EventHandler<LevelUpEventArgs> onLevelUp)
        //{
        //    onLevelUp += HandleLevelUp;
        //}
        {
        }

        public void ApplyModifiers(SortedList<int, GameplayAttributeModifier> modifiers, int reachedLevel, string tag = null)
        {
            GameplayAttributeModifier modifier = modifiers[reachedLevel];

            while (modifier != null)
            {
                if (modifier.tag != null && !modifier.tag.Equals(tag))
                    continue;

                Attributes[modifier.key].AddModifier(modifier);
                modifier = modifier.next;
            }
        }

        protected virtual void Awake()
        {
            if (!attributeAssetPath.Equals(string.Empty))
                Attributes = LoadAttributes(attributeAssetPath);
            else
            {
                Attributes = new Dictionary<string, GameplayAttribute>(8);
            }

            if (!attributeModifierPath.Equals(string.Empty))
                Modifiers = GameplayAttributeModifier.LoadAttributeModifiers(attributeModifierPath);
            else
            {
                Modifiers = new SortedList<int, GameplayAttributeModifier>();
                Modifiers.Add(0, null);
            }
        }

        protected virtual void Start()
        {
            
        }

        private Dictionary<string, GameplayAttribute> LoadAttributes(string xlsxFilePath)
        {
            Dictionary<string, GameplayAttribute> attributes = new Dictionary<string, GameplayAttribute>();

            string xlsxPath = Application.streamingAssetsPath + xlsxFilePath;
            string xlsxDir = Path.GetDirectoryName(xlsxPath);
            string xlsxName = Path.GetFileNameWithoutExtension(xlsxPath);

            XlsxToCsvConverter.Convert(xlsxDir, xlsxPath, xlsxName);

            using FileStream fs = new FileStream(xlsxDir + $"\\{xlsxName}+base.csv", FileMode.Open, FileAccess.Read);
            using (StreamReader rd = new StreamReader(fs))
            {
                rd.ReadLine(); // NOTE: Ignore header line.

                while (!rd.EndOfStream)
                {
                    string[] tokens = rd.ReadLine().Split(",");

                    if (tokens[0].Equals(string.Empty))
                        continue;

                    attributes.Add(tokens[0], new GameplayAttribute(float.Parse(tokens[1])));
                }
            }

            return attributes;
        }

        //private void HandleLevelUp(object sender, LevelUpEventArgs e)
        //{
        //    int level = e.CurrentLevel;

        //    this.ApplyModifiers(this.Modifiers, level);
        //}
    }
}