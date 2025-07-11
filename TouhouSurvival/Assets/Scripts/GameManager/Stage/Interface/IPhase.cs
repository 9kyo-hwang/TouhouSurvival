namespace Unchord
{
    public interface IPhase :
        IInterruptableHalt,
        IInterruptableResurrect
    {
        void Start();
        RuntimeState Update();
        void Pause();
        void Resume();
        void End();
    }
}