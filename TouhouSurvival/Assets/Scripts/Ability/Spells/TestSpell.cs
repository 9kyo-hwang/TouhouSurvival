using UnityEngine;

namespace Unchord
{
    public class TestSpell : SpellComponent
    {
        public override void UseSpell()
        {
            Debug.Log("Test Spell Using OK.");
        }
    }
}