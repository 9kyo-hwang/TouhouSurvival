using System;
using UnityEngine;

namespace Unchord
{
    public class PlayerAttributeSet : AttributeSet
    {
        protected static GameManager s_gameManager { get; private set; }
        protected static UIManager s_uiManager { get; private set; }
        protected static WorldUIManager s_wuiManager { get; private set; }

        public void Initialize(PlayerLevelSystem levelSystem)
        {
            levelSystem.OnLevelUp += HandleLevelUp;
            levelSystem.OnExperienceChanged += HandleExpChange;
            
            s_uiManager.GameCanvas.SetPlayerLevel(levelSystem.Level);
            s_uiManager.GameCanvas.SetExpGauge(levelSystem.Experience, levelSystem.TotalExperienceForCurrentLevel);
            s_wuiManager.SetPlayerHealthPosition(transform.position + Vector3.up * 0.7f);
            s_wuiManager.SetPlayerHealthValue(base[PlayerAttributeType.Health].CurrentValue, 10.0f);
        }

        protected override void Awake()
        {
            base.Awake();
            
            CreateSingletonReference();

            base[PlayerAttributeType.Health].OnAttributeChanged += OnHealthChanged;
        }

        private void CreateSingletonReference()
        {
            if (!s_gameManager)
                s_gameManager = GameManager.Instance;

            if (!s_uiManager)
                s_uiManager = UIManager.Instance;

            if (!s_wuiManager)
                s_wuiManager = WorldUIManager.Instance;
        }

        protected override void Start()
        {
            base.Start();
        }

        private void Update()
        {
            s_wuiManager.SetPlayerHealthPosition(transform.position + Vector3.up * 0.7f);
        }

        private void HandleExpChange(object sender, ExperienceChangedEventArgs e)
        {
            s_uiManager.GameCanvas.SetExpGauge(e.CurrentExperience, e.TotalExperience);
        }

        private void HandleLevelUp(object sender, LevelUpEventArgs e)
        {
            s_uiManager.GameCanvas.SetPlayerLevel(e.CurrentLevel);

            // 최대 레벨에 도달하면 경험치바를 항상 가득 채워놓음.
            //if (IsReachedMaxLevel)
            //    s_uiManager.GameCanvas.SetExpGauge(1.0f, 1.0f);
        }

        private void OnHealthChanged(object sender, AttributeChangedEventArgs e)
        {
            // TODO: 플레이어의 현재 체력을 UI에 표시하는 코드를 이 곳에 작성합니다.
            s_wuiManager.SetPlayerHealthValue(base[PlayerAttributeType.Health].CurrentValue, 10.0f);
        }
    }
}
