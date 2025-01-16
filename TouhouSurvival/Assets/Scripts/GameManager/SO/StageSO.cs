using System.Collections.Generic;
using UnityEngine;

namespace Unchord
{
    [CreateAssetMenu(fileName = "New Stage", menuName = "ScriptableObjects/Game Management/Stage", order = (int)GameManagerAssetMenuOrder.StageSO)]
    public class StageSO : PhaseCompositeSO
    {
        public MapSO mapSO;
    }
}