using UnityEngine;

namespace Unchord
{
    public class SettingsPanel : MonoBehaviour
    {
        private VolumeSlider _bgm;
        private VolumeSlider _sfx;

        private void Awake()
        {
            _bgm = new VolumeSlider(transform.Find("BgmSlider"), SoundManager.Instance.BGM);
            _sfx = new VolumeSlider(transform.Find("SfxSlider"), SoundManager.Instance.SFX);
        }

        private void OnEnable()
        {
            _bgm.OnShow();
            _sfx.OnShow();
        }
    }
}