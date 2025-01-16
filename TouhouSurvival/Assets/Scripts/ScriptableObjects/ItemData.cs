using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    public enum ItemType
    {
        Melee,
        Range,
        Glove,
        Shoe,
        Potion,
    }

    [Header("Main")]
    public ItemType itemType;
    public int itemId;
    [TextArea] public string itemName;
    [TextArea] public string itemDesc;
    public Sprite itemIcon;

    [Header("Level")] 
    public float baseDamage;
    public int baseCount;
    public int basePenetration;
    public float[] damages;
    public int[] counts;
    public int[] penetrations;

    [Header("Weapon")] 
    public GameObject projectile;
}
