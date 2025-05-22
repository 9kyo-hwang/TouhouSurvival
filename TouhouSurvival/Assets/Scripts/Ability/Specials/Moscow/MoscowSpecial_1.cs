using UnityEngine;

namespace Unchord
{
    public class MoscowSpecial_1 : SpecialAbilityComponent
    {
        public override void TestLog()
        {
            base.TestLog();

            Debug.Log($"base.Attributes[\"Attr0\"] == {base.Attributes["Attr0"].CurrentValue}");
        }
    }
}