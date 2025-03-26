namespace Unchord
{
    public class EnemyAttributeSet : AttributeSet
    {
        private Enemy _owner;

        protected override void Awake()
        {
            base.Awake();

            _owner = gameObject.GetComponent<Enemy>();
            Attributes[EnemyAttributeType.Health].OnAttributeChanged += OnHealthChanged;
        }

        private void OnHealthChanged(object sender, AttributeChangedEventArgs e)
        {
            //Debug.Log($"Health changed from {e.OldValue} to {e.NewValue}");
        }
    }
}
