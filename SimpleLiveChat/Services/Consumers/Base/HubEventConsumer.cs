using SimpleLiveChat.Interfaces.Entities;
using SimpleLiveChat.Interfaces.PublisherSubscriber;
using SimpleLiveChat.Models.CustomExceptions;
using SimpleLiveChat.Services.Hubs;

namespace SimpleLiveChat.Services.Consumers.Base
{
    public abstract class HubEventConsumer : BaseEventConsumer<IExternalHubEvent>
    {
        protected readonly HubContextWrapper _hubContextWrapper;
        public HubEventConsumer(ISubscriberProvider provider, ILogger<HubEventConsumer> logger, HubContextWrapper hubContextWrapper) : base(provider, logger)
        {
            _hubContextWrapper = hubContextWrapper;
        }

        public override async Task Consume(string value, IExternalHubEvent @event)
        {
            if (IsExternalNodeEvent(@event))
            {
                await Task.FromCanceled(Task.Factory.CancellationToken);
            }

            await Task.CompletedTask;
        }
    }
}