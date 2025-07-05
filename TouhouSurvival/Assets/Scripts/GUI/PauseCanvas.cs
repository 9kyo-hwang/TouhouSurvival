using UnityEngine;
using UnityEngine.UI;

namespace Unchord
{
    public class PauseCanvas : UnchordCanvas
    {
        private Button _mainButton;
        private Button _resumeButton;

        private VolumeSlider _bgmSlider;
        private VolumeSlider _sfxSlider;
        
        protected override void Awake()
        {
            base.Awake();

            _mainButton = transform.Find("Navigators/MainButton").GetComponent<Button>();
            _resumeButton = transform.Find("Navigators/ResumeButton").GetComponent<Button>();

            _resumeButton.onClick.AddListener(OnResumeButtonClick);
            _mainButton.onClick.AddListener(OnMainButtonClick);

            _bgmSlider = new VolumeSlider(transform.Find("Setting/BgmSlider"), SoundManager.Instance.BGM);
            _sfxSlider = new VolumeSlider(transform.Find("Setting/SfxSlider"), SoundManager.Instance.SFX);
        }

        public override void Show()
        {
            base.Show();

            s_uiManager.SingleColorCanvas0.LayerBackOf(s_uiManager.GameCanvas);
            s_uiManager.SingleColorCanvas0.Show();
        }

        public override void UpdateKeyboardInput()
        {
            base.UpdateKeyboardInput();

            if (Input.GetKeyDown(KeyCode.Escape))
                OnResumeButtonClick();
        }

        private void OnMainButtonClick()
        {
            s_uiManager.SingleColorCanvas0.Hide();
            this.Hide();

            s_gameManager.HaltGame();
            s_gameManager.ReleaseTimeStopInterrupt();
        }

        private void OnResumeButtonClick()
        {
            s_uiManager.SingleColorCanvas0.Hide();
            this.Hide();
            
            s_uiManager.GameCanvas.Show();
            s_gameManager.ResumeGame();
            s_gameManager.ReleaseTimeStopInterrupt();
        }
    }
}