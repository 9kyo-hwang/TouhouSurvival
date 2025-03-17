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

                for (int i = prevLevel + 1; i <= nextLevel; ++i) // 레벨 상승 시 적용
                    AttachLevelUpData(i);

                for (int i = prevLevel - 1; i >= nextLevel; --i) // 레벨 감소 시 적용
                    DetachLevelUpData(i);
            }
        }

        public int MaxLevel => levelUpData.Length + 1;

        public bool IsReachedMaxLevel => (int)Level > levelUpData.Length;

        public string attributeAssetPath;
        public string levelUpDataAssetPath;

        protected readonly Dictionary<string, GameplayAttribute> Attributes = new();
        protected readonly LevelUpData[] levelUpData;
        private int _level;

        public GameplayAttribute this[string attributeType]
        {
            get => Attributes[attributeType];
            set => Attributes[attributeType] = value;
        }

        protected virtual void Awake()
        {
            
        }

        protected virtual void Start()
        {

        }

        private void AttachLevelUpData(int level)
        {
            LevelUpData data = levelUpData[level - 1];
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
            LevelUpData data = levelUpData[level - 1];
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