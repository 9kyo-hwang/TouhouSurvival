using UnityEngine;
using UnityEngine.UI;

namespace Unchord
{
    public class Tooltip : MonoBehaviour
    {
        private RectTransform _background;
        private Text _description;

        private void Start()
        {
            _background = transform.GetChild(0).GetComponent<RectTransform>();
            _description = _background.GetChild(0).GetComponent<Text>();

            gameObject.SetActive(false);
        }

        public void Show(string description, Vector2 position)
        {
            _description.text = description;
            _background.position = position;
            
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}

