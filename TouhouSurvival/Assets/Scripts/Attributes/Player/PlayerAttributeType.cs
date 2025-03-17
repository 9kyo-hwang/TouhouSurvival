namespace Unchord
{
    public class PlayerAttributeType
    {
        public const string Health = "Health";                                      // 체력
        public const string MovementSpeed = "MovementSpeed";                        // 이동 속도
        public const string Attack = "Attack";                                      // 공격력
        public const string HealthRecovery = "HealthRecovery";                      // 초당 체력 회복량
        public const string Defense = "Defense";                                    // 방어력
        public const string ScanRangeMultiplier = "ScanRangeMultiplier";            // 아이템 획득 거리 배율
        public const string ExpGainIncrease = "ExpGainIncrease";                    // 경험치 획득량 증가율(기본 100%)
        public const string CooldownDecrease = "CooldownDecrease";                  // 스킬 쿨타임 감소율(기본 0%)
        public const string KnockBackStrength = "KnockBackStrength";                // 넉백율(기본 100%)
        public const string ProjectileSpeedChange = "ProjectileSpeedChange";        // 투사체 속도 변화율(기본 100%)
        public const string ProjectileSizeChange = "ProjectileSizeChange";          // 투사체 크기 변화율(기본 100%)
        public const string ProjectileDurationChange = "ProjectileDurationChange";  // 투사체 지속시간 변화율(기본 100%)
        public const string ProjectileIncreaseCount = "ProjectileIncreaseCount";    // 투사체 발사 개수 증가(기본 0개)
        public const string Lucky = "Lucky";                                        // 행운(기본값 10) = ""; 각종 확률에 보정
    }
}