using System;
using System.Collections.Generic;
using Unchord;
using UnityEngine;

public abstract class AttributeSetBase<T> : MonoBehaviour where T : Enum
{
    public float Level { get; private set; } = 1;
    public float Experience { get; private set; } = 0;

    [SerializeField] private List<GameplayAttributeData<T>> initialAttributes = new();
    
    protected readonly Dictionary<T, GameplayAttribute> Attributes = new();

    public LevelUpData<T>[] levelUpData;

    public event Action<int, float, float> onExpChanged;
    public event Action<int, float, float> onLevelUp;

    public GameplayAttribute this[T attributeType]
    {
        get => Attributes[attributeType];
    }

    protected virtual void Awake()
    {
        // 인스펙터에서 설정된 초기 속성들을 딕셔너리에 추가
        foreach (GameplayAttributeData<T> data in initialAttributes)
        {
            InitializeAttribute(data.attributeType, data.baseValue, data.minValue, data.maxValue);
        }

        CheckAllAttributeDefined();
    }

    public void InitializeAttribute(T attributeType, float baseValue, float minValue = float.MinValue, float maxValue = float.MaxValue)
    {
        if (!Attributes.ContainsKey(attributeType))
        {
            Attributes[attributeType] = new GameplayAttribute(baseValue, minValue, maxValue);
        }
    }

    private void CheckAllAttributeDefined()
    {
        foreach (T type in System.Enum.GetValues(typeof(T)))
        {
            if (!Attributes.ContainsKey(type))
            {
                Debug.LogWarning($"Attribute {type} not found!");
            }
        }
    }

    // TODO: Deprecated Codes region은 추후 삭제합니다.
    #region Deprecated Codes
    public GameplayAttribute GetAttribute(T attributeType)
    {
        UnityEngine.Debug.Assert(Attributes.ContainsKey(attributeType));

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
    #endregion

    public virtual void AddExperience(float amount)
    {
        if (Level > levelUpData.Length)
        {
            return;
        }

        LevelUpData<T> data = levelUpData[(int)Level - 1];
        float remainingExp = Experience + amount;
        float requiredExp = data.expRequirement;

        if (remainingExp < requiredExp)
        {
            Experience = remainingExp;
            onExpChanged?.Invoke((int)Level, remainingExp, requiredExp);
            return;
        }

        // LevelUp!
        while (remainingExp >= requiredExp)
        {
            remainingExp -= requiredExp;
            Experience = remainingExp;
            Level += 1;
            data = levelUpData[(int)Level - 1];
            requiredExp = data.expRequirement;

            ModifyBaseValue(data.attributeType, data.deltaValue);

            onExpChanged?.Invoke((int)Level, remainingExp, requiredExp);
            onLevelUp?.Invoke((int)Level, remainingExp, requiredExp);
        }
    }
}
