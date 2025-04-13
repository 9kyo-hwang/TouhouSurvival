using System.Collections.Generic;
using UnityEngine;

namespace Unchord
{
    [CreateAssetMenu(fileName = "New Stage", menuName = "Scriptable Objects/Game Management/Stage", order = (int)GameManagerAssetMenuOrder.StageSO)]
    public class StageDataSO : RuntimeDataSO
    {
        public MapSO mapSO;
        public List<PhaseDataSO> phaseList;

        public override IRuntime CreateRuntime() => new StageRuntime(this);
    }
}