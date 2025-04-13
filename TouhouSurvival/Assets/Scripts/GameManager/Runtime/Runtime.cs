namespace Unchord
{
    public abstract class Runtime<T_Data> : IRuntime<T_Data>
    {
        public T_Data RuntimeData => _runtimeData;

        object IRuntime.RuntimeData => _runtimeData;

        private T_Data _runtimeData;

        public Runtime(T_Data data)
        {
            _runtimeData = data;
        }
    }
}