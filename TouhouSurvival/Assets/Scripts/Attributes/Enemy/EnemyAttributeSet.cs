using UnityEngine;

public class EnemyAttributeSet : AttributeSet
{
    protected override void Awake()
    {
        base.Awake();
        OnAttributeChanged += ActionOnAttributeChanged;
    }
    
    private void ActionOnAttributeChanged(string attributeName, float oldValue, float newValue)
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
        OnAttributeChanged -= ActionOnAttributeChanged;  // 중복 등록을 막기 위해
    }

    public void ResetAttributes()
    {
        foreach (AttributeDataSO attribute in attributes)
        {
            attribute.ResetCurrentValue();
        }
    }
}
