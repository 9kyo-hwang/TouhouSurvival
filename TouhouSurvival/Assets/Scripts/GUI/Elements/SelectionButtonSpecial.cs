using System;
using UnityEngine;
using UnityEngine.UI;

namespace Unchord
{
    public class SelectionButtonSpecial : MonoBehaviour
    {
        public int SelectionIndex { get; private set; }
        public TooltipEvent TooltipEvent { get; private set; }

        private Button _button;
        private Image _lock;
        private Action<SelectionButtonSpecial> _onButtonClicked;

        private Color _clrLock;
        private Color _clrSelectable;
        private Color _clrSelected;

        private ButtonState _btnState;

        public enum ButtonState
        {
            // NOTE: 특수한 목적이 있어 개별 원소에 값을 할당했으니 절대 바꾸지 말 것.
            Lock = 0,
            Selectable = 2,
            Selected = 3
        }

        public void Init(Action<SelectionButtonSpecial> onButtonClicked, int index)
        {
            _button = transform.Find("Button").GetComponent<Button>();
            _lock = transform.Find("Lock Icon").GetComponent<Image>();
            _onButtonClicked = onButtonClicked;

            _clrLock = new Color(0.75f, 0.75f, 0.75f, 1.0f);
            _clrSelectable = Color.white;
            _clrSelected = new Color(0.375f, 0.375f, 0.375f, 1.0f);

            SelectionIndex = index;
            TooltipEvent = GetComponent<TooltipEvent>();

            SetState(ButtonState.Lock);

            _button.onClick.AddListener(OnButtonClicked);
        }

        public void SetIcon(Sprite icon)
        {
            UnityEngine.Debug.Assert(icon != null);

            _button.image.sprite = icon;
        }

        public void SetState(ButtonState state)
        {
            switch (state)
            {
                case ButtonState.Lock:
                    _lock.gameObject.SetActive(true);
                    _button.interactable = false;
                    _button.image.color = _clrLock;
                    break;
                case ButtonState.Selectable:
                    _lock.gameObject.SetActive(false);
                    _button.interactable = true;
                    _button.image.color = _clrSelectable;
                    break;
                case ButtonState.Selected:
                    _lock.gameObject.SetActive(false);
                    _button.interactable = false;
                    _button.image.color = _clrSelected;
                    break;
                default:
                    UnityEngine.Debug.Assert(false);
                    break;
            }

            _btnState = state;
        }

        private void OnButtonClicked()
        {
            if (_btnState != ButtonState.Selectable)
                return;

            _onButtonClicked?.Invoke(this);
        }
    }
}