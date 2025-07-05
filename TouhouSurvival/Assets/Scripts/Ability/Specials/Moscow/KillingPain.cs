namespace Unchord
{
    // 1-3
    public class KillingPain : SpecialAbilityComponent
    {
        private GameManager _gm;

        private GameplayAttributeModifier _modifier;
        private GameplayAttribute _attrHealthRegen;

        private int _resurrectedCount;

        protected override void Awake()
        {
            base.Awake();

            _gm = GameManager.Instance;

            _modifier = new GameplayAttributeModifier(
                PlayerAttributeType.HpRegen,
                0.0f,
                GameplayAttributeOperator.PercentAdd);
        }

        protected override void Start()
        {
            base.Start();

            _attrHealthRegen = base.Player.AttributeBase[PlayerAttributeType.HpRegen];

            _attrHealthRegen.AddModifier(_modifier);
        }

        protected override void Update()
        {
            base.Update();

            if (_resurrectedCount == _gm.ResurrectedCount)
                return;

            _resurrectedCount = _gm.ResurrectedCount;

            float w = (float)_resurrectedCount / (float)_gm.ResurrectCountMax;

            _attrHealthRegen.RemoveModifier(_modifier);
            _modifier.value = base.AttributeBase[PlayerAttributeType.HpRegen + "Max"].CurrentValue * w;
            _attrHealthRegen.AddModifier(_modifier);
        }

        protected override void OnEnableSpecial()
        {
            base.OnEnableSpecial();

            int resurrectCountMax = (int)base.AttributeBase["ResurrectCount"].CurrentValue;

            _resurrectedCount = _gm.ResurrectedCount;
            _gm.ResurrectCountMax += resurrectCountMax;
        }
    }
}