using UnityEngine;

public class NewEnemyAttributeSet : AttributeSetBase
{
    protected override void Awake()
    {
        base.Awake();
        
        var healthAttribute = GetAttribute(PlayerAttributeNames.Health);
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
        foreach (var attribute in Attributes)
        {
            attribute.Value.ResetToBase();
        }
    }
}
