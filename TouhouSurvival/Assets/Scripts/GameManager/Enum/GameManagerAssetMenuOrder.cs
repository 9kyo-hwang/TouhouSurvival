namespace Unchord
{
    public enum GameManagerAssetMenuOrder : int
    {
        // Stage or Phase Manager
        SpawnerSO = 0,
        StageSO,
        PhaseCompositeSO,
        SurvivalPhaseSO,
        BossPhaseSO,

        // Map
        FreeMapSO = 100,
        XScrollingMapSO,
        YScrollingMapSO,
        FixedMapSO
    }
}