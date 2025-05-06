namespace Unchord
{
    public abstract class SpellComponent : AbilityComponent
    {
        public bool IsCooldownPaused { get; set; } = false;

        public abstract void UseSpell();
    }
}