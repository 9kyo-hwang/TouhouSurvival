using UnityEngine;

namespace Unchord
{
    public class PlayerAttributeSet : AttributeSetBase<PlayerAttributeType>
    {
        protected override void Awake()
        {
            base.Awake();

            OnLevelUp += HandleLevelUp;
            OnExpChanged += HandleExpChange;

            base[PlayerAttributeType.Health].OnAttributeChanged += OnHealthChanged;
        }

        protected override void Start()
        {
            base.Start();

            s_uiManager.GameCanvas.SetPlayerLevel((int)Level);
            s_uiManager.GameCanvas.SetExpGauge(base.Experience, base.ExperienceRequirement);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F4))
            {
                Debug.Log("Get 1 Exp.");
                AddExperience(1.0f);
            }
        }

        public override void AddExperience(float amount)
        {
            float expGainIncrease = Attributes[PlayerAttributeType.ExpGainIncrease].CurrentValue;
            base.AddExperience(amount * expGainIncrease);
        }

        private void HandleExpChange(int level, float remainingExp, float requiredExp)
        {
            LevelUpData<PlayerAttributeType> data = levelUpData[(int)Level - 1];
            UIManager.Instance.GameCanvas.SetExpGauge(Experience, data.expRequirement);
        
        }

        private void HandleLevelUp(int level, float remainingExp, float requiredExp)
        {
            UIManager.Instance.GameCanvas.SetPlayerLevel((int)Level);
        }

        private void OnHealthChanged(object sender, AttributeChangedEventArgs e)
        {
            // TODO: 플레이어의 현재 체력을 UI에 표시하는 코드를 이 곳에 작성합니다.
        }
    }
}
