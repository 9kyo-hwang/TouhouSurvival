using System;
using UnityEngine;

[Serializable]
public class GameplayAttributeData
{
    [Header("어트리뷰트 설정")] 
    public string attributeName;
    public float baseValue;
    public float minValue = float.MinValue;
    public float maxValue = float.MaxValue;
}