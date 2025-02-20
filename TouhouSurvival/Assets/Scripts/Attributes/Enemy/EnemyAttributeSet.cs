using System.Collections.Generic;
using Unchord;
using UnityEngine;

public enum EnemyAttributeType
{
    Health,
    Speed,
    Attack,
    DropRate
}

public class EnemyAttributeSet : AttributeSetBase<EnemyAttributeType>
{
    private Enemy _owner;
    protected override void Awake()
    {
        base.Awake();

        _owner = gameObject.GetComponent<Enemy>();
        
        GameplayAttribute healthAttribute = GetAttribute(EnemyAttributeType.Health);
        if (healthAttribute != null)
        {
            healthAttribute.OnAttributeChanged += OnHealthChanged;
        }
    }
    
    private void OnHealthChanged(object sender, AttributeChangedEventArgs e)
    {
        //Debug.Log($"Health changed from {e.OldValue} to {e.NewValue}");
        if (e.NewValue <= 0.0f)
        {
            _owner.OnDead();
        }
        else
        {
            _owner.OnHit(1.0f);
        }
    }

    public void ResetAttributes()
    {
        foreach (KeyValuePair<EnemyAttributeType, GameplayAttribute> attribute in Attributes)
        {
            attribute.Value.ResetToBase();
        }
    }
}
