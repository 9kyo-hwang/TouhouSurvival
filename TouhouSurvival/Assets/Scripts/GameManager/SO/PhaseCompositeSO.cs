using System.Collections.Generic;
using UnityEngine;

namespace Unchord
{
    [CreateAssetMenu(fileName = "New Phase Composite", menuName = "ScriptableObjects/Game Management/Phase Composite", order = (int)GameManagerAssetMenuOrder.PhaseCompositeSO)]
    public class PhaseCompositeSO : PhaseSO
    {
        public List<PhaseSO> phaseSoList;
    }
}