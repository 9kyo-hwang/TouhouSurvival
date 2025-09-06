using UnityEngine;
using UnityEngine.UI;

namespace Unchord
{
    public class Tooltip : MonoBehaviour
    {
        private RectTransform _background;
        private Vector2 _pivot;
        private Vector2 _offset;
        private Text _description;

        private void Start()
        {
            _background = transform.GetChild(0).GetComponent<RectTransform>();
            _pivot = _background.pivot;
            _offset = Vector2.zero;
            _description = _background.GetChild(0).GetComponent<Text>();

            gameObject.SetActive(false);
        }

        public void SetPivot(Vector2 pivot)
        {
            _pivot = pivot;
        }

        public void SetOffset(Vector2 offset)
        {
            _offset = offset;
        }

        public void Show(string description, Vector2 position)
        {
            _description.text = description;
            _background.position = _offset + position;
            _background.pivot = _pivot;
            
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}

