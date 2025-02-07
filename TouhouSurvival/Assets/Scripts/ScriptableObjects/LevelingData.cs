using System;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelingData", menuName = "Scriptable Objects/LevelingData")]
public class LevelingData : ScriptableObject
{
    [Serializable]
    public struct LevelRequirement
    {
        public int requiredExp;                 // 다음 레벨로 업하기 위한 필요 경험치
        public AttributeBonus[] levelUpBonuses; // 다음 레벨로 업할 때 오르는 attribute들
    }

    /**
     * 레벨 업 시 증가할 스탯들. 예) 2레벨 시 Hp +10, Attack +2
     * 사용 예시
        levelRequirements[0] = new LevelRequirement {
            level = 2,
            requiredExp = 100,
            levelUpBonuses = new AttributeBonus[] {
                new AttributeBonus { attributeName = "Health", bonusValue = 10f },
                new AttributeBonus { attributeName = "AttackPower", bonusValue = 2f }
            }
        };
     */
    [Serializable]
    public struct AttributeBonus
    {
        public string attributeName;
        public float bonusValue;

        public override string ToString()
        {
            return $"Attribute: {attributeName}, Increase : {bonusValue}";
        }
    }
    
    public LevelRequirement[] levelRequirements;
}
