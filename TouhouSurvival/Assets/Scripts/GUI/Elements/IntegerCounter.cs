using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Unchord
{
    public class IntegerCounter : MonoBehaviour
    {
        public Vector2 offset;
        public IntegerCounter.Alignment alignment = Alignment.Left;
        public int scaleFactor = 2;
        public int spacing;
        public int digitCount = 2;
        public bool usePadding = true;

        [SerializeField]
        private IntegerCounterFontSO _fontAsset;

        private List<Digit> _digitPool;
        private int _value;
        private int _nonPaddedDigitCount;

        public enum Alignment
        {
            Left, Right, Center
        }

        private class Digit
        {
            public RectTransform rTransform { get; private set; }
            public Image imageComponent { get; private set; }
            public int splitDigit;

            public Digit(Transform parent, int index)
            {
                string imgName = $"D{index}";

                rTransform = parent.Find(imgName).GetComponent<RectTransform>();
                imageComponent = rTransform.GetComponent<Image>();
                splitDigit = 0;
            }

            public void Enable()
            {
                rTransform.gameObject.SetActive(true);
            }

            public void Disable()
            {
                rTransform.gameObject.SetActive(false);
            }
        }

        private void Awake()
        {
            _digitPool = new List<Digit>(digitCount);

            RefreshDigitPool();
            SetValue(0);
        }

        public void SetValue(int value)
        {
            UnityEngine.Debug.Assert(value >= 0);

            _value = value;

            SplitValue(value);

            switch (alignment)
            {
                case Alignment.Left:
                    RenderAsLeftAlignment();
                    break;
                case Alignment.Right:
                    RenderAsRightAlignment();
                    break;
                case Alignment.Center:
                    throw new NotImplementedException();
                default:
                    Debug.Assert(false, "Invalid case occurred.");
                    break;
            }
        }

        private void RefreshDigitPool()
        {
            for (int i = _digitPool.Count; i < digitCount; ++i)
            {
                _digitPool.Add(new Digit(this.transform, i));
            }

            for (int i = 0; i < digitCount; ++i)
            {
                _digitPool[i].Enable();
            }

            for (int i = digitCount; i < _digitPool.Count; ++i)
            {
                _digitPool[i].Disable();
            }
        }

        private void SplitValue(int value)
        {
            int digitBase = _fontAsset.digitSprites.Length;

            _nonPaddedDigitCount = 0;

            for (int i = 0; i < _digitPool.Count; ++i)
            {
                _digitPool[i].splitDigit = value % digitBase;
                value /= digitBase;

                if (_nonPaddedDigitCount == 0 && value == 0)
                {
                    _nonPaddedDigitCount = i + 1;
                }
            }
        }

        private void RenderAsLeftAlignment()
        {
            int renderingDigitCount = usePadding ? digitCount : _nonPaddedDigitCount;

            Vector2 position = offset;
            Vector2 sizeBuffer = _fontAsset.digitSprites[_digitPool[renderingDigitCount - 1].splitDigit].rect.size;

            for (int i = renderingDigitCount - 1; i >= 0 ; --i)
            {
                int d = _digitPool[i].splitDigit;
                Vector2 size = sizeBuffer;

                _digitPool[i].rTransform.anchoredPosition = position;
                _digitPool[i].rTransform.sizeDelta = scaleFactor * size;
                _digitPool[i].imageComponent.sprite = _fontAsset.digitSprites[d];
                _digitPool[i].Enable();

                if (i == 0)
                    break;

                sizeBuffer += _fontAsset.digitSprites[_digitPool[i - 1].splitDigit].rect.size;
                position.x += scaleFactor * (spacing + sizeBuffer.x * 0.5f);
                sizeBuffer -= size;
            }

            for (int i = renderingDigitCount; i < digitCount; ++i)
            {
                _digitPool[i].Disable();
            }
        }

        private void RenderAsRightAlignment()
        {
            int renderingDigitCount = usePadding ? digitCount : _nonPaddedDigitCount;

            Vector2 position = offset;
            Vector2 sizeBuffer = _fontAsset.digitSprites[_digitPool[0].splitDigit].rect.size;

            for (int i = 0; i < renderingDigitCount; ++i)
            {
                int d = _digitPool[i].splitDigit;
                Vector2 size = sizeBuffer;

                _digitPool[i].rTransform.anchoredPosition = position;
                _digitPool[i].rTransform.sizeDelta = scaleFactor * size;
                _digitPool[i].imageComponent.sprite = _fontAsset.digitSprites[d];
                _digitPool[i].Enable();

                if (i == renderingDigitCount - 1)
                    break;

                sizeBuffer += _fontAsset.digitSprites[_digitPool[i + 1].splitDigit].rect.size;
                position.x -= scaleFactor * (spacing + sizeBuffer.x * 0.5f);
                sizeBuffer -= size;
            }

            for (int i = renderingDigitCount; i < digitCount; ++i)
            {
                _digitPool[i].Disable();
            }
        }
    }
}