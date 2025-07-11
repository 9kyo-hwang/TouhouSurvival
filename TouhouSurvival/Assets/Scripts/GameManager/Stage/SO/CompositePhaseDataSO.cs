using System.Collections.Generic;
using UnityEngine;

namespace Unchord
{
    [CreateAssetMenu(fileName = "New Phase Composite", menuName = "Scriptable Objects/Game Management/Phase Composite", order = (int)GameManagerAssetMenuOrder.PhaseCompositeSO)]
    public sealed class CompositePhaseDataSO : PhaseDataSO
    {
        public List<PhaseDataSO> phaseList;

        public override IRuntime CreateRuntime(PhaseRuntimeCommons commonData) => new CompositePhaseRuntime(phaseList, commonData);
    }
}