namespace Unchord
{
    public class StageRuntime : PhaseCompositeRuntime
    {
        private Map _map;

        public StageRuntime(StageSO phaseSO, GameManager.Properties properties)
        : base(phaseSO, properties)
        {
            _map = Map.Create(phaseSO.mapSO);
            _map.transform.parent = properties.RuntimeContainer;
        }

        public override void Update()
        {
            _map.ScrollMap(_gameManagerProperties.MainCamera);

            base.Update();
        }
    }
}