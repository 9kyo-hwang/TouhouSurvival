namespace Unchord
{
    public interface IPhase
    {
        void Start();
        RuntimeState Update();
        void Pause();
        void Resume();
        void End();
        void InterruptHalt();
    }
}