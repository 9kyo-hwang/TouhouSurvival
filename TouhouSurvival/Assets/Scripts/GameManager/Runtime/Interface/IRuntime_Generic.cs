namespace Unchord
{
    public interface IRuntime<T> : IRuntime
    {
        new T RuntimeData { get; }
    }
}