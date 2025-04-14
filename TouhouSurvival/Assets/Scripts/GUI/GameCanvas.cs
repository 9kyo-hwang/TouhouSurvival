using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unchord
{
    public class GameCanvas : UnchordCanvas
    {
        private const int MAX_ABILITY_COUNT = 6;

        private Image _expGauge;
        private IntegerCounter _playerLevel;
        private TextMeshProUGUI _timer;
        private TextMeshProUGUI _killCount;
        private TextMeshProUGUI _earnedGold;

        private Button _pauseButton;

        private AbilitySlot[] _weaponSlots;
        //private AbilitySlot[] _passiveSlots;

        private class AbilitySlot
        {
            public Image icon;
            public IntegerCounter level;

            public AbilitySlot(Transform iconParent, Transform levelParent, int siblingIndex)
            {
                icon = iconParent.GetChild(siblingIndex).GetComponent<Image>();
                level = levelParent.GetChild(siblingIndex).GetComponent<IntegerCounter>();
            }

            public void Enable()
            {
                SetActive(true);
            }

            public void Disable()
            {
                SetActive(false);
            }

            public void SetActive(bool active)
            {
                icon.gameObject.SetActive(active);
                level.gameObject.SetActive(active);
            }
        }

        protected override void Awake()
        {
            base.Awake();

            _expGauge = transform.Find("Exp/Gauge").GetComponent<Image>();
            _playerLevel = transform.Find("Exp/Level").GetComponent<IntegerCounter>();
            _timer = transform.Find("TimerText").GetComponent<TextMeshProUGUI>();
            _killCount = transform.Find("KillCount/Value").GetComponent<TextMeshProUGUI>();
            _earnedGold = transform.Find("EarnedGold/Value").GetComponent<TextMeshProUGUI>();

            _pauseButton = transform.Find("PauseButton").GetComponent<Button>();

            _weaponSlots = new AbilitySlot[MAX_ABILITY_COUNT];
            //_passiveSlots = new AbilitySlot[MAX_ABILITY_COUNT];

            Transform weaponIconParent = transform.Find("WeaponSlot/Icons");
            Transform weaponLevelParent = transform.Find("WeaponSlot/Levels");
            
            //Transform passiveIconParent = transform.Find("PassiveSlot/Icons");
            //Transform passiveLevelParent = transform.Find("PassiveSlot/Levels");

            for (int i = 0; i < MAX_ABILITY_COUNT; ++i)
            {
                _weaponSlots[i] = new AbilitySlot(weaponIconParent, weaponLevelParent, i);
                //_passiveSlots[i] = new AbilitySlot(passiveIconParent, passiveLevelParent, i);
            }

            _pauseButton.onClick.AddListener(OnPauseButtonClick);
        }

        public override void Show()
        {
            base.Show();

            SetKillCount(s_gameManager.KillCount);
            SetEarnedGold(s_gameManager.EarnedGold);
            s_wuiManager.ShowPlayerHealth();

            s_uiManager.MainIllustCanvas.Hide();
            //SoundManager.Instance.BGM.ChangeSoundEvent("event:/BGM/TestBGM");
        }

        public override void Hide()
        {
            base.Hide();

            s_wuiManager.HidePlayerHealth();
        }

        public void Clear()
        {
            SetKillCount(0);
            SetEarnedGold(0);

            for (int i = 0; i < MAX_ABILITY_COUNT; ++i)
            {
                DisableWeaponSlot(i);
                //DisablePassiveSlot(i);
            }
        }

        public void SetExpGauge(float value, float max)
        {
            UnityEngine.Debug.Assert(max > 0.0f);

            _expGauge.fillAmount = Mathf.Clamp01(value / max);
        }

        public void SetPlayerLevel(int level)
        {
            UnityEngine.Debug.Assert(level >= 0);

            _playerLevel.SetValue(level);
        }

        public void SetTimer(int playtime)
        {
            int s = playtime % 60;
            int m = playtime / 60;

            _timer.text = $"{m.ToString("D02")}:{s.ToString("D02")}";
        }

        public void SetKillCount(int count)
        {
            UnityEngine.Debug.Assert(count >= 0);

            _killCount.text = count.ToString();
        }

        public void SetEarnedGold(int earnedGold)
        {
            UnityEngine.Debug.Assert(earnedGold >= 0);

            _earnedGold.text = earnedGold.ToString();
        }

        public void EnableWeaponSlot(int index)
        {
            UnityEngine.Debug.Assert(index >= 0 && index < MAX_ABILITY_COUNT);

            _weaponSlots[index].Enable();
        }

        public void DisableWeaponSlot(int index)
        {
            UnityEngine.Debug.Assert(index >= 0 && index < MAX_ABILITY_COUNT);

            _weaponSlots[index].Disable();
        }

        public void SetWeaponIcon(int index, Sprite weaponIcon)
        {
            UnityEngine.Debug.Assert(index >= 0 && index < MAX_ABILITY_COUNT);

            _weaponSlots[index].icon.sprite = weaponIcon;
        }

        public void SetWeaponLevel(int index, int level)
        {
            UnityEngine.Debug.Assert(index >= 0 && index < MAX_ABILITY_COUNT);

            _weaponSlots[index].level.SetValue(level);
        }

        //public void EnablePassiveSlot(int index)
        //{
        //    UnityEngine.Debug.Assert(index >= 0 && index < MAX_ABILITY_COUNT);

        //    _passiveSlots[index].Enable();
        //}

        //public void SetPassiveIcon(int index, Sprite passiveIcon)
        //{
        //    UnityEngine.Debug.Assert(index >= 0 && index < MAX_ABILITY_COUNT);

        //    _passiveSlots[index].icon.sprite = passiveIcon;
        //}

        //public void SetPassiveLevel(int index, int level)
        //{
        //    UnityEngine.Debug.Assert(index >= 0 && index < MAX_ABILITY_COUNT);

        //    _passiveSlots[index].level.SetValue(level);
        //}

        private void OnPauseButtonClick()
        {
            s_gameManager.InterruptTimeStop();
            s_gameManager.PauseGame();
            this.Hide();
            s_uiManager.PauseCanvas.Show();
        }
    }
}