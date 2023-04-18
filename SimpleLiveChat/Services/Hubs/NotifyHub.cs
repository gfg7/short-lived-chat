using SimpleLiveChat.Interfaces.Entities;
using SimpleLiveChat.Interfaces.Hubs;
using SimpleLiveChat.Interfaces.PublisherSubscriber;
using SimpleLiveChat.Models.Entity;
using SimpleLiveChat.Services.Consumers;

namespace SimpleLiveChat.Services.Hubs
{
    public class NotifyHub : BaseHub<INotifyHub>
    {
        private readonly IPublisher? _publisher;

        public NotifyHub(IPublisher publisher, IServiceProvider serviceProvider)
        {
            if (serviceProvider.GetService<EventConsumer>() is not null)
            {
                _publisher = publisher;
            }
        }

        public async Task Notify(ILocalEvent @event)
        {
            await _publisher?.Publish(@event);
            await Clients.All.Notify(@event);
        }
    }
}