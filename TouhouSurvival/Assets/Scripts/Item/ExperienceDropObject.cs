using UnityEngine;

namespace Unchord
{
    public class ExperienceDropObject : ItemComponent
    {
        public float amount;

        public override void Use(Player player)
        {
            player.LevelSystem.Experience += amount;
            Destroy(this.gameObject);
        }
    }
}