namespace Unchord
{
    public class StageRuntime : PhaseCompositeRuntime
    {
        private Map _map;

        public StageRuntime(StageSO phaseSO)
        : base(phaseSO)
        {
            _map = Map.Create(phaseSO.mapSO);
            _map.transform.parent = s_gameManager.RuntimeContainer;
        }

        public override void Update()
        {
            _map.ScrollMap(s_gameManager.MainCamera);

            base.Update();
        }
    }
}