namespace Unchord
{
    // 2-1
    public class MoscowFlame : SpecialAbilityComponent
    {
        protected override void OnEnableSpecial()
        {
            string key = FireballAttributeType.ExplosionSize;
            float dSize = base.AttributeBase[FireballAttributeType.ExplosionSize].CurrentValue;
            GameplayAttributeOperator opcode = GameplayAttributeOperator.PercentMul;

            Fireball weapon = base.Player.WeaponTransform.GetComponentInChildren<Fireball>();
            MoscowSpell spell = base.Player.SpellTransform.GetComponentInChildren<MoscowSpell>();

            GameplayAttributeModifier modW = new GameplayAttributeModifier(key, dSize, opcode);
            GameplayAttributeModifier modS = new GameplayAttributeModifier(key, dSize, opcode);

            weapon.AttributeBase[key].AddModifier(modW);
            spell.AttributeBase[key].AddModifier(modS);
        }
    }
}