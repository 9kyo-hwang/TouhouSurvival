using UnityEngine;
using UnityEngine.UI;

namespace Unchord
{
    public class GameCanvas : UnchordCanvas
    {
        private const int MAX_ABILITY_COUNT = 6;
        
        public IntegerCounterFontSO levelFont;

        private Image _expGauge;
        private Image[] _imgPlayerLevel;
        private TimerText _timer;
        private DecimalCounterText _killCount;
        private DecimalCounterText _earnedGold;

        private Button _pauseButton;

        private Transform _weaponIconParent;
        private Transform _passiveIconParent;
        private Image[] _weaponIcons;
        private Image[] _passiveIcons;
        private int _weaponIconCount;
        private int _passiveIconCount;

        private class AbilitySlot
        {
            public Image icon;

            public AbilitySlot(Transform iconParent, int siblingIndex)
            {
                icon = iconParent.GetChild(siblingIndex).GetComponent<Image>();
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
            }
        }

        protected override void Awake()
        {
            base.Awake();

            _expGauge = transform.Find("Exp/Gauge").GetComponent<Image>();
            _imgPlayerLevel = new Image[2];

            for (int i = 0; i < _imgPlayerLevel.Length; ++i)
            {
                _imgPlayerLevel[i] = transform.Find($"Exp/Level/D{i}").GetComponent<Image>();
            }

            _timer = transform.Find("Exp/TimerText").GetComponent<TimerText>();
            _killCount = transform.Find("Scores/KillText").GetComponent<DecimalCounterText>();
            _earnedGold = transform.Find("Scores/GoldText").GetComponent<DecimalCounterText>();

            _pauseButton = transform.Find("PauseButton").GetComponent<Button>();

            _weaponIcons = new Image[MAX_ABILITY_COUNT];
            _passiveIcons = new Image[MAX_ABILITY_COUNT];

            _weaponIconParent = transform.Find("ItemSlots/WeaponIcons");
            _passiveIconParent = transform.Find("ItemSlots/PassiveIcons");

            for (int i = 0; i < MAX_ABILITY_COUNT; ++i)
            {
                _weaponIcons[i] = _weaponIconParent.Find($"Icon ({i})").GetComponent<Image>();
                _passiveIcons[i] = _passiveIconParent.Find($"Icon ({i})").GetComponent<Image>();
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

        public override void UpdateKeyboardInput()
        {
            base.UpdateKeyboardInput();

            if (Input.GetKeyDown(KeyCode.Escape))
                OnPauseButtonClick();
        }

        public void Clear()
        {
            SetKillCount(0);
            SetEarnedGold(0);

            _weaponIconCount = 0;
            _passiveIconCount = 0;

            for (int i = 0; i < MAX_ABILITY_COUNT; ++i)
            {
                _weaponIcons[i].gameObject.SetActive(false);
                _weaponIcons[i].sprite = null;

                _passiveIcons[i].gameObject.SetActive(false);
                _passiveIcons[i].sprite = null;
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

            if (level > 99)
                level = 99;

            int d0 = level % 10;
            int d1 = level / 10;

            _imgPlayerLevel[0].sprite = levelFont.digitSprites[d0];
            _imgPlayerLevel[1].sprite = levelFont.digitSprites[d1];
        }

        public void SetTimer(float playtime)
        {
            _timer.SetValue(playtime);
        }

        public void SetKillCount(int count)
        {
            UnityEngine.Debug.Assert(count >= 0);

            if (count > 999999)
                count = 999999;

            _killCount.SetValue(count);
        }

        public void SetEarnedGold(int earnedGold)
        {
            UnityEngine.Debug.Assert(earnedGold >= 0);

            if (earnedGold > 999999)
                earnedGold = 999999;

            _earnedGold.SetValue(earnedGold);
        }

        public void AddWeaponIcon(Sprite icon)
        {
            _weaponIcons[_weaponIconCount].sprite = icon;
            _weaponIcons[_weaponIconCount].gameObject.SetActive(true);
            _weaponIconCount++;
        }

        public void AddPassiveIcon(Sprite icon)
        {
            _passiveIcons[_passiveIconCount].sprite = icon;
            _passiveIcons[_passiveIconCount].gameObject.SetActive(true);
            _passiveIconCount++;
        }

        private void OnPauseButtonClick()
        {
            if (!s_gameManager.IsGameStarted)
            {
                return;
            }

            if (s_gameManager.IsGamePaused)
            {
                s_gameManager.ResumeGame();
            }
            else
            {
                s_gameManager.PauseGame();
            }
        }
    }
}