using UnityEngine;

namespace Unchord
{
    public static class PhaseRuntimeFactory
    {
        public static PhaseRuntime CreateRuntime(PhaseSO phaseSO, GameManager.Properties gameManagerProperties)
        {
            if (phaseSO is BossPhaseSO)
                return new BossPhaseRuntime(phaseSO as BossPhaseSO, gameManagerProperties);
            else if (phaseSO is StageSO)
                return new StageRuntime(phaseSO as StageSO, gameManagerProperties);
            else if (phaseSO is PhaseCompositeSO)
                return new PhaseCompositeRuntime(phaseSO as PhaseCompositeSO, gameManagerProperties);
            else if (phaseSO is SurvivalPhaseSO)
                return new SurvivalPhaseRuntime(phaseSO as SurvivalPhaseSO, gameManagerProperties);
            else
            {
                Debug.Assert(false, "Invalid phase so type.");
                return null;
            }
        }
    }
}