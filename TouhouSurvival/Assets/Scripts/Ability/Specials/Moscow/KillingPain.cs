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

            _modifier = new GameplayAttributeModifier(
                PlayerAttributeType.HpRegen,
                0.0f,
                GameplayAttributeOperator.PercentAdd);
        }

        protected override void Start()
        {
            base.Start();

            _gm = GameManager.Instance;

            _attrHealthRegen = base.Player.AttributeBase[PlayerAttributeType.HpRegen];

            _attrHealthRegen.AddModifier(_modifier);
        }

        protected override void Update()
        {
            base.Update();

            // TODO: 다음 알고리즘을 이 곳에 작성합니다.

            // 1. _gm에서 값을 얻어옴: 부활한 횟수 k; 부활 가능 횟수 kMax;

            // 2-1. if, _resurrectedCount == k, then, return;

            // 2-2. else, then, _resurrectedCount = k; continue;

            // 3. w = k / kMax; 따라서, w는 0과 1 사이의 실수.

            // 4. (적용할 체력 재생) = (최대 체력 재생) * w
            //// _attrHealthRegen.RemoveModifier(_modifier);
            //// _modifier.value = base.AttributeBase[PlayerAttributeType.HpRegen + "Max"].CurrentValue * w;
            //// _attrHealthRegen.AddModifier(_modifier);
        }

        protected override void OnEnableSpecial()
        {
            base.OnEnableSpecial();

            float resurrectCountMax = base.AttributeBase["ResurrectCount"].CurrentValue;

            _resurrectedCount = 0;

            // TODO: 다음 알고리즘을 이 곳에 작성합니다.
            // 1. _gm.(부활 가능 횟수) += resurrectCountMax;
        }
    }
}