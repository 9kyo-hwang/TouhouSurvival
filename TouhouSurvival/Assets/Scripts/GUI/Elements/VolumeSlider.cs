using UnityEngine;
using UnityEngine.UI;

namespace Unchord
{
    public class VolumeSlider
    {
        private const int c_BUTTON_COUNT = 10;

        private SoundChannel _targetChannel;

        private Button[] _btnVolumes;
        private Button _btnMute;
        private Button _btnFull;

        private Color _clrEnabled;
        private Color _clrDisabled0;
        private Color _clrDisabled1;

        public VolumeSlider(Transform sliderRoot, SoundChannel channel)
        {
            _targetChannel = channel;

            _btnVolumes = new Button[10];

            for (int i = 0; i < c_BUTTON_COUNT; ++i)
            {
                Transform btnRoot = sliderRoot.Find($"Frame/VolumeButtons/Button ({i + 1})");
                int level = i + 1;

                _btnVolumes[i] = btnRoot.GetComponent<Button>();
                _btnVolumes[i].onClick.AddListener(() =>
                {
                    float v = (float)level / c_BUTTON_COUNT;

                    _targetChannel.Volume = v;
                    UpdateButtons(level);
                });
            }

            _btnMute = sliderRoot.Find("Frame/MuteButton").GetComponent<Button>();
            _btnFull = sliderRoot.Find("Frame/FullButton").GetComponent<Button>();

            _btnMute.onClick.AddListener(() =>
            {
                _targetChannel.Volume = 0.0f;
                UpdateButtons(0);
            });
            _btnFull.onClick.AddListener(() =>
            {
                _targetChannel.Volume = 1.0f;
                UpdateButtons(c_BUTTON_COUNT);
            });

            _clrEnabled = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            _clrDisabled0 = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            _clrDisabled1 = new Color(0.4f, 0.4f, 0.4f, 1.0f);
        }

        public void OnShow()
        {
            float v = _targetChannel.Volume;
            int level = (int)(v * c_BUTTON_COUNT);
            UpdateButtons(level);
        }

        private void UpdateButtons(int currentVolumeLevel)
        {
            for (int i = 0; i < c_BUTTON_COUNT; ++i)
            {
                _btnVolumes[i].image.color = (i < currentVolumeLevel) ? _clrEnabled : _clrDisabled0;
            }

            _btnMute.image.color = (currentVolumeLevel == 0) ? _clrEnabled : _clrDisabled1;
            _btnFull.image.color = (currentVolumeLevel == c_BUTTON_COUNT) ? _clrEnabled : _clrDisabled1;
        }
    }
}