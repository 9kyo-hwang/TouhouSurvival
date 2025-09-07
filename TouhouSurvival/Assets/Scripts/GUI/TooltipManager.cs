namespace Unchord
{
    public class TooltipManager : UIManagerBase<TooltipManager>
    {
        public Tooltip Tooltip => this.GetTooltip("GUIs/Element/Tooltip");

        private Tooltip GetTooltip(string resourcePath)
        {
            return base.GetComponentFromTable<Tooltip>(resourcePath, this.transform, true);
        }
    }
}