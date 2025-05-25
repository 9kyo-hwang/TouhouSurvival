namespace Unchord
{
    public abstract class SpellComponent : AbilityComponent
    {
        public AttributeBaseSet AttributeBase { get; private set; }

        public sealed override int MaxLevel => _attributeModifier.MaxLevel;

        public bool IsCooldownPaused { get; set; } = false;

        public string attributeXlsxPathRelative;

        private AttributeModifierSet _attributeModifier;

        public abstract void UseSpell();

        protected override void Awake()
        {
            base.Awake();

            string[] csvPaths = AttributeUtility.ConvertXlsxToCsv(attributeXlsxPathRelative);

            AttributeBase = AttributeBaseSet.LoadFromFile(csvPaths[0]);
            _attributeModifier = AttributeModifierSet.LoadFromFile(csvPaths[1]);
        }

        public sealed override void LevelUp()
        {
            base.LevelUp();

            AttributeBase.ApplyModifiers(_attributeModifier[CurrentLevel]);
        }
    }
}