using UnityEngine;
using UnityEngine.UI;

namespace Unchord
{
    public class CharacterSlot : MonoBehaviour
    {
        private Image _characterIcon;
        private Image _weaponIcon;
        private Image _specialIcon;

        private TooltipEvent _characterTooltip;
        private TooltipEvent _weaponTooltip;
        private TooltipEvent _specialTooltip;

        private void Awake()
        {
            _characterIcon = transform.Find("Icon (0)").GetComponent<Image>();
            _weaponIcon = transform.Find("Icon (1)").GetComponent<Image>();
            _specialIcon = transform.Find("Icon (2)").GetComponent<Image>();

            _characterTooltip = _characterIcon.GetComponent<TooltipEvent>();
            _weaponTooltip = _weaponIcon.GetComponent<TooltipEvent>();
            _specialTooltip = _specialIcon.GetComponent<TooltipEvent>();
        }

        public void Show(Player player)
        {
            _characterIcon.sprite = player.iconCharacter;
            _weaponIcon.sprite = player.iconMainWeapon;
            _specialIcon.sprite = player.iconSpecial;

            _characterTooltip.description = player.name;
            _weaponTooltip.description = "Main Weapon Name Here.";
            _specialTooltip.description = "Special Ability Name Here.";
        }
    }
}