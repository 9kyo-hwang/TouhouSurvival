using UnityEngine;

namespace Unchord
{
    public abstract class SpecialAbilityComponent : AbilityComponent
    {
        public virtual void TestLog()
        {

        }

        protected override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(KeyCode.F5))
            {
                TestLog();
            }
        }
    }
}