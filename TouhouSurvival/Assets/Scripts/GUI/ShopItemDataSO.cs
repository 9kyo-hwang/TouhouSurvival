using UnityEngine;

[CreateAssetMenu(fileName = "ShopItemDataSO", menuName = "Scriptable Objects/ShopItemDataSO")]
public class ShopItemDataSO : ScriptableObject
{
    public Sprite icon;
    public string title;
    public string xlsxPath;
}
