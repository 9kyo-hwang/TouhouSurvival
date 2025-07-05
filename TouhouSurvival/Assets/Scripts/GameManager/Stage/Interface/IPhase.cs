namespace Unchord
{
    public interface IPhase :
        IInterruptableHalt,
    {
        void Start();
        RuntimeState Update();
        void Pause();
        void Resume();
        void End();
    }
}