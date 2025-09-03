using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopItemShelfSO", menuName = "Scriptable Objects/ShopItemShelfSO")]
public class ShopItemShelfSO : ScriptableObject
{
    public List<ShopItemDataSO> itemDataSOs;
}
