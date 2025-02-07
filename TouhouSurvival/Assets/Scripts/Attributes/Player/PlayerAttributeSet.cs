using System;
using UnityEngine;

public class PlayerAttributeSet : AttributeSet
{
    [SerializeField] protected LevelingData levelingData;
    public float CurrentExperience { get; private set; }
    
    protected override void Awake()
    {
        // 인스펙터 창에서 세팅되지 않았을 경우 초기값
        if (attributes.Count == 0)
        {
            Debug.Assert(false, "PlayerAttributeSet is not set");
            return;
        }
        
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

    public void AddExperience(float amount)
    {
        // 필요한 경우 경험치 증가량 어트리뷰트를 얻어서 연산 후 적용
        CheckLevelUp(amount);
    }

    private void CheckLevelUp(float amount)
    {
        int currentLevel = (int)GetAttribute("Level");
        Debug.Log($"Current level: {currentLevel}, CurrentExperience: {CurrentExperience}");
        if (currentLevel > levelingData.levelRequirements.Length)
        {
            Debug.Log("maximum level!");
            return;
        }
        
        var requirement = levelingData.levelRequirements[currentLevel - 1];  // level은 1부터 시작하므로
        if (CurrentExperience + amount < requirement.requiredExp)
        {
            Debug.Log("Not enough experience to level up");
            CurrentExperience += amount;
            return;
        }
        
        // 경험치 초과분
        float remainingExp = CurrentExperience + amount - requirement.requiredExp;
        CurrentExperience = remainingExp;

        // 1레벨 증가
        ModifyAttributeBase("Level", 1);
        Debug.Log("Level Up!");

        // 레벨 업 시 증가하는 어트리뷰트
        foreach (var bonus in requirement.levelUpBonuses)
        {
            Debug.Log(bonus);
            ModifyAttributeBase(bonus.attributeName, bonus.bonusValue);
        }
    }

    private void ActionOnHealthZero()
    {
        Debug.Log("Player Dead!");
        // TODO: 사망 시 관련 스탯 처리
    }
}
