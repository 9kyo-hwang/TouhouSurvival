using UnityEngine;

namespace Unchord
{
    public abstract class PhaseDataSO : RuntimeDataSO
    {
        public abstract IRuntime CreateRuntime(PhaseRuntimeCommons commonData);
    }
}