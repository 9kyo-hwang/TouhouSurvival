using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class PlayerAttributeNames
{
    public const string Health = "Health";
    public const string Speed = "Speed";
    public const string Attack = "Attack";
    public const string ScanRangeMultiplier = "ScanRangeMultiplier";
    public const string ExperienceGainMultiplier = "ExperienceGainMultiplier";
}

public class NewPlayerAttributeSet : AttributeSetBase
{
    [SerializeField] private LevelingData levelingData;
    public float Level { get; private set; } = 1;
    public float Experience { get; private set; } = 0;
    
    protected override void Awake()
    {
        base.Awake();
     
        if (Attributes.Count == 0)
        {
            Debug.Assert(false, "PlayerAttributeSet is empty");
            return;
        }
        
        var healthAttribute = GetAttribute(PlayerAttributeNames.Health);
        if (healthAttribute != null)
        {
            healthAttribute.OnAttributeChanged += OnHealthChanged;
        }
    }

    public void AddExperience(float amount)
    {
        if (Level > levelingData.levelRequirements.Length)
        {
            return;
        }
        
        float expGainMultiplier = GetAttributeValue(PlayerAttributeNames.ExperienceGainMultiplier);
        amount *= expGainMultiplier;
        
        var requirement = levelingData.levelRequirements[(int)Level - 1];
        if (Experience + amount < requirement.requiredExp)
        {
            Experience += amount;
            return;
        }
        
        float remainingExp = Experience + amount - requirement.requiredExp;
        Experience = remainingExp;
        Level += 1;

        foreach (var bonus in requirement.levelUpBonuses)
        {
            ModifyAttributeValueBase(bonus.attributeName, bonus.bonusValue);
        }

        foreach (var attribute in Attributes)
        {
            attribute.Value.ResetToBase();
        }
    }
    
    private void OnHealthChanged(object sender, AttributeChangedEventArgs e)
    {
        //Debug.Log($"Health changed from {e.OldValue} to {e.NewValue}");
    }
}
