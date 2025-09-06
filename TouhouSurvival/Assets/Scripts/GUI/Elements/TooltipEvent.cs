using UnityEngine;
using UnityEngine.EventSystems;

namespace Unchord
{
    public class TooltipEvent : MonoBehaviour
    , IPointerEnterHandler
    , IPointerExitHandler
    , IPointerMoveHandler
    {
        private Tooltip _tooltip;
        private string _description;
        private Vector2 _pivot;
        private Vector2 _offset;

        private void Awake()
        {
            _tooltip = transform.parent.GetComponentInChildren<Tooltip>();
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            _tooltip.SetDescription(_description);
            _tooltip.SetPivot(_pivot);
            _tooltip.SetOffset(_offset);
            _tooltip.Show(Input.mousePosition);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            _tooltip.Hide();
        }

        void IPointerMoveHandler.OnPointerMove(PointerEventData eventData)
        {
            _tooltip.SetPivot(_pivot);
            _tooltip.SetOffset(_offset);
            _tooltip.Show(Input.mousePosition);
        }

        public void SetDescription(string description)
        {
            UnityEngine.Debug.Assert(description != null);

            _description = description;
        }

        public void SetTooltipPivot(Vector2 pivot)
        {
            _pivot = pivot;
        }

        public void SetTooltipOffset(Vector2 offset)
        {
            _offset = offset;
        }
    }
}