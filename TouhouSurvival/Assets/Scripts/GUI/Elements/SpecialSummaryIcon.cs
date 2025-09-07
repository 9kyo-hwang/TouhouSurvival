using UnityEngine;
using UnityEngine.UI;

namespace Unchord
{
    public class SpecialSummaryIcon : MonoBehaviour
    {
        public TooltipEvent TooltipEvent { get; private set; }

        private Image _img;
        private Transform _lockIcon;

        private void Awake()
        {
            _img = GetComponent<Image>();
            _lockIcon = transform.Find("LockIcon");
            TooltipEvent = GetComponent<TooltipEvent>();
        }

        public void SetIcon(Sprite icon)
        {
            _img.sprite = icon;
        }

        public void SetLock(bool shouldLock)
        {
            _lockIcon.gameObject.SetActive(shouldLock);
            TooltipEvent.enabled = !shouldLock;
        }

        public void SetTooltipDescription(string description)
        {
            TooltipEvent.description = description;
        }

        public void Register(SpecialAbilityComponent special)
        {
            UnityEngine.Debug.Assert(special != null);

            SetIcon(special.DisplayIcon);
            SetLock(special.CurrentLevel == 0);
            SetTooltipDescription($"{special.DisplayName}\n\n{special.DisplayDescription}");
        }
    }
}