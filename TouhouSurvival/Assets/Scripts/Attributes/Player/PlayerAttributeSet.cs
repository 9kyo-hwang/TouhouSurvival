using System;
using UnityEngine;

public class PlayerAttributeSet : AttributeSet
{
    [SerializeField] protected LevelingData levelingData;
    protected override void Awake()
    {
        // 인스펙터 창에서 세팅되지 않았을 경우 초기값
        if (attributes.Count == 0)
        {
            // attributes.Add(new AttributeData(AttributeNames.Level, 1, 1, 99));
            // attributes.Add(new AttributeData(AttributeNames.Experience, 0, 0));
            // attributes.Add(new AttributeData(AttributeNames.Health, 100, 0));
            // attributes.Add(new AttributeData(AttributeNames.Attack, 10));
            // attributes.Add(new AttributeData(AttributeNames.Speed, 1));
            
            Debug.Assert(false, "PlayerAttributeSet is not set");
            return;
        }
        
        base.Awake();
        OnAttributeChanged += ActionOnAttributeChanged;
    }

    private void ActionOnAttributeChanged(string attributeName, float oldValue, float newValue)
    {
        switch (attributeName)
        {
            case "Experience":
                CheckLevelUp();
                break;
            case "Health" when newValue <= 0:
                ActionOnHealthZero();
                break;
        }
    }

    public void AddExperience(float amount)
    {
        // 필요한 경우 경험치 증가량 어트리뷰트를 얻어서 연산 후 적용
        ModifyAttribute("Experience", amount);
    }

    private void CheckLevelUp()
    {
        int currentLevel = (int)GetAttribute("Level");
        float currentExp = GetAttribute("Experience");
        
        var requirement = levelingData.levelRequirements[currentLevel - 1];  // level은 1부터 시작하므로
        if (currentExp >= requirement.requiredExp)
        {
            // 경험치 초과분
            float remainingExp = currentExp - requirement.requiredExp;
            SetAttribute("Experience", remainingExp);

            // 1레벨 증가
            ModifyAttribute("Level", 1);

            // 레벨 업 시 증가하는 어트리뷰트
            foreach (var bonus in requirement.levelUpBonuses)
            {
                ModifyAttribute(bonus.attributeName, bonus.bonusValue);
            }
        }
    }

    private void ActionOnHealthZero()
    {
        Debug.Log("Player Dead!");
        // TODO: 사망 처리 로직
    }
}
