using System.IO;
using UnityEngine;

namespace Unchord
{
    public abstract class WeaponComponent : AbilityComponent
    {
        protected const float c_DEFAULT_WEAPON_COOLDOWN = 1.0f;

        public AttributeBaseSet AttributeBase { get; private set; }

        public sealed override int MaxLevel => _attributeModifier.MaxLevel;

        protected bool _isCooldownPaused;

        private AttributeModifierSet _attributeModifier;
        private float _leftCooldown;

        protected override void Awake()
        {
            base.Awake();

            FileStream fs = new FileStream(Application.streamingAssetsPath + base.dataFilePathRelative, FileMode.Open, FileAccess.Read, FileShare.Read);
            MultiCSVReader rd = new MultiCSVReader(fs);

            this.AttributeBase = new AttributeBaseSet(rd);
            this._attributeModifier = new AttributeModifierSet(rd);

            rd.Close();
            fs.Close();
        }

        protected override void Update()
        {
            base.Update();

            if (ShouldUseWeapon())
            {
                UseWeapon();
                ResetCooldown();
            }
        }

        public abstract void UseWeapon();

        public sealed override void LevelUp()
        {
            base.LevelUp();

            if (!_attributeModifier.ContainsKey(CurrentLevel))
                return;

            UnityEngine.Debug.Assert(_attributeModifier[CurrentLevel] != null);

            AttributeBase.ApplyModifiers(_attributeModifier[CurrentLevel]);
        }

        public override string GetModifierDescription(int level)
        {
            return _attributeModifier.GetDescription(level);
        }

        private bool ShouldUseWeapon()
        {
            if (_leftCooldown <= 0.0f)
                return true;

            if (!_isCooldownPaused)
                _leftCooldown -= Time.deltaTime;

            return false;
        }

        private void ResetCooldown()
        {
            float w = UnityEngine.Random.value;
            float cooldown = c_DEFAULT_WEAPON_COOLDOWN;
            float dMin = 0.0f;
            float dMax = 0.0f;

            // TODO: 문자열 리터럴을 어떤 문자열 구조체에서 관리할지 결정해야 합니다.
            if (AttributeBase.ContainsKey("Cooldown"))
            {
                cooldown = AttributeBase["Cooldown"].CurrentValue;
            }

            // TODO: 문자열 리터럴을 어떤 문자열 구조체에서 관리할지 결정해야 합니다.
            if (AttributeBase.ContainsKey("CooldownOffsetMin"))
            {
                dMin = AttributeBase["CooldownOffsetMin"].CurrentValue;

                UnityEngine.Debug.Assert(dMin >= 0.0f);
            }

            // TODO: 문자열 리터럴을 어떤 문자열 구조체에서 관리할지 결정해야 합니다.
            if (AttributeBase.ContainsKey("CooldownOffsetMax"))
            {
                dMax = AttributeBase["CooldownOffsetMax"].CurrentValue;

                UnityEngine.Debug.Assert(dMax >= 0.0f);
            }

            float min = Mathf.Max(0.0f, cooldown - dMin);
            float max = cooldown + dMax;

            _leftCooldown = min + (max - min) * w;
        }
    }
}