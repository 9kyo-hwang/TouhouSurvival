namespace Unchord
{
    public interface IRuntime<T> : IRuntime
    {
        T RuntimeData { get; }
    }
}