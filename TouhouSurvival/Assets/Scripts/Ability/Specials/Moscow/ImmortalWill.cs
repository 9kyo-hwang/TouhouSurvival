namespace Unchord
{
    // 1-1
    public class ImmortalWill : SpecialAbilityComponent
    {
        private GameplayAttributeModifier _modifier;
        private GameplayAttribute _attrHealthMax;
        private GameplayAttribute _attrHealthRegen;

        protected override void Awake()
        {
            base.Awake();

            _modifier = new GameplayAttributeModifier(
                PlayerAttributeType.HealthRegeneration,
                0.0f,
                GameplayAttributeOperator.PercentAdd);
        }

        protected override void Start()
        {
            base.Start();

            _attrHealthMax = base.Player.AttributeBase[PlayerAttributeType.Health];
            _attrHealthRegen = base.Player.AttributeBase[PlayerAttributeType.HealthRegeneration];

            _attrHealthRegen.AddModifier(_modifier);
        }

        protected override void Update()
        {
            base.Update();

            _attrHealthRegen.RemoveModifier(_modifier);
            _modifier.value = GetHealthRegeneration(base.Player.CurrentHealth, _attrHealthMax.CurrentValue);
            _attrHealthRegen.AddModifier(_modifier);
        }

        public float GetHealthRegeneration(float currentHealth, float maxHealth)
        {
            float min = base.AttributeBase[PlayerAttributeType.HealthRegeneration + "Min"].CurrentValue;
            float max = base.AttributeBase[PlayerAttributeType.HealthRegeneration + "Max"].CurrentValue;
            float threshold = base.AttributeBase[PlayerAttributeType.HealthRegeneration + "Threshold"].CurrentValue;
            float health01 = currentHealth / maxHealth;

            if (health01 > threshold)
                return 0.0f;

            float w = 1.0f - health01 / threshold;

            return min + (max - min) * w;
        }
    }
}