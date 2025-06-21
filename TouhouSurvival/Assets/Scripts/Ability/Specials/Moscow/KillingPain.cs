namespace Unchord
{
    // 1-3
    public class KillingPain : SpecialAbilityComponent
    {
        private int _resurrectCountMax;
        private int _resurrectCount;

        protected override void Start()
        {
            base.Start();

            _resurrectCountMax = (int)base.AttributeBase["ResurrectCount"].CurrentValue;
            _resurrectCount = _resurrectCountMax;
        }

        public bool TryResurrect()
        {
            if (_resurrectCount == 0)
                return false;

            --_resurrectCount;
            return true;
        }
    }
}