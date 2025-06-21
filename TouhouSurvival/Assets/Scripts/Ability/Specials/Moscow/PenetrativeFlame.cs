namespace Unchord
{
    // 2-2
    public class PenetrativeFlame : SpecialAbilityComponent
    {
        public float PenetrationCount => base.AttributeBase["PenetrationCount"].CurrentValue;
    }
}