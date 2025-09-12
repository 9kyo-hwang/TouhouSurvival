using UnityEngine;
using UnityEngine.UI;

namespace Unchord
{
    public class GameResultCanvas : UnchordCanvas
    {
        public bool IsResultButtonClicked { get; private set; }

        private TimerText _timer;
        private DecimalCounterText _killCount;
        private DecimalCounterText _earnedGold;
        private Button _btnBack;

        protected override void Awake()
        {
            base.Awake();

            _timer = transform.Find("Panel/Playtime/Text").GetComponent<TimerText>();
            _killCount = transform.Find("Panel/KillCount/Text").GetComponent<DecimalCounterText>();
            _earnedGold = transform.Find("Panel/Gold/Text").GetComponent<DecimalCounterText>();
            _btnBack = transform.Find("Panel/BackToMenuButton").GetComponent<Button>();

            _btnBack.onClick.AddListener(OnBackButtonClick);
        }

        public override void Show()
        {
            base.Show();

            _timer.SetValue(s_gameManager.ElapsedPlaytime);
            _killCount.SetValue(s_gameManager.KillCount);
            _earnedGold.SetValue(s_gameManager.EarnedGold);

            IsResultButtonClicked = false;
        }

        public override void Hide()
        {
            base.Hide();

            IsResultButtonClicked = false;
        }

        private void OnBackButtonClick()
        {
            IsResultButtonClicked = true;
        }
    }
}