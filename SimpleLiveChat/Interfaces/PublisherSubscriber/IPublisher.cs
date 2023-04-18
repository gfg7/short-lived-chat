using SimpleLiveChat.Interfaces.Entities;

namespace SimpleLiveChat.Interfaces.PublisherSubscriber
{
    public interface IPublisher<T> where T: notnull {
        Task Publish(T @event, string channel=null);
    }

    public interface IPublisher : IPublisher<ILocalEvent> { }

}