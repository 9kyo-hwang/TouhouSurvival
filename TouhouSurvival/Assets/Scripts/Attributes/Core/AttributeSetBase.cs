using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unchord
{
    public abstract class AttributeSetBase<TAttributeType> : MonoBehaviour where TAttributeType : Enum
    {
        protected static GameManager s_gameManager { get; private set; }
        protected static UIManager s_uiManager { get; private set; }

        public float Level { get; private set; } = 1;
        public bool IsReachedMaxLevel => (int)Level > levelUpData.Length;

        protected float Experience { get; private set; }
        protected float ExperienceRequirement
        {
            get
            {
                int intLevel = (int)Level;

                if (IsReachedMaxLevel)
                    return 1.0f;

                return levelUpData[intLevel - 1].expRequirement;
            }
        }

        [SerializeField] private List<GameplayAttributeData<TAttributeType>> initialAttributes = new();

        protected readonly Dictionary<TAttributeType, GameplayAttribute> Attributes = new();

        public LevelUpData<TAttributeType>[] levelUpData;

        public event Action<int, float, float> OnExpChanged;
        public event Action<int, float, float> OnLevelUp;

        public GameplayAttribute this[TAttributeType attributeType]
        {
            get => Attributes[attributeType];
            set => Attributes[attributeType] = value;
        }

        protected virtual void Awake()
        {
            CreateSingletonReference();
            InitAttributes();
        }

        protected virtual void Start()
        {

        }

        private void CreateSingletonReference()
        {
            if (s_gameManager == null)
                s_gameManager = GameManager.Instance;

            if (s_uiManager == null)
                s_uiManager = UIManager.Instance;
        }

        private void InitAttributes()
        {
            // 1. 속성 추가
            // 인스펙터에서 설정된 초기 속성들을 딕셔너리에 추가
            foreach (GameplayAttributeData<TAttributeType> data in initialAttributes)
            {
                if (!Attributes.ContainsKey(data.attributeType))
                {
                    Attributes.Add(data.attributeType, new GameplayAttribute(data.baseValue, data.minValue, data.maxValue));
                }
            }

            // 2. 정의되지 않은 속성 검사
            foreach (TAttributeType type in Enum.GetValues(typeof(TAttributeType)))
            {
                if (!Attributes.ContainsKey(type))
                {
                    Debug.LogWarning($"Attribute {type} not found!");
                }
            }
        }

        public virtual void AddExperience(float amount)
        {
            if (Level > levelUpData.Length)
            {
                return;
            }

            LevelUpData<TAttributeType> data = levelUpData[(int)Level - 1];
            float remainingExp = Experience + amount;
            float requiredExp = data.expRequirement;

            if (remainingExp < requiredExp)
            {
                Experience = remainingExp;
                OnExpChanged?.Invoke((int)Level, remainingExp, requiredExp);
                return;
            }

            // LevelUp!
            while (remainingExp >= requiredExp)
            {
                remainingExp -= requiredExp;
                Experience = remainingExp;
                OnExpChanged?.Invoke((int)Level, remainingExp, requiredExp);
                
                Level += 1;
                OnLevelUp?.Invoke((int)Level, remainingExp, requiredExp);
                
                data = levelUpData[(int)Level - 1];
                requiredExp = data.expRequirement;
                Attributes[data.attributeType].BaseValue += data.deltaValue;
            }
        }
    }
}