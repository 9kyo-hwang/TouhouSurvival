using UnityEngine;

namespace Unchord
{
    public abstract class WeaponComponent : AbilityComponent
    {
        public AttributeBaseSet AttributeBase { get; private set; }

        public sealed override int MaxLevel => _attributeModifier.MaxLevel;

        [Header("Weapon Basic Settings")]
        public string attributeXlsxPathRelative;
        public WeaponActivationMode weaponActivationMode = WeaponActivationMode.FixedCooltime;
        public float fixedCooltime = 1.0f;
        public float variableCooltimeMin = 1.0f;
        public float variableCooltimeMax = 2.0f;

        protected bool _isCooltimePaused;

        private AttributeModifierSet _attributeModifier;
        private float _leftCooltime;

        protected override void Awake()
        {
            base.Awake();

            string[] csvPaths = AttributeUtility.ConvertXlsxToCsv(attributeXlsxPathRelative);

            AttributeBase = AttributeBaseSet.LoadFromFile(csvPaths[0]);
            _attributeModifier = AttributeModifierSet.LoadFromFile(csvPaths[1]);
        }

        protected override void Update()
        {
            base.Update();

            switch (weaponActivationMode)
            {
                case WeaponActivationMode.Always:
                    // NOTE: 매번 무기를 사용하므로 조심해서 활용해야 함.
                    UseWeapon();
                    break;
                case WeaponActivationMode.FixedCooltime:
                    if (TryUpdateCooltime())
                        break;
                    UseWeapon();
                    ResetCooltime(fixedCooltime, fixedCooltime);
                    break;
                case WeaponActivationMode.VariableCooltime:
                    if (TryUpdateCooltime())
                        break;
                    UseWeapon();
                    ResetCooltime(variableCooltimeMin, variableCooltimeMax);
                    break;
                default:
                    UnityEngine.Debug.Assert(false, "Invalid case occurred. Please debug.");
                    break;
            }
        }

        public abstract void UseWeapon();

        public sealed override void LevelUp()
        {
            base.LevelUp();

            AttributeBase.ApplyModifiers(_attributeModifier[CurrentLevel]);
        }

        private bool TryUpdateCooltime()
        {
            if (_leftCooltime <= 0.0f)
                return false;

            if (!_isCooltimePaused)
                _leftCooltime -= Time.deltaTime;

            return true;
        }

        private void ResetCooltime(float minTime, float maxTime)
        {
            float w = UnityEngine.Random.value;
            float nextCooltime = minTime + (maxTime - minTime) * w;
            _leftCooltime += nextCooltime;
        }
    }
}