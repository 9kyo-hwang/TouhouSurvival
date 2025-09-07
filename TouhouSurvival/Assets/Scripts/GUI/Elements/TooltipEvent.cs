using UnityEngine;
using UnityEngine.EventSystems;

namespace Unchord
{
    public class TooltipEvent : MonoBehaviour
    , IPointerEnterHandler
    , IPointerExitHandler
    , IPointerMoveHandler
    {
        public Tooltip tooltip;

        public string description;
        public Vector2 pivot;
        public Vector2 offset;
        public Vector2 position;

        private bool _pointerEntered;

        private void Start()
        {
            tooltip = TooltipManager.Instance.Tooltip;
        }

        private void OnDisable()
        {
            if (_pointerEntered)
            {
                _pointerEntered = false;
                tooltip.Hide();
            }
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            _pointerEntered = true;

            tooltip.SetDescription(description);
            tooltip.SetPivot(pivot);
            tooltip.SetOffset(offset);
            tooltip.Show(Input.mousePosition);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            _pointerEntered = false;
            tooltip.Hide();
        }

        void IPointerMoveHandler.OnPointerMove(PointerEventData eventData)
        {
            tooltip.SetPivot(pivot);
            tooltip.SetOffset(offset);
            tooltip.Show(Input.mousePosition);
        }
    }
}