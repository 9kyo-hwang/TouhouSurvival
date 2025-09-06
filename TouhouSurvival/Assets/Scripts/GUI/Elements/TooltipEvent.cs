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

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            tooltip.SetDescription(description);
            tooltip.SetPivot(pivot);
            tooltip.SetOffset(offset);
            tooltip.Show(Input.mousePosition);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            tooltip.Hide();
        }

        void IPointerMoveHandler.OnPointerMove(PointerEventData eventData)
        {
            tooltip.SetPivot(pivot);
            tooltip.SetOffset(offset);
            tooltip.Show(Input.mousePosition);
        }

        public void SetDescription(string description)
        {
            UnityEngine.Debug.Assert(description != null);

            this.description = description;
        }

        public void SetTooltipPivot(Vector2 pivot)
        {
            this.pivot = pivot;
        }

        public void SetTooltipOffset(Vector2 offset)
        {
            this.offset = offset;
        }
    }
}