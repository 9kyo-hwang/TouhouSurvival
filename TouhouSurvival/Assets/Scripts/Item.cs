using System;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public ItemData data;
    public int level;
    public Weapon weapon;

    private Image _icon;
    private Text _level;
    private Text _name;
    private Text _desc;
    
    private void Awake()
    {
        _icon = GetComponentsInChildren<Image>()[1];  // 0번은 자기자신
        _icon.sprite = data.itemIcon;
        
        Text[] texts = GetComponentsInChildren<Text>();
        _level = texts[0];
        _name = texts[1];
        _desc = texts[2];
        _name.text = data.itemName;
    }

    private void OnEnable()
    {
        _level.text = "Lv." + (level + 1);

        _desc.text = data.itemType switch
        {
            ItemData.ItemType.Melee => string.Format(data.itemDesc, data.damages[level] * 100, data.counts[level]),
            ItemData.ItemType.Range => string.Format(data.itemDesc, data.damages[level] * 100, data.penetrations[level]),
            ItemData.ItemType.Glove or ItemData.ItemType.Shoe => string.Format(data.itemDesc, data.damages[level] * 100),
            _ => data.itemDesc
        };
    }

    public void OnClick()
    {
        switch (data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                if (level == 0)
                {
                    GameObject newWeapon = new GameObject();
                    weapon = newWeapon.AddComponent<Weapon>();
                    weapon.Initialize(data);
                }
                else
                {
                    float nextDamage = data.baseDamage + data.baseDamage * data.damages[level];
                    int nextCount = data.counts[level];
                    int nextPenetration = data.penetrations[level];
                    
                    weapon.LevelUp(nextDamage, nextCount, nextPenetration);
                }
                break;
            case ItemData.ItemType.Glove:
                break;
            case ItemData.ItemType.Shoe:
                break;
            case ItemData.ItemType.Potion:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        ++level;
        if (level == data.damages.Length)
        {
            GetComponent<Button>().interactable = false;
        }
    }
}
