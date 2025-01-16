using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unchord
{
    public class SettingsCanvas : UnchordCanvas
    {
        private Button _btnBack;
        private Slider _sliderBgm;
        private Slider _sliderSfx;
        private Button _btnBgmMute;
        private Button _btnSfxMute;
        private TextMeshProUGUI _txtBgmAmp;
        private TextMeshProUGUI _txtSfxAmp;

        private void Awake()
        {
            _btnBack = transform.Find("SettingsPanel/BackButton").GetComponent<Button>();
            _sliderBgm = transform.Find("SettingsPanel/BgmSlider").GetComponent<Slider>();
            _sliderSfx = transform.Find("SettingsPanel/SfxSlider").GetComponent<Slider>();
            _btnBgmMute = _sliderBgm.transform.Find("MuteButton").GetComponent<Button>();
            _btnSfxMute = _sliderSfx.transform.Find("MuteButton").GetComponent<Button>();
            _txtBgmAmp = _sliderBgm.transform.Find("VolumeLabel/ContentText").GetComponent<TextMeshProUGUI>();
            _txtSfxAmp = _sliderSfx.transform.Find("VolumeLabel/ContentText").GetComponent<TextMeshProUGUI>();

            _btnBack.onClick.AddListener(OnBackButtonClick);
            _sliderBgm.onValueChanged.AddListener(OnBgmSliderValueChanged);
            _sliderSfx.onValueChanged.AddListener(OnSfxSliderValueChanged);
            _btnBgmMute.onClick.AddListener(OnBgmMuteButtonClick);
            _btnSfxMute.onClick.AddListener(OnSfxMuteButtonClick);
        }

        public override void Show()
        {
            base.Show();

            SoundChannel chanBgm = SoundManager.Instance.BGM;
            SoundChannel chanSfx = SoundManager.Instance.SFX;

            MoveSliderHandle(chanBgm, _sliderBgm);
            MoveSliderHandle(chanSfx, _sliderSfx);
            RenderVolumeText(chanBgm, _sliderBgm, _txtBgmAmp);
            RenderVolumeText(chanSfx, _sliderSfx, _txtSfxAmp);
        }

        private void OnBackButtonClick()
        {
            this.Hide();
            UIManager.Instance.LobbyCanvas.Show();
        }

        private void OnBgmSliderValueChanged(float value)
        {
            SoundChannel channel = SoundManager.Instance.BGM;

            float volume = value / _sliderBgm.maxValue;
            Debug.Assert(volume >= 0.0f && volume <= 1.0f, "Volume should be in range between 0 and 1");

            // NOTE: 코드 순서 바뀌지 않도록 주의.
            channel.ChangeVolume(volume);
            channel.IsMuted = false;
            RenderVolumeText(channel, _sliderBgm, _txtBgmAmp);
        }

        private void OnSfxSliderValueChanged(float value)
        {
            SoundChannel channel = SoundManager.Instance.SFX;

            float volume = value / _sliderSfx.maxValue;
            Debug.Assert(volume >= 0.0f && volume <= 1.0f, "Volume should be in range between 0 and 1");

            // NOTE: 코드 순서 바뀌지 않도록 주의.
            channel.ChangeVolume(volume);
            channel.IsMuted = false;
            RenderVolumeText(channel, _sliderSfx, _txtSfxAmp);
        }

        private void OnBgmMuteButtonClick()
        {
            SoundChannel channel = SoundManager.Instance.BGM;

            ToggleMute(channel, _sliderBgm);
            RenderVolumeText(channel, _sliderBgm, _txtBgmAmp);
        }

        private void OnSfxMuteButtonClick()
        {
            SoundChannel channel = SoundManager.Instance.SFX;

            ToggleMute(channel, _sliderSfx);
            RenderVolumeText(channel, _sliderSfx, _txtSfxAmp);
        }

        private void ToggleMute(SoundChannel channel, Slider slider)
        {
            channel.IsMuted ^= true;
            MoveSliderHandle(channel, slider);
        }

        private void RenderVolumeText(SoundChannel channel, Slider slider, TextMeshProUGUI textComponent)
        {
            int discreteVolume = Mathf.RoundToInt(channel.Volume * slider.maxValue);
            textComponent.text = string.Format("{0}", discreteVolume);
        }

        private void MoveSliderHandle(SoundChannel channel, Slider slider)
        {
            slider.value = channel.Volume * slider.maxValue;
        }
    }
}