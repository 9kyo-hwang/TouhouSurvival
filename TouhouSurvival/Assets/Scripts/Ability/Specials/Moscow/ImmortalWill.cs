namespace Unchord
{
    // 1-1
    public class ImmortalWill : SpecialAbilityComponent
    {
        public float GetHealthRegeneration(float currentHealth, float maxHealth, float finalHealthRegeneration)
        {
            float min = base.AttributeBase[PlayerAttributeType.HealthRegeneration + "Min"].CurrentValue;
            float max = base.AttributeBase[PlayerAttributeType.HealthRegeneration + "Max"].CurrentValue;
            float threshold = base.AttributeBase[PlayerAttributeType.HealthRegeneration + "Threshold"].CurrentValue;
            float health01 = currentHealth / maxHealth;

            if (health01 > threshold)
                return finalHealthRegeneration;

            float w = 1.0f - health01 / threshold;

            return finalHealthRegeneration * (min + (max - min) * w);
        }
    }
}