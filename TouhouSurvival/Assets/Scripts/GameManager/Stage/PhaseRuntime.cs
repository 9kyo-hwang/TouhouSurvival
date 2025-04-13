namespace Unchord
{
    public abstract class PhaseRuntime<T_Data> : Runtime<T_Data>, IPhase
    {
        public PhaseRuntime(T_Data data)
        : base(data)
        {

        }

        public virtual void Start() { }
        public abstract RuntimeState Update();
        public virtual void Pause() { }
        public virtual void Resume() { }
        public virtual void End() { }
        public virtual void InterruptHalt() { }
    }
}