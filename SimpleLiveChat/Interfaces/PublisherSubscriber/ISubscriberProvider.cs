using StackExchange.Redis;

namespace SimpleLiveChat.Interfaces.PublisherSubscriber
{
    public interface ISubscriberProvider
    {
        ISubscriber Subscriber {get;}
    }
}