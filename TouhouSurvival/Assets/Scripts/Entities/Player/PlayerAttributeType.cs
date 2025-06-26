namespace Unchord
{
    public class PlayerAttributeType
    {
        public const string HpMax = "HpMax";                // 최대 체력(Passive 가능/값)
        public const string HpRegen = "HpRegen";            // 초당 체력 재생(Passive 가능/값)
        public const string Speed = "Speed";                // 이동속도(Passive 가능/비율)
        public const string Damage = "Damage";              // 공격력(100%+-/Passive 가능/비율)
        public const string Armor = "Armor";                // 방어력(Passive 가능/값)
        public const string PickRange = "PickRange";        // 아이템 획득 거리(Passive 가능/비율)
        public const string ExpGain = "ExpGain";            // 경험치 획득률(100%+-/Passive 가능/비율)
        public const string Cooldown = "Cooldown";          // 쿨타임(100%+-/Passive 가능/비율)
        public const string KnockBack = "KnockBack";        // 넉백(100%+-/Passive 가능/비율)
        public const string ProjSpeed = "ProjSpeed";        // 투사체 속도(100%+-)
        public const string ProjSize = "ProjSize";          // 투사체 크기(100%+-/Passive 가능/비율)
        public const string ProjLifetime = "ProjLifetime";  // 투사체 지속시간(100%+-)
        public const string ProjCount = "ProjCount";        // 투사체 개수(0개+)
        public const string Lucky = "Lucky";                                        // 행운(10/Passive 가능/값)
        public const string SpellCardGaugeAcquisitionRateChange = "SpellCardGaugeAcquisitionRateChange";
        public const string MaxSpellCount = "MaxSpellCount";                        // 최대 스펠(필살기) 소지 개수
        public const string SpellCooldown = "SpellCooldown";                        // 스펠 재사용 대기시간
        public const string SpellAutoRechargeTime = "SpellAutoRechargeTime";        // 스펠 1개가 충전되는 데 걸리는 시간
    }
}