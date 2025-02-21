using UnityEngine;
using UnityEngine.UI;

namespace Unchord
{
    public class ImageHsvVibrator : MonoBehaviour
    {
        [SerializeField]
        [Range(0.01f, 100.0f)]
        private float _satPeriod = 5.0f;

        [SerializeField]
        [Range(0.01f, 1.0f)]
        private float _satAmplitude = 0.5f;

        [SerializeField]
        [Range(0.01f, 100.0f)]
        private float _huePeriod = 15.0f;

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float _phase = 0.0f;

        private Image img;
        private float _sTime;
        private float _hTime;

        private void Awake()
        {
            img = GetComponent<Image>();
        }

        private void Update()
        {
            float absSatPeriod = Mathf.Abs(_satPeriod);
            float absHuePeriod = Mathf.Abs(_huePeriod);

            if (absSatPeriod > 0.0f && absHuePeriod > 0.0f)
            {
                _sTime = (_sTime + Time.deltaTime) % absSatPeriod;
                _hTime = (_hTime + Time.deltaTime) % absHuePeriod;

                float h = _hTime / absHuePeriod;
                float s = _sTime / absSatPeriod;
                s = (TriangleSin(_satAmplitude, _phase, s) + _satAmplitude) * 0.5f;
                img.color = Color.HSVToRGB(h, s, 1.0f);
            }
            else
            {
                _sTime = 0.0f;
                _hTime = 0.0f;
                img.color = Color.white;
            }
        }

        private float TriangleSin(float amplitude, float phase, float t)
        {
            UnityEngine.Debug.Assert(phase >= 0.0f && phase <= 1.0f, "Please input normalized phase.");
            UnityEngine.Debug.Assert(t >= 0.0f && t <= 1.0f, "Please input normalized time.");

            if (t < 0.25f)
                return 4.0f * amplitude * t;
            else if (t < 0.75f)
                return 2.0f * amplitude * (1.0f - 2.0f * t);
            else
                return 4.0f * amplitude * (t - 1.0f);
        }
    }
}