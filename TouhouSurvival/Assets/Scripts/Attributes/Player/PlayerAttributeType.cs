namespace Unchord
{
    public class PlayerAttributeType
    {
        /**
         *  TODO
         *  1. 모든 능력치에 대한 기본값 설정
         *  2. 각 능력치 별 증감 방식 결정
         *  3. 기본값을 기준으로 - and + 구조인지, 0(%)을 기준으로 순수 - or + 인지 확정
         *  4. 값 증가/비율 증가가 동시에 가능한 지 결정
         *  5. 모든 패시브가 동일한 레벨업 요구 경험치를 요구하는지? 무기와 동일한지?
         */
        public const string Health = "Health";                                      // 체력(Passive 가능/값)
        public const string HealthRegeneration = "HealthRegeneration";              // 초당 체력 재생량(Passive 가능/값)
        public const string MovementSpeed = "MovementSpeed";                        // 이동 속도(Passive 가능/비율)
        public const string Attack = "Attack";                                      // 공격력(Passive 가능/비율)
        public const string Defense = "Defense";                                    // 방어력(Passive 가능/값)
        public const string ScanRangeMultiplier = "ScanRangeMultiplier";            // 아이템 획득 거리 배율(Passive 가능/비율)
        public const string ExpGainIncrease = "ExpGainIncrease";                    // 경험치 획득량 증가율(100%/Passive 가능/비율)
        public const string CooldownDecrease = "CooldownDecrease";                  // 스킬 쿨타임 감소율(0%/Passive 가능/비율)
        public const string KnockBackStrength = "KnockBackStrength";                // 넉백율(100%/Passive 가능/비율)
        public const string ProjectileSpeedChange = "ProjectileSpeedChange";        // 투사체 속도 변화율(100%)
        public const string ProjectileSizeChange = "ProjectileSizeChange";          // 투사체 크기 변화율(100%/Passive 가능/비율)
        public const string ProjectileDurationChange = "ProjectileDurationChange";  // 투사체 지속시간 변화율(100%)
        public const string ProjectileIncreaseCount = "ProjectileIncreaseCount";    // 투사체 발사 개수 증가(0개)
        public const string Lucky = "Lucky";                                        // 행운(10/Passive 가능/값)
        public const string SpellCardGaugeAcquisitionRateChange = "SpellCardGaugeAcquisitionRateChange";
        public const string MaxSpellCount = "MaxSpellCount";                        // 최대 스펠(필살기) 소지 갯수
        public const string SpellCooldown = "SpellCooldown";                        // 스펠 재사용 대기시간
        public const string SpellAutoRechargeTime = "SpellAutoRechargeTime";        // 스펠 1개가 충전되는 데 걸리는 시간
    }
}