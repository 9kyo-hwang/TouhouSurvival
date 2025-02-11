using UnityEngine.UI;

namespace Unchord
{
    public class PauseCanvas : UnchordCanvas
    {
        private Button _resumeButton;
        private Button _settingsButtons;
        private Button _menuButton;

        protected override void Awake()
        {
            base.Awake();

            // TODO: 캔버스 구현 후 상대 경로를 문자열로 삽입합니다.
            _resumeButton = transform.Find("ResumeButton").GetComponent<Button>();
            _settingsButtons = transform.Find("SettingsButton").GetComponent<Button>();
            _menuButton = transform.Find("MenuButton").GetComponent<Button>();

            _resumeButton.onClick.AddListener(OnResumeButtonClick);
            _settingsButtons.onClick.AddListener(OnSettingsButtonClick);
            _menuButton.onClick.AddListener(OnMenuButtonClick);
        }

        void Update()
        {

        }

        public override void Show()
        {
            base.Show();

            s_uiManager.SettingsCanvas.ReserveReturnCanvas(this);
        }

        private void OnResumeButtonClick()
        {
            this.Hide();
            s_uiManager.GameCanvas.Show();
            s_gameManager.ResumeGame();
            s_gameManager.ReleaseTimeStopInterrupt();
        }

        private void OnSettingsButtonClick()
        {
            this.Hide();
            s_uiManager.SettingsCanvas.Show();
        }

        private void OnMenuButtonClick()
        {
            this.Hide();
            s_gameManager.HaltGame();
            s_gameManager.ReleaseTimeStopInterrupt();
        }
    }
}