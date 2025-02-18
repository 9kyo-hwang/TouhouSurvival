using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum PlayerAttributeType
{
    Health,
    Speed,
    Attack,
    HealthRegenPerSecs,
    Defense,
    ScanRangeMultiplier,
    ExperienceGainRate,
    CooldownReduceRate,
    KnockBackStrengthRate,
    ProjectileSpeed,
    ProjectileSize,
    ProjectileDuration,
    ProjectileCount,
}

public class PlayerAttributeSet : AttributeSetBase
{
    public float Level { get; private set; } = 1;
    public float Experience { get; private set; } = 0;
    
    [Serializable]
    public struct LevelUpData
    {
        public int requiredExp;
        public PlayerAttributeType attribute;
        public float value;
    }

    public LevelUpData[] levelUpData;
    
    protected override void Awake()
    {
        base.Awake();
     
        if (Attributes.Count == 0)
        {
            Debug.Assert(false, "PlayerAttributeSet is empty");
            return;
        }
        
        GameplayAttribute healthAttribute = GetAttribute(PlayerAttributeType.Health.ToString());
        if (healthAttribute != null)
        {
            healthAttribute.OnAttributeChanged += OnHealthChanged;
        }
    }

    public void AddExperience(float amount)
    {
        if (Level > levelUpData.Length)
        {
            return;
        }
        
        float expGainMultiplier = GetAttributeValue(PlayerAttributeType.ExperienceGainRate.ToString());
        amount *= expGainMultiplier;
        
        LevelUpData data = levelUpData[(int)Level - 1];
        if (Experience + amount < data.requiredExp)
        {
            Experience += amount;
            return;
        }
        
        float remainingExp = Experience + amount - data.requiredExp;
        Experience = remainingExp;
        Level += 1;

        ModifyAttributeBaseValue(data.attribute.ToString(), data.value);

        foreach (KeyValuePair<string, GameplayAttribute> attribute in Attributes)
        {
            attribute.Value.ResetToBase();
        }
    }
    
    private void OnHealthChanged(object sender, AttributeChangedEventArgs e)
    {
        //Debug.Log($"Health changed from {e.OldValue} to {e.NewValue}");
    }
}
