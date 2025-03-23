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

        protected float Experience { get; private set; }
        protected float ExperienceRequirement
        {
            get
            {
                int intLevel = (int)Level;

                if (IsReachedMaxLevel || this.Level == 0)
                    return 1.0f;

                return levelUpData[intLevel - 1].expRequirement;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            
            CreateSingletonReference();

            OnExpChanged += HandleExpChange;
            OnLevelUp += HandleLevelUp;

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

            LevelUpData data = levelUpData[Level - 1];
            float remainingExp = Experience + amount * (1.0f + Attributes[PlayerAttributeType.ExpGainIncrease].CurrentValue);
            float requiredExp = data.expRequirement;

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
    }
}
