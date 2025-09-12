namespace Unchord
{
    public class SpellEnergyObject : ItemComponent
    {
        public float amount = 1.0f;

        public override void Use(Player player)
        {
            player.AddSpellGauge(amount);
            Destroy(this.gameObject);
        }
    }
}