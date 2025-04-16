using UnityEngine;

namespace Unchord
{
    public class ExperienceDropObject : Item
    {
        public float amount;

        public override void UseItem(Player player)
        {
            player.AttributeSet.AddExperience(amount);
            Destroy(this.gameObject);
        }
    }
}