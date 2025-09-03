using System.Collections.Generic;
using Unchord;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopItemDataSO", menuName = "Scriptable Objects/ShopItemDataSO")]
public class ShopItemDataSO : ScriptableObject
{
    public Sprite icon;
    public string title;
    public string attributeType;
    public string alias = string.Empty;    // unique id in master csv
    public List<SerializedGameplayAttributeModifier> modifiers;
}
