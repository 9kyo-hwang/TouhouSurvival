using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Unchord
{
    public class SelectionButtonSpecial : MonoBehaviour
    , IPointerEnterHandler
    , IPointerExitHandler
    , IPointerMoveHandler
    {
        public int SelectionIndex { get; private set; }

        private Button _button;
        private Image _lock;
        private Tooltip _tooltip;
        private string _description;
        private Action<SelectionButtonSpecial> _onButtonClicked;

        private Color _clrLock;
        private Color _clrSelectable;
        private Color _clrSelected;

        private Vector2 _tooltipPivot;
        private Vector2 _tooltipOffset;

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
            _tooltip = transform.GetComponentInChildren<Tooltip>(true);
            _onButtonClicked = onButtonClicked;

            _clrLock = new Color(0.75f, 0.75f, 0.75f, 1.0f);
            _clrSelectable = Color.white;
            _clrSelected = new Color(0.375f, 0.375f, 0.375f, 1.0f);

            SelectionIndex = index;

            SetState(ButtonState.Lock);

            _button.onClick.AddListener(OnButtonClicked);
        }

        public void SetIcon(Sprite icon)
        {
            UnityEngine.Debug.Assert(icon != null);

            _button.image.sprite = icon;
        }

        public void SetDescription(string description)
        {
            UnityEngine.Debug.Assert(description != null);

            _description = description;
        }

        public void SetTooltipPivot(Vector2 pivot)
        {
            _tooltipPivot = pivot;
        }

        public void SetTooltipOffset(Vector2 offset)
        {
            _tooltipOffset = offset;
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

        public void OnPointerEnter(PointerEventData eventData)
        {
            _tooltip.SetPivot(_tooltipPivot);
            _tooltip.SetOffset(_tooltipOffset);
            _tooltip.Show(_description, Input.mousePosition);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _tooltip.Hide();
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            _tooltip.SetPivot(_tooltipPivot);
            _tooltip.SetOffset(_tooltipOffset);
            _tooltip.Show(_description, Input.mousePosition);
        }

        private void OnButtonClicked()
        {
            if (_btnState != ButtonState.Selectable)
                return;

            _onButtonClicked?.Invoke(this);
            _tooltip.Hide();
        }
    }
}