using SimpleLiveChat.Interfaces.Entities;
using SimpleLiveChat.Interfaces.Hubs;
using SimpleLiveChat.Interfaces.PublisherSubscriber;
using SimpleLiveChat.Models.Entity;
using SimpleLiveChat.Services.Consumers;

namespace SimpleLiveChat.Services.Hubs
{
    public class NotifyHub : BaseHub<INotifyHub>
    {
        private readonly IPublisher _publisher;
        private readonly IConsumingState _consumingState;

        public NotifyHub(IPublisher publisher, IConsumingState consumingState)
        {
            _publisher = publisher;
        }

        public async Task Notify(ILocalEvent @event)
        {
            if (_consumingState.IsConsumingEvent())
            {
                await _publisher.Publish(@event);
            }

            await Clients.All.Notify(@event);
        }
    }
}