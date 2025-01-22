using System.Collections.Generic;
using UnityEngine;

namespace Unchord
{
    [CreateAssetMenu(fileName = "New Phase Composite", menuName = "Scriptable Objects/Game Management/Phase Composite", order = (int)GameManagerAssetMenuOrder.PhaseCompositeSO)]
    public class PhaseCompositeSO : PhaseSO
    {
        public List<PhaseSO> phaseSoList;
    }
}