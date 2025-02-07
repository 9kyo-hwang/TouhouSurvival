using UnityEngine;

public class EnemyAttributeSet : AttributeSet
{
    protected override void Awake()
    {
        base.Awake();
        OnAttributeChanged += HandleAttributeChanged;
    }
    
    private void HandleAttributeChanged(string attributeName, float oldValue, float newValue)
    {
        switch (attributeName)
        {
            case "Health" when newValue <= 0:
                ActionOnHealthZero();
                break;
        }
    }

    private void ActionOnHealthZero()
    {
        Debug.Log("Enemy Dead!");
        OnAttributeChanged -= HandleAttributeChanged;  // 중복 등록을 막기 위해
    }

    public void ResetAttributes()
    {
        foreach (AttributeData attribute in attributes)
        {
            attribute.ResetCurrentValue();
        }
    }
}
