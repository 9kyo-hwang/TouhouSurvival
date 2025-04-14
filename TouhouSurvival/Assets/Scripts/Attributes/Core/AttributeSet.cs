using System;
using System.Collections.Generic;
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
                    AttachLevelUpData(i);

                for (int i = Math.Min(prevLevel - 1, this.MaxLevel - 1); i >= Math.Max(nextLevel, 1); --i) // 레벨 감소 시 적용
                    DetachLevelUpData(i);
            }
        }

        public int MaxLevel => LevelUpData.Count + 1;

        public bool IsReachedMaxLevel => (int)Level > LevelUpData.Count;

        public string attributeAssetPath;

        public Dictionary<string, GameplayAttribute> Attributes { get; private set; }
        public List<LevelUpData> LevelUpData { get; private set; }

        private int _level;

        public GameplayAttribute this[string attributeType]
        {
            get => Attributes[attributeType];
            set => Attributes[attributeType] = value;
        }

        protected virtual void Awake()
        {
            Attributes = new Dictionary<string, GameplayAttribute>();
            LevelUpData = new List<LevelUpData>();

            AttributeSetBuilder.LoadAttributes(this);
        }

        protected virtual void Start()
        {

        }

        private void AttachLevelUpData(int level)
        {
            LevelUpData data = LevelUpData[level - 1];
            string type = data.attributeType;
            float deltaValue = data.deltaValue;

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

        private void DetachLevelUpData(int level)
        {
            LevelUpData data = LevelUpData[level - 1];
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