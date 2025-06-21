namespace Unchord
{
    public abstract class SpecialAbilityComponent : AbilityComponent
    {
        public AttributeBaseSet AttributeBase { get; private set; }

        public sealed override int MaxLevel => 1;

        public string attributeXlsxPathRelative;

        protected override void Awake()
        {
            base.Awake();

            string[] csvPaths = AttributeUtility.ConvertXlsxToCsv(attributeXlsxPathRelative);

            AttributeBase = AttributeBaseSet.LoadFromFile(csvPaths[0]);
        }

        public sealed override void LevelUp()
        {
            base.LevelUp();

            if (base.CurrentLevel != 1)
                return;

            OnEnableSpecial();
        }

        protected virtual void OnEnableSpecial()
        {

        }
    }
}