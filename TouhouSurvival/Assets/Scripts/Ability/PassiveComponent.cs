using System.IO;
using UnityEngine;

namespace Unchord
{
    public sealed class PassiveComponent : AbilityComponent
    {
        public sealed override int MaxLevel => _attributeModifier.MaxLevel;

        private AttributeModifierSet _attributeModifier;

        protected override void Awake()
        {
            base.Awake();

            FileStream fs = new FileStream(Application.streamingAssetsPath + base.dataFilePathRelative, FileMode.Open, FileAccess.Read, FileShare.Read);
            MultiCSVReader rd = new MultiCSVReader(fs);

            this._attributeModifier = new AttributeModifierSet(rd);

            rd.Close();
            fs.Close();
        }

        public sealed override void LevelUp()
        {
            base.LevelUp();

            if (!_attributeModifier.ContainsKey(CurrentLevel))
                return;

            UnityEngine.Debug.Assert(_attributeModifier[CurrentLevel] != null);

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