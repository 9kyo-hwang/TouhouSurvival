using UnityEngine;

namespace Unchord
{
    public abstract class RuntimeDataSO : ScriptableObject
    {
        public abstract IRuntime CreateRuntime();
    }
}