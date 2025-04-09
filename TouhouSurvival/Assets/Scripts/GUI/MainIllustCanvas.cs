namespace Unchord
{
    public class MainIllustCanvas : UnchordCanvas
    {
        public override void Show()
        {
            base.Show();

            transform.SetAsFirstSibling();
        }
    }
}