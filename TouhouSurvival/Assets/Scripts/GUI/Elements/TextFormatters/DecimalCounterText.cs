using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Unchord
{
    public class DecimalCounterText : MonoBehaviour
    {
        public const int MAX_DIGIT_COUNT = 10;

        [Range(1, MAX_DIGIT_COUNT)]
        public int digit = 6;
        public Color clrEnabled = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        public Color clrDisabled = new Color(0.3f, 0.3f, 0.3f, 1.0f);

        private char[] _buffer;
        private StringBuilder _builder;

        private TextMeshProUGUI _text;
        private int _value;

        private void Awake()
        {
            _buffer = new char[MAX_DIGIT_COUNT];
            _builder = new StringBuilder(64);

            _text = GetComponent<TextMeshProUGUI>();
            _value = 0;
        }

        private void Start()
        {
            _text.text = ToRichText(_value, digit, clrEnabled, clrDisabled);
        }

        public void SetValue(int value)
        {
            if (value == _value)
                return;

            _value = value;
            _text.text = ToRichText(_value, digit, clrEnabled, clrDisabled);
        }

        private string ToRichText(int value, int digitLength, Color enabled, Color disabled)
        {
            UnityEngine.Debug.Assert(1 <= digitLength && digitLength <= MAX_DIGIT_COUNT);

            int i = 0;

            for (i = 0; i < MAX_DIGIT_COUNT && value > 0; ++i)
            {
                _buffer[i] = (char)('0' + value % 10);
                value /= 10;
            }

            if (i == 0)
            {
                _buffer[i++] = '0';
            }

            _builder.Clear();
            _builder.Append($"<color=#{disabled.ToHexString()}>");

            for (int j = digitLength - 1; j >= i; --j)
            {
                _builder.Append('0');
            }

            _builder.Append("</color>");
            _builder.Append($"<color=#{enabled.ToHexString()}>");

            for (int j = i - 1; j >= 0; --j)
            {
                _builder.Append(_buffer[j]);
            }

            _builder.Append("</color>");

            return _builder.ToString();
        }
    }
}