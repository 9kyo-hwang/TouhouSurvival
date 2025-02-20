using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class GameplayAttributeData<T> where T : Enum
{
    [Header("어트리뷰트 설정")] 
    public T attributeType;
    public float baseValue;
    public float minValue = float.MinValue;
    public float maxValue = float.MaxValue;
}