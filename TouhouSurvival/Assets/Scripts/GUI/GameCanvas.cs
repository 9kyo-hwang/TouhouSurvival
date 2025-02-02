using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unchord
{
    public class GameCanvas : UnchordCanvas
    {
        private const int MAX_ABILITY_COUNT = 6;

        private Image _expGauge;
        private TextMeshProUGUI _playerLevel;
        private TextMeshProUGUI _timer;
        private TextMeshProUGUI _killCount;
        private TextMeshProUGUI _earnedGold;

        private Button _pauseButton;

        private Transform _weaponIconRoot;
        private Transform _passiveIconRoot;

        private Image[] _weaponIcons;
        private Image[] _passiveIcons;

        private Transform[] _weaponLevels;
        private Transform[] _passiveLevels;

        protected override void Awake()
        {
            base.Awake();

            _expGauge = transform.Find("ExpFrame/ExpGauge").GetComponent<Image>();
            _playerLevel = transform.Find("InfoLeft/PlayerLevel/TextPanel/Text").GetComponent<TextMeshProUGUI>();
            _timer = transform.Find("TimerText").GetComponent<TextMeshProUGUI>();
            _killCount = transform.Find("InfoRight/KillCount/TextPanel/Text").GetComponent<TextMeshProUGUI>();
            _earnedGold = transform.Find("InfoRight/EarnedGold/TextPanel/Text").GetComponent<TextMeshProUGUI>();

            _pauseButton = transform.Find("InfoRight/PauseButton").GetComponent<Button>();

            _weaponIconRoot = transform.Find("AbilityLeft");
            _passiveIconRoot = transform.Find("AbilityRight");

            _weaponIcons = new Image[MAX_ABILITY_COUNT];
            _passiveIcons = new Image[MAX_ABILITY_COUNT];

            _weaponLevels = new Transform[MAX_ABILITY_COUNT];
            _passiveLevels = new Transform[MAX_ABILITY_COUNT];

            for (int i = 0; i < MAX_ABILITY_COUNT; ++i)
            {
                Image imgw = _weaponIconRoot.GetChild(i).Find("IconFrame/Icon").GetComponent<Image>();
                Transform trw = _weaponIconRoot.GetChild(i).Find("Level");

                Image imgp = _passiveIconRoot.GetChild(i).Find("IconFrame/Icon").GetComponent<Image>();
                Transform trp = _passiveIconRoot.GetChild(i).Find("Level");

                _weaponIcons[i] = imgw;
                _weaponLevels[i] = trw;

                _passiveIcons[i] = imgp;
                _passiveLevels[i] = trp;
            }

            _pauseButton.onClick.AddListener(OnPauseButtonClick);
        }

        public void SetExpGauge(float value, float max)
        {
            UnityEngine.Debug.Assert(max > 0.0f);

            _expGauge.fillAmount = Mathf.Clamp01(value / max);
        }

        public void SetPlayerLevel(int level)
        {
            UnityEngine.Debug.Assert(level >= 0);

            _playerLevel.text = $"Lv. {level}";
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

        public void SetWeaponIcon(int index, Sprite iconSprite)
        {
            UnityEngine.Debug.Assert(index >= 0 && index < MAX_ABILITY_COUNT);

            _weaponIcons[index].sprite = iconSprite;
        }

        public void SetWeaponLevel(int index, int level)
        {
            throw new NotImplementedException();

            UnityEngine.Debug.Assert(index >= 0 && index < MAX_ABILITY_COUNT);

            Transform t = _weaponLevels[index];
        }

        public void SetPassiveIcon(int index, Sprite iconSprite)
        {
            Debug.Assert(index >= 0 && index < MAX_ABILITY_COUNT);

            _passiveIcons[index].sprite = iconSprite;
        }

        public void SetPassiveLevel(int index, int level)
        {
            throw new NotImplementedException();

            UnityEngine.Debug.Assert(index >= 0 && index < MAX_ABILITY_COUNT);

            Transform t = _passiveLevels[index];
        }

        private void OnPauseButtonClick()
        {
            s_gameManager.PauseGame();
            this.Hide();
            s_uiManager.PauseCanvas.Show();
        }
    }
}