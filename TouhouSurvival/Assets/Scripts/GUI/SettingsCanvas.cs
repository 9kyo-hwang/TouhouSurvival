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
        private TextMeshProUGUI _txtBgm;
        private TextMeshProUGUI _txtSfx;

        private UnchordCanvas _reservedReturnCanvas;

        protected override void Awake()
        {
            _btnBack = transform.Find("SettingsPanel/BackButton").GetComponent<Button>();
            _sliderBgm = transform.Find("SettingsPanel/BgmSlider").GetComponent<Slider>();
            _sliderSfx = transform.Find("SettingsPanel/SfxSlider").GetComponent<Slider>();
            _btnBgmMute = transform.Find("SettingsPanel/BgmMuteButton").GetComponent<Button>();
            _btnSfxMute = transform.Find("SettingsPanel/SfxMuteButton").GetComponent<Button>();
            _txtBgm = _sliderBgm.transform.Find("VolumeLabel/ContentText").GetComponent<TextMeshProUGUI>();
            _txtSfx = _sliderSfx.transform.Find("VolumeLabel/ContentText").GetComponent<TextMeshProUGUI>();

            _btnBack.onClick.AddListener(OnBackButtonClick);
            _btnBgmMute.onClick.AddListener(OnBgmMuteButtonClick);
            _btnSfxMute.onClick.AddListener(OnSfxMuteButtonClick);
        }

        public override void Show()
        {
            base.Show();

            SoundChannel chanBgm = SoundManager.Instance.BGM;
            SoundChannel chanSfx = SoundManager.Instance.SFX;

            _sliderBgm.value = GetSliderVolume(chanBgm, _sliderBgm);
            _sliderSfx.value = GetSliderVolume(chanSfx, _sliderSfx);

            _sliderBgm.onValueChanged.AddListener(OnBgmSliderValueChanged);
            _sliderSfx.onValueChanged.AddListener(OnSfxSliderValueChanged);

            _txtBgm.text = GetVolumeString(_sliderBgm.value);
            _txtSfx.text = GetVolumeString(_sliderSfx.value);
        }

        public override void Hide()
        {
            base.Hide();

            _sliderBgm.onValueChanged.RemoveListener(OnBgmSliderValueChanged);
            _sliderSfx.onValueChanged.RemoveListener(OnSfxSliderValueChanged);
        }

        public void ReserveReturnCanvas(UnchordCanvas returnCanvas)
        {
            _reservedReturnCanvas = returnCanvas;
        }

        private void OnBackButtonClick()
        {
            this.Hide();

            Debug.Assert(_reservedReturnCanvas != null);

            _reservedReturnCanvas.Show();
        }

        private void OnBgmSliderValueChanged(float value)
        {
            SoundChannel channel = SoundManager.Instance.BGM;

            float volume = GetChannelVolume(_sliderBgm);

            // NOTE: 코드 순서 바뀌지 않도록 주의.
            channel.Volume = volume;
            channel.IsMuted = false;

            _txtBgm.text = GetVolumeString(_sliderBgm.value);
        }

        private void OnSfxSliderValueChanged(float value)
        {
            SoundChannel channel = SoundManager.Instance.SFX;

            float volume = value / _sliderSfx.maxValue;

            // NOTE: 코드 순서 바뀌지 않도록 주의.
            channel.Volume = volume;
            channel.IsMuted = false;

            _txtSfx.text = GetVolumeString(_sliderSfx.value);
        }

        private void OnBgmMuteButtonClick()
        {
            SoundChannel channel = SoundManager.Instance.BGM;

            _sliderBgm.onValueChanged.RemoveListener(OnBgmSliderValueChanged);
            ToggleMute(channel, _sliderBgm, _txtBgm);
            _sliderBgm.onValueChanged.AddListener(OnBgmSliderValueChanged);
        }

        private void OnSfxMuteButtonClick()
        {
            SoundChannel channel = SoundManager.Instance.SFX;

            _sliderSfx.onValueChanged.RemoveListener(OnSfxSliderValueChanged);
            ToggleMute(channel, _sliderSfx, _txtSfx);
            _sliderSfx.onValueChanged.AddListener(OnSfxSliderValueChanged);
        }

        private void ToggleMute(SoundChannel channel, Slider slider, TextMeshProUGUI textComponent)
        {
            channel.IsMuted ^= true;

            float sliderVolume = GetSliderVolume(channel, slider);
            slider.value = sliderVolume;
            textComponent.text = GetVolumeString(sliderVolume);
        }

        private string GetVolumeString(float sliderVolume)
        {
            return $"{sliderVolume}";
        }

        private float GetSliderVolume(SoundChannel channel, Slider slider)
        {
            float channelVolume = channel.Volume;

            if (!channel.IsMuted && channel.IsPaused)
                channelVolume = channel.BufferedVolume;

            float range = slider.maxValue - slider.minValue;

            return Mathf.RoundToInt(slider.minValue + channelVolume * range);
        }

        private float GetChannelVolume(Slider slider)
        {
            float range = slider.maxValue - slider.minValue;
            float value = slider.value - slider.minValue;

            return value / range;
        }
    }
}