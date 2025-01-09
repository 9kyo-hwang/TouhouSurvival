using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    public enum ItemType
    {
        Melee,
        Range,
        Potion,
    }

    [Header("Main")]
    public ItemType type;
    public int id;
    public string designation;
    public string description;
    public Sprite icon;

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
