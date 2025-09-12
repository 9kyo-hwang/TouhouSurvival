namespace Unchord
{
    public class HealObject : ItemComponent
    {
        public float amount;

        public override void Use(Player player)
        {
            player.AddHealth(amount);
            Destroy(this.gameObject);
        }
    }
}