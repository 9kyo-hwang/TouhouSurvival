using System;
using System.Collections.Generic;
using UnityEngine;

public class AttributeSet : MonoBehaviour
{
    [SerializeField] protected List<AttributeData> attributes = new List<AttributeData>();

    private readonly Dictionary<string, int> _attributeIndexMap = new Dictionary<string, int>();
    public event Action<string, float, float> OnAttributeChanged;   // attribute name, old value, new value

    protected virtual void Awake()
    {
        for (int i = 0; i < attributes.Count; i++)
        {
            _attributeIndexMap.Add(attributes[i].attributeName, i);
        }
    }

    public float GetAttribute(string attributeName)
    {
        if (_attributeIndexMap.TryGetValue(attributeName, out int index))
        {
            return attributes[index].currentValue;
        }
        
        Debug.LogWarning($"Attribute {attributeName} not found");
        return 0f;
    }
    
    /**
     * 기존 currentValue에 delta값 만큼의 변화를 주고 싶을 떄 호출
     */
    public void ModifyAttribute(string attributeName, float delta)
    {
        if (_attributeIndexMap.TryGetValue(attributeName, out int index))
        {
            AttributeData attribute = attributes[index];
            float oldValue = attribute.currentValue;
            
            attribute.currentValue = Mathf.Clamp(attribute.currentValue + delta, attribute.minValue, attribute.maxValue);
            attributes[index] = attribute;
            OnAttributeChanged?.Invoke(attribute.attributeName, oldValue, attribute.currentValue);
        }
    }

    public void ModifyAttributeBase(string attributeName, float delta)
    {
        if (_attributeIndexMap.TryGetValue(attributeName, out int index))
        {
            AttributeData attribute = attributes[index];
            float oldValue = attribute.baseValue;
            
            attribute.baseValue = Mathf.Clamp(attribute.baseValue + delta, attribute.minValue, attribute.maxValue);
            //attribute.currentValue = attribute.baseValue;
            attributes[index] = attribute;
            OnAttributeChanged?.Invoke(attribute.attributeName, oldValue, attribute.baseValue);
            Debug.Log($"ModifyAttributeBase: {attributeName}, {oldValue} -> {attribute.baseValue}");
        }
    }

    /**
     * 기존 currentValue를 value로 고정시키고 싶을 때 호출
     */
    public void SetAttribute(string attributeName, float value)
    {
        if (_attributeIndexMap.TryGetValue(attributeName, out int index))
        {
            AttributeData attribute = attributes[index];
            float oldValue = attribute.currentValue;
            
            attribute.currentValue = Mathf.Clamp(value, attribute.minValue, attribute.maxValue);
            attributes[index] = attribute;
            OnAttributeChanged?.Invoke(attribute.attributeName, oldValue, attribute.currentValue);
        }
    }
}
