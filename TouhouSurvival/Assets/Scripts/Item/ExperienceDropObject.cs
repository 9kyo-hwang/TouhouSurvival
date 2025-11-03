using UnityEngine;

namespace Unchord
{
    public class ExperienceDropObject : ItemComponent
    {
        public float amount;

        public override void Use(Player player)
        {
            player.LevelSystem.Experience += amount;

            SoundManager.Instance.SFX.AddSoundEvent("event:/SFX/Xp");

            Destroy(this.gameObject);
        }
    }
}