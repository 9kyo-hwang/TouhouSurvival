namespace Unchord
{
    public sealed class PassiveComponent : AbilityComponent
    {
        public sealed override int MaxLevel => _attributeModifier.MaxLevel;

        public string attributeXlsxPathRelative;

        private AttributeModifierSet _attributeModifier;

        protected override void Awake()
        {
            base.Awake();

            string[] csvPaths = AttributeUtility.ConvertXlsxToCsv(attributeXlsxPathRelative);

            _attributeModifier = AttributeModifierSet.LoadFromFile(csvPaths[1]);
        }

        public sealed override void LevelUp()
        {
            base.LevelUp();

            Player player = GameManager.Instance.Player;
            AttributeBaseSet attr = player.AttributeBase;

            attr.ApplyModifiers(_attributeModifier[CurrentLevel]);
        }

        public override string GetModifierDescription(int level)
        {
            return _attributeModifier.GetDescription(level);
        }
    }
}