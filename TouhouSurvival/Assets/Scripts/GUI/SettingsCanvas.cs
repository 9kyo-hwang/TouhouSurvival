using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unchord
{
    // NOTE:
    // 일시적으로 아래 기능을 사용 중지합니다.
    //      1. Mute 기능
    //      2. GUI에 볼륨 값을 표시하는 기능
    // 필요에 따라 언제든 코드를 복구하여 사용할 수 있도록 주석 처리된 코드를 다수 포함하고 있습니다.
    public class SettingsCanvas : UnchordCanvas
    {
        private Button _btnBack;
        private Slider _sliderBgm;
        private Slider _sliderSfx;
        //private Button _btnBgmMute;
        //private Button _btnSfxMute;
        //private TextMeshProUGUI _volLabelBgm;
        //private TextMeshProUGUI _volLabelSfx;

        private UnchordCanvas _reservedReturnCanvas;

        protected override void Awake()
        {
            _btnBack = transform.Find("SettingsPanel/BackButton").GetComponent<Button>();
            _sliderBgm = transform.Find("SettingsPanel/BgmSlider").GetComponent<Slider>();
            _sliderSfx = transform.Find("SettingsPanel/SfxSlider").GetComponent<Slider>();
            //_btnBgmMute = transform.Find("SettingsPanel/BgmMuteButton").GetComponent<Button>();
            //_btnSfxMute = transform.Find("SettingsPanel/SfxMuteButton").GetComponent<Button>();
            //_volLabelBgm = _sliderBgm.transform.Find("VolumeLabel/ContentText").GetComponent<TextMeshProUGUI>();
            //_volLabelSfx = _sliderSfx.transform.Find("VolumeLabel/ContentText").GetComponent<TextMeshProUGUI>();

            _btnBack.onClick.AddListener(OnBackButtonClick);
            //_btnBgmMute.onClick.AddListener(OnBgmMuteButtonClick);
            //_btnSfxMute.onClick.AddListener(OnSfxMuteButtonClick);
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

            //_volLabelBgm.text = GetVolumeString(_sliderBgm.value);
            //_volLabelSfx.text = GetVolumeString(_sliderSfx.value);
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

            //_volLabelBgm.text = GetVolumeString(_sliderBgm.value);
        }

        private void OnSfxSliderValueChanged(float value)
        {
            SoundChannel channel = SoundManager.Instance.SFX;

            float volume = value / _sliderSfx.maxValue;

            // NOTE: 코드 순서 바뀌지 않도록 주의.
            channel.Volume = volume;
            channel.IsMuted = false;

            //_volLabelSfx.text = GetVolumeString(_sliderSfx.value);
        }

        private void OnBgmMuteButtonClick()
        {
            SoundChannel channel = SoundManager.Instance.BGM;

            _sliderBgm.onValueChanged.RemoveListener(OnBgmSliderValueChanged);
            //ToggleMute(channel, _sliderBgm, _volLabelBgm);
            _sliderBgm.onValueChanged.AddListener(OnBgmSliderValueChanged);
        }

        private void OnSfxMuteButtonClick()
        {
            SoundChannel channel = SoundManager.Instance.SFX;

            _sliderSfx.onValueChanged.RemoveListener(OnSfxSliderValueChanged);
            //ToggleMute(channel, _sliderSfx, _volLabelSfx);
            _sliderSfx.onValueChanged.AddListener(OnSfxSliderValueChanged);
        }

        //private void ToggleMute(SoundChannel channel, Slider slider, TextMeshProUGUI textComponent)
        //{
        //    channel.IsMuted ^= true;

        //    float sliderVolume = GetSliderVolume(channel, slider);
        //    slider.value = sliderVolume;
        //    textComponent.text = GetVolumeString(sliderVolume);
        //}

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