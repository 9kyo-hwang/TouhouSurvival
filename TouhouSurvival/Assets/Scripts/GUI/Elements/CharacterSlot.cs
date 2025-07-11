using UnityEngine;
using UnityEngine.UI;

namespace Unchord
{
    public class CharacterSlot
    {
        private Image _characterIcon;
        private Image _weaponIcon;
        private Image _spellIcon;

        public CharacterSlot(Transform slot)
        {
            _characterIcon = slot.Find("Icon (0)").GetComponent<Image>();
            _weaponIcon = slot.Find("Icon (1)").GetComponent<Image>();
            _spellIcon = slot.Find("Icon (2)").GetComponent<Image>();
        }

        public void SetIcons(Sprite character, Sprite mainWeapon, Sprite spell)
        {
            _characterIcon.sprite = character;
            _weaponIcon.sprite = mainWeapon;
            _spellIcon.sprite = spell;
        }
    }
}