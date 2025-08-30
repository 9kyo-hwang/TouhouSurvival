using System.IO;
using UnityEngine;

namespace Unchord
{
    public abstract class SpellComponent : AbilityComponent
    {
        public AttributeBaseSet AttributeBase { get; private set; }

        public sealed override int MaxLevel => _attributeModifier.MaxLevel;

        public bool IsCooldownPaused { get; set; } = false;

        private AttributeModifierSet _attributeModifier;

        public abstract void UseSpell();

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

        public sealed override void LevelUp()
        {
            base.LevelUp();

            if (!_attributeModifier.ContainsKey(CurrentLevel))
                return;

            UnityEngine.Debug.Assert(_attributeModifier[CurrentLevel] != null);

            AttributeBase.ApplyModifiers(_attributeModifier[CurrentLevel]);
        }
    }
}