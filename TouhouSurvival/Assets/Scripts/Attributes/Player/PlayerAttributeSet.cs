using System;
using System.Collections;
using System.Collections.Generic;
using Unchord;
using UnityEngine;
using UnityEngine.Serialization;

public enum PlayerAttributeType
{
    MaxHealth,              // 최대 체력
    MovementSpeed,          // 이동 속도
    Attack,                 // 공격력
    HealthRecovery,         // 초당 체력 회복량
    Defense,                // 방어력
    ScanRangeMultiplier,    // 아이템 획득 거리 배율
    ExpGainIncrease,        // 경험치 획득량 증가율(기본 100%)
    CooldownDecrease,       // 스킬 쿨타임 감소율(기본 0%)
    KnockBackStrength,      // 넉백율(기본 100%)
    ProjectileSpeedChange,  // 투사체 속도 변화율(기본 100%)
    ProjectileSizeChange,   // 투사체 크기 변화율(기본 100%)
    ProjectileDurationChange,   // 투사체 지속시간 변화율(기본 100%)
    ProjectileIncreaseCount,    // 투사체 발사 개수 증가(기본 0개)
    Lucky,                      // 행운(기본값 10), 각종 확률에 보정
}

public class PlayerAttributeSet : AttributeSetBase<PlayerAttributeType>
{
    public float Level { get; private set; } = 1;
    public float Experience { get; private set; } = 0;
    
    [Serializable]
    public struct LevelUpData
    {
        public int requiredExp;
        public PlayerAttributeType levelUpAttributeType;
        public float levelUpAttributeValue;
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
        
        GameplayAttribute healthAttribute = GetAttribute(PlayerAttributeType.MaxHealth);
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
        
        float expGainIncrease = GetCurrentValue(PlayerAttributeType.ExpGainIncrease);
        amount *= expGainIncrease;
        
        LevelUpData data = levelUpData[(int)Level - 1];
        if (Experience + amount < data.requiredExp)
        {
            Experience += amount;
            return;
        }
        
        // LevelUp!
        float remainingExp = Experience + amount - data.requiredExp;
        Experience = remainingExp;
        Level += 1;

        ModifyBaseValue(data.levelUpAttributeType, data.levelUpAttributeValue);

        foreach (KeyValuePair<PlayerAttributeType, GameplayAttribute> attribute in Attributes)
        {
            attribute.Value.ResetToBase();
        }
        
        GameManager.Instance.BlockingEvent.Publish(OnLevelUp());
    }

    private IEnumerator OnLevelUp()
    {
        // LevelUpCanvas levelUpCanvas = UIManager.Instance.LevelUpCanvas;
        // levelUpCanvas.Show();
        // yield return new WaitUntil(() => levelUpCanvas.IsButtonClicked);
        //
        // // TODO: 기능 처리
        //
        // levelUpCanvas.Hide();

        LobbyCanvas canvas = UIManager.Instance.LobbyCanvas;
        canvas.Show();

        Debug.Log("Level Up! Show Canvas");
        yield return new WaitForSecondsRealtime(1f);
        Debug.Log("Level Up End! Hide Canvas");
        
        canvas.Hide();
    }
    
    
    private void OnHealthChanged(object sender, AttributeChangedEventArgs e)
    {
        //Debug.Log($"Health changed from {e.OldValue} to {e.NewValue}");
        
        if (e.NewValue <= 0.0f)
        {
            // TODO: Call Player OnDead Method
        }
        else
        {
            // TODO: Call Player OnHit Method
        }
    }
}
