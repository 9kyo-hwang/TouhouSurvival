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

        private Image[] _imgSpecials;
        
        protected override void Awake()
        {
            base.Awake();

            _mainButton = transform.Find("Navigators/MainButton").GetComponent<Button>();
            _resumeButton = transform.Find("Navigators/ResumeButton").GetComponent<Button>();

            _resumeButton.onClick.AddListener(OnResumeButtonClick);
            _mainButton.onClick.AddListener(OnMainButtonClick);

            _bgmSlider = new VolumeSlider(transform.Find("Setting/BgmSlider"), SoundManager.Instance.BGM);
            _sfxSlider = new VolumeSlider(transform.Find("Setting/SfxSlider"), SoundManager.Instance.SFX);

            _imgSpecials = new Image[7];
            _imgSpecials[0] = transform.Find("Special/Icon (0)").GetComponent<Image>();
            _imgSpecials[1] = transform.Find("Special/Icon (1-1)").GetComponent<Image>();
            _imgSpecials[2] = transform.Find("Special/Icon (1-2)").GetComponent<Image>();
            _imgSpecials[3] = transform.Find("Special/Icon (1-3)").GetComponent<Image>();
            _imgSpecials[4] = transform.Find("Special/Icon (2-1)").GetComponent<Image>();
            _imgSpecials[5] = transform.Find("Special/Icon (2-2)").GetComponent<Image>();
            _imgSpecials[6] = transform.Find("Special/Icon (2-3)").GetComponent<Image>();

            for (int i = 1; i < _imgSpecials.Length; ++i)
            {
                _imgSpecials[i].gameObject.SetActive(false);
            }
        }

        public override void Show()
        {
            base.Show();

            _bgmSlider.OnShow();
            _sfxSlider.OnShow();

            // TODO: 어빌리티 매니저를 public property로 열어서 접근을 할까?
            _imgSpecials[0].sprite = s_gameManager.Player.iconSpecial;
            ShowSpecial(1, s_gameManager.Player.SpecialTransform0.GetChild(0).GetComponent<SpecialAbilityComponent>());
            ShowSpecial(2, s_gameManager.Player.SpecialTransform0.GetChild(1).GetComponent<SpecialAbilityComponent>());
            ShowSpecial(3, s_gameManager.Player.SpecialTransform0.GetChild(2).GetComponent<SpecialAbilityComponent>());
            ShowSpecial(4, s_gameManager.Player.SpecialTransform1.GetChild(0).GetComponent<SpecialAbilityComponent>());
            ShowSpecial(5, s_gameManager.Player.SpecialTransform1.GetChild(1).GetComponent<SpecialAbilityComponent>());
            ShowSpecial(6, s_gameManager.Player.SpecialTransform1.GetChild(2).GetComponent<SpecialAbilityComponent>());

            s_uiManager.SingleColorCanvas0.LayerBackOf(s_uiManager.GameCanvas);
            s_uiManager.SingleColorCanvas0.Show();
        }

        private void ShowSpecial(int idxSpecial, SpecialAbilityComponent special)
        {
            _imgSpecials[idxSpecial].sprite = special.DisplayIcon;
            _imgSpecials[idxSpecial].gameObject.SetActive(special.CurrentLevel > 0);
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
        }

        private void OnResumeButtonClick()
        {
            s_uiManager.SingleColorCanvas0.Hide();
            this.Hide();

            s_gameManager.ResumeGame();
        }
    }
}