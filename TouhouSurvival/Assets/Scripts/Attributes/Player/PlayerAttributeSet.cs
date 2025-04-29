using System;
using UnityEngine;

namespace Unchord
{
    public class PlayerAttributeSet : AttributeSet
    {
        protected static GameManager s_gameManager { get; private set; }
        protected static UIManager s_uiManager { get; private set; }
        protected static WorldUIManager s_wuiManager { get; private set; }

        // TODO: EventHandler<> delegate 형식으로 전환합니다.
        public Action<int, float, float> OnExpChanged;
        public Action<int, float, float> OnLevelUp;
        public Action<float> OnSpellGaugeChanged;

        protected float Experience { get; private set; }
        protected float ExperienceRequirement
        {
            get
            {
                int intLevel = (int)Level;

                if (IsReachedMaxLevel || this.Level == 0)
                    return 1.0f;

                //return LevelUpData[intLevel - 1].expRequirement;
                return 5;
            }
        }

        public float SpellGaugeValue { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            
            CreateSingletonReference();

            OnExpChanged += HandleExpChange;
            OnLevelUp += HandleLevelUp;
            OnSpellGaugeChanged += HandleSpellGauge;

            base[PlayerAttributeType.Health].OnAttributeChanged += OnHealthChanged;
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

        protected override void Start()
        {
            base.Start();

            s_uiManager.GameCanvas.SetPlayerLevel((int)Level);
            s_uiManager.GameCanvas.SetExpGauge(Experience, ExperienceRequirement);

            s_wuiManager.SetPlayerHealthPosition(transform.position + Vector3.up * 0.7f);
            s_wuiManager.SetPlayerHealthValue(base[PlayerAttributeType.Health].CurrentValue, 10.0f);

            SpellGaugeValue = 0.0f;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F4))
            {
                Debug.Log("Get 1 Exp.");
                AddExperience(1.0f);
            }

            if (Input.GetKeyDown(KeyCode.Y))
            {
                Debug.Log("Get 1 Health.");
                base[PlayerAttributeType.Health].CurrentValue += 1.0f;
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                Debug.Log("Sub 1 Health.");
                base[PlayerAttributeType.Health].CurrentValue -= 1.0f;
            }

            s_wuiManager.SetPlayerHealthPosition(transform.position + Vector3.up * 0.7f);
        }

        private void HandleExpChange(int level, float remainingExp, float requiredExp)
        {
            s_uiManager.GameCanvas.SetExpGauge(Experience, ExperienceRequirement);
        }

        private void HandleLevelUp(int level, float remainingExp, float requiredExp)
        {
            s_uiManager.GameCanvas.SetPlayerLevel((int)Level);

            // 최대 레벨에 도달하면 경험치바를 항상 가득 채워놓음.
            if (IsReachedMaxLevel)
                s_uiManager.GameCanvas.SetExpGauge(1.0f, 1.0f);
        }

        private void HandleSpellGauge(float currentSpellGaugeValue)
        {
            float max = this[PlayerAttributeType.MaxSpellCount].CurrentValue;

            s_uiManager.GameCanvas.SetSpellGauge(currentSpellGaugeValue, max);
        }

        private void OnHealthChanged(object sender, AttributeChangedEventArgs e)
        {
            // TODO: 플레이어의 현재 체력을 UI에 표시하는 코드를 이 곳에 작성합니다.
            s_wuiManager.SetPlayerHealthValue(base[PlayerAttributeType.Health].CurrentValue, 10.0f);
        }

        public void AddExperience(float amount)
        {
            if (IsReachedMaxLevel)
            {
                return;
            }

            LevelUpData data = LevelUpData[Level - 1];
            float remainingExp = Experience + amount * (1.0f + Attributes[PlayerAttributeType.ExpGainIncrease].CurrentValue);
            //float requiredExp = data.expRequirement;
            float requiredExp = 5;

            if (remainingExp < requiredExp)
            {
                Experience = remainingExp;
                OnExpChanged?.Invoke(Level, remainingExp, requiredExp);
                return;
            }

            // LevelUp!
            while (!IsReachedMaxLevel && remainingExp >= requiredExp)
            {
                int prevLevel = Level;

                remainingExp -= requiredExp;
                Experience = remainingExp;
                Level += 1;
                requiredExp = ExperienceRequirement;

                int nextLevel = Level;
                OnExpChanged?.Invoke(nextLevel, remainingExp, requiredExp);
                OnLevelUp?.Invoke(nextLevel, remainingExp, requiredExp);
            }
        }

        public void AddSpellGauge(float amount)
        {
            float max = this[PlayerAttributeType.MaxSpellCount].CurrentValue;

            SpellGaugeValue = Mathf.Clamp(SpellGaugeValue + amount, 0.0f, max);

            OnSpellGaugeChanged?.Invoke(SpellGaugeValue);
        }
    }
}
