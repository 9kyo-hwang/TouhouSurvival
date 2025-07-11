namespace Unchord
{
    // 1-1
    public class ImmortalWill : SpecialAbilityComponent
    {
        private GameplayAttributeModifier _modifier;
        private GameplayAttribute _attrHpMax;
        private GameplayAttribute _attrHpRegen;

        protected override void Awake()
        {
            base.Awake();

            _modifier = new GameplayAttributeModifier(
                PlayerAttributeType.HpRegen,
                0.0f,
                GameplayAttributeOperator.PercentAdd);
        }

        protected override void Start()
        {
            base.Start();

            _attrHpMax = base.Player.AttributeBase[PlayerAttributeType.HpMax];
            _attrHpRegen = base.Player.AttributeBase[PlayerAttributeType.HpRegen];

            _attrHpRegen.AddModifier(_modifier);
        }

        protected override void Update()
        {
            base.Update();

            _attrHpRegen.RemoveModifier(_modifier);
            _modifier.value = GetHealthRegeneration(base.Player.CurrentHealth, _attrHpMax.CurrentValue);
            _attrHpRegen.AddModifier(_modifier);
        }

        public float GetHealthRegeneration(float currentHealth, float maxHealth)
        {
            float min = base.AttributeBase[PlayerAttributeType.HpRegen + "Min"].CurrentValue;
            float max = base.AttributeBase[PlayerAttributeType.HpRegen + "Max"].CurrentValue;
            float threshold = base.AttributeBase[PlayerAttributeType.HpRegen + "Threshold"].CurrentValue;
            float health01 = currentHealth / maxHealth;

            if (health01 > threshold)
                return 0.0f;

            float w = 1.0f - health01 / threshold;

            return min + (max - min) * w;
        }
    }
}