using System.IO;
using UnityEngine;

namespace Unchord
{
    public abstract class SpecialAbilityComponent : AbilityComponent
    {
        public AttributeBaseSet AttributeBase { get; private set; }

        public sealed override int MaxLevel => 1;

        protected override void Awake()
        {
            base.Awake();

            FileStream fs = new FileStream(Application.streamingAssetsPath + base.dataFilePathRelative, FileMode.Open, FileAccess.Read);
            MultiCSVReader rd = new MultiCSVReader(fs);

            this.AttributeBase = new AttributeBaseSet(rd);
            
            rd.Close();
            fs.Close();
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