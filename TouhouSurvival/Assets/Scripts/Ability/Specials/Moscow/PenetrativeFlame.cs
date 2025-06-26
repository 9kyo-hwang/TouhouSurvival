namespace Unchord
{
    // 2-2
    public class PenetrativeFlame : SpecialAbilityComponent
    {
        protected override void OnEnableSpecial()
        {
            string key = FireballAttributeType.ProjectilePenetrationCount;
            float dCount = base.AttributeBase[key].CurrentValue;
            GameplayAttributeOperator opcode = GameplayAttributeOperator.Flat;

            Fireball weapon = base.Player.WeaponTransform.GetComponentInChildren<Fireball>();
            MoscowSpell spell = base.Player.SpellTransform.GetComponentInChildren<MoscowSpell>();

            GameplayAttributeModifier modW = new GameplayAttributeModifier(key, dCount, opcode);
            GameplayAttributeModifier modS = new GameplayAttributeModifier(key, dCount, opcode);

            weapon.AttributeBase[key].AddModifier(modW);
            spell.AttributeBase[key].AddModifier(modS);
        }
    }
}