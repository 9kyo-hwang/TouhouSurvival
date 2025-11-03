namespace Unchord
{
    public class GoldObject : ItemComponent
    {
        public int amount;

        public override void Use(Player player)
        {
            GameManager gm = GameManager.Instance;
            UIManager ui = UIManager.Instance;

            gm.EarnedGold += amount;
            ui.GameCanvas.SetEarnedGold(gm.EarnedGold);

            SoundManager.Instance.SFX.AddSoundEvent("event:/SFX/Gold");
            
            Destroy(this.gameObject);
        }
    }
}