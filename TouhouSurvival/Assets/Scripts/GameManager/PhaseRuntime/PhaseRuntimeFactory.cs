using UnityEngine;

namespace Unchord
{
    public static class PhaseRuntimeFactory
    {
        public static PhaseRuntime CreateRuntime(PhaseSO phaseSO)
        {
            if (phaseSO is BossPhaseSO)
                return new BossPhaseRuntime(phaseSO as BossPhaseSO);
            else if (phaseSO is StageSO)
                return new StageRuntime(phaseSO as StageSO);
            else if (phaseSO is PhaseCompositeSO)
                return new PhaseCompositeRuntime(phaseSO as PhaseCompositeSO);
            else if (phaseSO is SurvivalPhaseSO)
                return new SurvivalPhaseRuntime(phaseSO as SurvivalPhaseSO);
            else
            {
                Debug.Assert(false, "Invalid phase so type.");
                return null;
            }
        }
    }
}