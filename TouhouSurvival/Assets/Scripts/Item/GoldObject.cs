namespace Unchord
{
    public class GoldObject : ItemComponent
    {
        public int amount;

        public override void Use(Player player)
        {
            GameManager.Instance.EarnedGold += amount;
            Destroy(this.gameObject);
        }
    }
}