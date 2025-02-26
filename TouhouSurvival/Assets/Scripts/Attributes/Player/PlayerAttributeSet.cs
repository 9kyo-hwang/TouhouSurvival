using System;
using System.Collections;
using System.Collections.Generic;
using Unchord;
using UnityEngine;
using UnityEngine.Serialization;

public enum PlayerAttributeType
{
    Health,                 // 현재 체력
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
    protected override void Awake()
    {
        base.Awake();

        base.onExpChanged += this.OnExpChange;
        base[PlayerAttributeType.Health].OnAttributeChanged += OnHealthChanged;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F4))
        {
            Debug.Log("Get 1 Exp.");
            AddExperience(1.0f);
        }
    }

    public override void AddExperience(float amount)
    {
        float expGainIncrease = GetCurrentValue(PlayerAttributeType.ExpGainIncrease);
        base.AddExperience(amount * expGainIncrease);
    }

    private void OnExpChange(int level, float remainingExp, float requiredExp)
    {
        LevelUpData<PlayerAttributeType> data = levelUpData[(int)Level - 1];
        UIManager.Instance.GameCanvas.SetExpGauge(Experience, data.expRequirement);
        UIManager.Instance.GameCanvas.SetPlayerLevel((int)this.Level);
    }

    private void OnHealthChanged(object sender, AttributeChangedEventArgs e)
    {
        // TODO: 플레이어의 현재 체력을 UI에 표시하는 코드를 이 곳에 작성합니다.
    }
}
