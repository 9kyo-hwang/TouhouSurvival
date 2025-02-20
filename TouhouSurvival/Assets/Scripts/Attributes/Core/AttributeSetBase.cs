using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttributeSetBase<T> : MonoBehaviour where T : Enum
{
    [SerializeField] private List<GameplayAttributeData<T>> initialAttributes = new();
    
    protected readonly Dictionary<T, GameplayAttribute> Attributes = new();

    protected virtual void Awake()
    {
        // 인스펙터에서 설정된 초기 속성들을 딕셔너리에 추가
        foreach (GameplayAttributeData<T> data in initialAttributes)
        {
            InitializeAttribute(data.attributeType, data.baseValue, data.minValue, data.maxValue);
        }
    }

    public void InitializeAttribute(T attributeType, float baseValue, float minValue = float.MinValue, float maxValue = float.MaxValue)
    {
        if (!Attributes.ContainsKey(attributeType))
        {
            Attributes[attributeType] = new GameplayAttribute(baseValue, minValue, maxValue);
        }
    }

    public GameplayAttribute GetAttribute(T attributeType)
    {
        if (Attributes.TryGetValue(attributeType, out GameplayAttribute attribute))
        {
            return attribute;
        }
        
        Debug.LogWarning($"Attribute {attributeType} not found!");
        return null;
    }

    public float GetCurrentValue(T attributeType)
    {
        GameplayAttribute attribute = GetAttribute(attributeType);
        return attribute?.CurrentValue ?? 0f;
    }

    public void SetCurrentValue(T attributeType, float value)
    {
        GameplayAttribute attribute = GetAttribute(attributeType);
        if (attribute != null)
        {
            attribute.CurrentValue = value;
        }
    }
    
    public void SetBaseValue(T attributeType, float value)
    {
        GameplayAttribute attribute = GetAttribute(attributeType);
        if (attribute != null)
        {
            attribute.BaseValue = value;
        }
    }

    public void ModifyCurrentValue(T attributeType, float delta)
    {
        GameplayAttribute attribute = GetAttribute(attributeType);
        if (attribute != null)
        {
            attribute.CurrentValue += delta;
        }
    }

    public void ModifyBaseValue(T attributeType, float delta)
    {
        GameplayAttribute attribute = GetAttribute(attributeType);
        if (attribute != null)
        {
            attribute.BaseValue += delta;
        }
    }
}
