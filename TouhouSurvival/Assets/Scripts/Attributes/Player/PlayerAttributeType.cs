namespace Unchord
{
    public enum PlayerAttributeType
    {
        Health,                 // 체력
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
}