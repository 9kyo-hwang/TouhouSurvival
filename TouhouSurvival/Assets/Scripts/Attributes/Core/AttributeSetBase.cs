using System.Collections.Generic;
using UnityEngine;

public class AttributeSetBase : MonoBehaviour
{
    [SerializeField] private List<GameplayAttributeData> initialAttributes = new List<GameplayAttributeData>();
    
    protected readonly Dictionary<string, GameplayAttribute> Attributes = new Dictionary<string, GameplayAttribute>();

    protected virtual void Awake()
    {
        // 인스펙터에서 설정된 초기 속성들을 딕셔너리에 추가
        foreach (var data in initialAttributes)
        {
            if (!string.IsNullOrEmpty(data.attributeName))
            {
                InitializeAttribute(data.attributeName, data.baseValue, data.minValue, data.maxValue);
            }
        }
    }

    public void InitializeAttribute(string attributeName, float baseValue, float minValue = float.MinValue, float maxValue = float.MaxValue)
    {
        if (!Attributes.ContainsKey(attributeName))
        {
            Attributes[attributeName] = new GameplayAttribute(baseValue, minValue, maxValue);
        }
    }

    public GameplayAttribute GetAttribute(string attributeName)
    {
        if (Attributes.TryGetValue(attributeName, out GameplayAttribute attribute))
        {
            return attribute;
        }
        Debug.LogWarning($"Attribute {attributeName} not found!");
        return null;
    }

    public float GetAttributeValue(string attributeName)
    {
        var attribute = GetAttribute(attributeName);
        return attribute?.CurrentValue ?? 0f;
    }

    public void SetAttributeValue(string attributeName, float value)
    {
        var attribute = GetAttribute(attributeName);
        if (attribute != null)
        {
            attribute.CurrentValue = value;
        }
    }
    
    public void SetAttributeBaseValue(string attributeName, float value)
    {
        var attribute = GetAttribute(attributeName);
        if (attribute != null)
        {
            attribute.BaseValue = value;
        }
    }

    public void ModifyAttributeValue(string attributeName, float delta)
    {
        var attribute = GetAttribute(attributeName);
        if (attribute != null)
        {
            attribute.CurrentValue += delta;
        }
    }

    public void ModifyAttributeBaseValue(string attributeName, float delta)
    {
        var attribute = GetAttribute(attributeName);
        if (attribute != null)
        {
            attribute.BaseValue += delta;
        }
    }
}
