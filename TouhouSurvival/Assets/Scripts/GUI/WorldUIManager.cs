using UnityEngine;
using UnityEngine.UI;

namespace Unchord
{
    public class WorldUIManager : UIManagerBase<WorldUIManager>
    {
        private RectTransform _playerHealthTransform;
        private Image _playerHealthImage;

        protected override void Awake()
        {
            base.Awake();

            _playerHealthTransform = base.GetComponentFromTable<RectTransform>("GUIs/Element/World/PlayerHealth", this.transform, false);
            _playerHealthImage = _playerHealthTransform.Find("Gauge").GetComponent<Image>();
        }

        public void ShowPlayerHealth()
        {
            _playerHealthTransform.gameObject.SetActive(true);
        }

        public void HidePlayerHealth()
        {
            _playerHealthTransform.gameObject.SetActive(false);
        }

        public void SetPlayerHealthPosition(Vector2 position)
        {
            float z = _playerHealthTransform.position.z;
            _playerHealthTransform.position = new Vector3(position.x, position.y, z);
        }

        public void SetPlayerHealthValue(float value, float max)
        {
            _playerHealthImage.fillAmount = Mathf.Clamp01(value / max);
        }
    }
}