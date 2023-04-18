using SimpleLiveChat.Interfaces.Entities;

namespace SimpleLiveChat.Interfaces.PublisherSubscrib
{
    public interface IBaseConsumer
    {
        void Subscribe();
    }

    public interface IConsumer<T> : IBaseConsumer, IDisposable where T : notnull
    {
        Task Consume(string channel, T @event);
    }

    public interface IDelayed { }
}