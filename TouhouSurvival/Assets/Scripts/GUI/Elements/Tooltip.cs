using TMPro;
using UnityEngine;

namespace Unchord
{
    public class Tooltip : MonoBehaviour
    {
        public float maxWidth = 540.0f;
        public float margin = 8.0f;

        private RectTransform _background;
        private Vector2 _pivot;
        private Vector2 _offset;
        private TextMeshProUGUI _description;
        
        private void Start()
        {
            _background = transform.GetChild(0).GetComponent<RectTransform>();
            _pivot = _background.pivot;
            _offset = Vector2.zero;
            _description = _background.GetChild(0).GetComponent<TextMeshProUGUI>();

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

        public void SetDescription(string description)
        {
            _description.text = description;
        }

        public void Show(Vector2 position)
        {
            _background.position = _offset + position;
            _background.pivot = _pivot;

            gameObject.SetActive(true);
            FitBoxSize();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void FitBoxSize()
        {
            // NOTE: TextMeshProUGUI의 preferredWidth(이하 w) 및 preferredHeight(이하 h)의 동작 원리
            // 1. _description 및 _description.rectTransform에 변동 사항이 생기면 TextMeshProUGUI 컴포넌트가 알아서 w 및 h 값을 계산해줌.
            // 2. 계산된 w 및 h 값은 다음 의미를 가짐;
            //      - RectTransform 컴포넌트의 sizeDelta.x를 w로 변경하거나 sizeDelta.y를 h로 변경해야 한다.
            //      - 둘 중 하나만 수행하더라도 Rect 영역 내에 모든 글자가 들어오는 경우가 있음.
            //      - 둘 다 수행해야만 Rect 영역 내에 모든 글자가 들어오는 경우가 있음.
            //      - 결론적으로, 한 번의 w, h 값 계산마다 하나의 축 길이를 확정해야만 완벽하게 구현 가능하다.
            // 3. 따라서, 텍스트를 포함하는 사각형의 크기를 계산하기 위해 아래 알고리즘을 수행함;
            //      - (w, h) 정의
            //      - 보여주고 싶은 최대 가로 길이를 정의해서 w와 비교 후 w 값을 확정
            //      - _description.rectTransform.sizeDelta에 w를 정리한 크기를 대입하면 TextMeshProUGUI는 새로운 w, h를 계산함.
            //      - 툴팁 박스의 높이는 새로 계산된 h로 확정
            //      - _description.rectTransform.sizeDelta에 다시 대입하면 구현 완료.

            Vector2 size = new Vector2(_description.preferredWidth, _description.preferredHeight);
            size.x = Mathf.Min(maxWidth, size.x);
            _description.rectTransform.sizeDelta = size;
            size.y = _description.preferredHeight;
            _description.rectTransform.sizeDelta = size;
            _background.sizeDelta = size + 2.0f * margin * Vector2.one;
        }
    }
}