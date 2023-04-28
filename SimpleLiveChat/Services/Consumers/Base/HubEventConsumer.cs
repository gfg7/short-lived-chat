using SimpleLiveChat.Interfaces.Entities;
using SimpleLiveChat.Interfaces.PublisherSubscriber;
using SimpleLiveChat.Models.CustomExceptions;

namespace SimpleLiveChat.Services.Consumers.Base
{
    public abstract class HubEventConsumer : BaseEventConsumer<IExternalHubEvent>
    {
        public IServiceProvider _serviceProvider;
        public HubEventConsumer(ISubscriberProvider provider,IServiceProvider serviceProvider, ILogger<HubEventConsumer> logger) : base(provider, logger)
        {
            _serviceProvider = serviceProvider;
        }

        public override async Task Consume(string value, IExternalHubEvent @event)
        {
            if (@event.Node == _subscriber.Multiplexer.ClientName)
            {
                await Task.FromCanceled(Task.Factory.CancellationToken);
            }

            await Task.CompletedTask;
        }
    }
}