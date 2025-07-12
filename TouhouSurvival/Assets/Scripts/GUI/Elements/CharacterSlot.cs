using UnityEngine;
using UnityEngine.UI;

namespace Unchord
{
    public class CharacterSlot
    {
        private Image _characterIcon;
        private Image _weaponIcon;
        private Image _specialIcon;

        public CharacterSlot(Transform slot)
        {
            _characterIcon = slot.Find("Icon (0)").GetComponent<Image>();
            _weaponIcon = slot.Find("Icon (1)").GetComponent<Image>();
            _specialIcon = slot.Find("Icon (2)").GetComponent<Image>();
        }

        public void SetIcons(Sprite character, Sprite mainWeapon, Sprite special)
        {
            _characterIcon.sprite = character;
            _weaponIcon.sprite = mainWeapon;
            _specialIcon.sprite = special;
        }
    }
}