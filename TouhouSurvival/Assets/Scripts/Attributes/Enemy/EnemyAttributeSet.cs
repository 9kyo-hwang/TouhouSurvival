using System.Collections.Generic;
using UnityEngine;

public enum EnemyAttributeType
{
    Health,
    Speed,
    Attack,
    DropRate
}

public class EnemyAttributeSet : AttributeSetBase
{
    protected override void Awake()
    {
        base.Awake();
        
        GameplayAttribute healthAttribute = GetAttribute(EnemyAttributeType.Health.ToString());
        if (healthAttribute != null)
        {
            healthAttribute.OnAttributeChanged += OnHealthChanged;
        }
    }
    
    private void OnHealthChanged(object sender, AttributeChangedEventArgs e)
    {
        //Debug.Log($"Health changed from {e.OldValue} to {e.NewValue}");
    }

    public void ResetAttributes()
    {
        foreach (KeyValuePair<string, GameplayAttribute> attribute in Attributes)
        {
            attribute.Value.ResetToBase();
        }
    }
}
