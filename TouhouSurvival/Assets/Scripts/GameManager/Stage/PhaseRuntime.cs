namespace Unchord
{
    public abstract class PhaseRuntime<T_Data> : Runtime<T_Data>, IPhase
    {
        protected PhaseRuntimeCommons CommonData { get; private set; }

        public PhaseRuntime(T_Data data, PhaseRuntimeCommons commonData)
        : base(data)
        {
            CommonData = commonData;
        }

        public virtual void Start() { }
        public abstract RuntimeState Update();
        public virtual void Pause() { }
        public virtual void Resume() { }
        public virtual void End() { }
        public virtual void InterruptHalt() { }
        public virtual void InterruptResurrect() { }
    }
}