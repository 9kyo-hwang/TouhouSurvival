using TMPro;
using UnityEngine;

namespace Unchord
{
    public class TimerText : MonoBehaviour
    {
        public bool useHours = false;

        private TextMeshProUGUI _text;
        private int _intTime;

        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
            _intTime = 0;
        }

        private void Start()
        {
            _text.text = ToRichText(_intTime);
        }

        public void SetValue(int timeSeconds)
        {
            UnityEngine.Debug.Assert(timeSeconds >= 0);

            if (_intTime == timeSeconds)
                return;

            _intTime = timeSeconds;
            _text.text = ToRichText(_intTime);
        }

        public void SetValue(float timeSeconds)
        {
            UnityEngine.Debug.Assert(timeSeconds >= 0);

            int intTime = Mathf.FloorToInt(timeSeconds);
            SetValue(intTime);
        }

        private string ToRichText(int timeSeconds)
        {
            int sec = timeSeconds % 60;
            int min = (timeSeconds / 60) % 60;
            int hrs = timeSeconds / 3600;

            if (useHours && hrs > 0)
                return string.Format("{0:D02}:{1:D02}:{2:D02}", hrs, min, sec);
            else
                return string.Format("{0:D02}:{1:D02}", min, sec);
        }
    }
}