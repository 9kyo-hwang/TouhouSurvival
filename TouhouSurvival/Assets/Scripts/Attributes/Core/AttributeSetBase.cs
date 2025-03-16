using System;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

namespace Unchord
{
    public abstract class AttributeSetBase<TAttributeType> : MonoBehaviour where TAttributeType : Enum
    {
        protected static GameManager s_gameManager { get; private set; }
        protected static UIManager s_uiManager { get; private set; }
        protected static WorldUIManager s_wuiManager { get; private set; }

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

            if (s_wuiManager == null)
                s_wuiManager = WorldUIManager.Instance;
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

            // TODO: 어떤 방식으로 코드를 작성할지 합의가 필요해 보임.
            // 코드 작성 합의 후 코드를 확정, 이후에는 코드 로직을 건드리면 안 됨.
            #region Level Up Code
            // 레벨업 코드 - 작성자: ㅊㅁㅅ
            // LevelUp!
            while (!IsReachedMaxLevel && remainingExp >= requiredExp)
            {
                int prevLevel = (int)Level;

                remainingExp -= requiredExp;
                Experience = remainingExp;
                Level += 1;
                requiredExp = ExperienceRequirement;
                ApplyLevelUpData(prevLevel);

                // 모든 값이 다 바뀐 후 최종적으로 한 번만 UI를 갱신하기 때문에
                // 이벤트 함수가 경험치, 레벨, 스텟이 전부 바뀐 후 호출되는 것이 논리적으로 맞다고 생각함.
                int intLevel = (int)Level;
                OnExpChanged?.Invoke(intLevel, remainingExp, requiredExp); // 이 곳에서 경험치, 레벨을 UI에 갱신하는 코드가 호출됨.
                OnLevelUp?.Invoke(intLevel, remainingExp, requiredExp); // 이 곳에는 경험치, 레벨 UI를 갱신하는 코드가 없다고 생각하고 이벤트 호출을 전부 뒤로 미룸.
            }

            // 레벨업 코드 - 작성자: ㄱㄱㅎ
            // LevelUp!
            //while (remainingExp >= requiredExp)
            //{
            //    remainingExp -= requiredExp;
            //    Experience = remainingExp;

            // 1. 현재 레벨에서의 레벨, 경험치, 경헙치 요구량을 보여주는 부분
            //    OnExpChanged?.Invoke((int)Level, remainingExp, requiredExp);

            //    Level += 1;

            // 만약에 여기서도 UI를 갱신하는 코드가 호출된다고 하면 말이 됨.
            // 2. 레벨업 후 레벨, 경험치, 경험치 요구량을 보여주는 부분
            //    OnLevelUp?.Invoke((int)Level, remainingExp, requiredExp);

            // 3. 만약 상기한 1, 2의 동작을 가정하고 코딩했다 하면
            //    경험치 요구량은 레벨업 후 도착한 레벨의 경험치 요구량을 UI에 표시하는 것이 적절하므로
            //    아래 코드 두 줄은 OnLevelUp 이벤트 호출 직전에 와야됨.
            //    data = levelUpData[(int)Level - 1];
            //    requiredExp = data.expRequirement;

            // 4. 만약 상기한 1, 2의 동작이 아닌 다른 방식의 동작을 가정하고 코딩했다면
            //    차후 회의에서 간단한 설명을 부탁.

            //    Attributes[data.attributeType].BaseValue += data.deltaValue;
            //}
            #endregion
        }

        // TODO: 경험치 상승과 레벨업 데이터 적용 코드의 분리 검토 필요.
        public void ApplyLevelUpData(int level)
        {
            LevelUpData<TAttributeType> data = levelUpData[level - 1];
            TAttributeType type = data.attributeType;
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
    }
}