using System;
using UnityEngine;

public class AttributeChangedEventArgs : EventArgs
{
    public float OldValue { get; private set; }
    public float NewValue { get; private set; }

    public AttributeChangedEventArgs(float oldValue, float newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}

[Serializable]
public class GameplayAttribute
{
    [SerializeField] private float baseValue;
    [SerializeField] private float currentValue;
    [SerializeField] private float minValue;
    [SerializeField] private float maxValue;

    public event EventHandler<AttributeChangedEventArgs> OnAttributeChanged;

    public float BaseValue
    {
        get => baseValue;
        set
        {
            float oldValue = baseValue;
            baseValue = value;
            currentValue = Mathf.Clamp(baseValue, minValue, maxValue);
            OnAttributeChanged?.Invoke(this, new AttributeChangedEventArgs(oldValue, currentValue));
        }
    }

    public float CurrentValue
    {
        get => currentValue;
        set
        {
            float oldValue = currentValue;
            currentValue = Mathf.Clamp(value, minValue, maxValue);
            OnAttributeChanged?.Invoke(this, new AttributeChangedEventArgs(oldValue, currentValue));
        }
    }

    public GameplayAttribute(float baseValue, float minValue = float.MinValue, float maxValue = float.MaxValue)
    {
        this.baseValue = baseValue;
        this.currentValue = baseValue;
        this.minValue = minValue;
        this.maxValue = maxValue;
    }

    public void ResetToBase()
    {
        CurrentValue = BaseValue;
    }
}
