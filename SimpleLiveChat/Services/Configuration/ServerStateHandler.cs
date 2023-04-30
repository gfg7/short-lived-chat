using System.Reflection.Metadata;
using SimpleLiveChat.Interfaces.Entities;
using SimpleLiveChat.Interfaces.PublisherSubscriber;
using SimpleLiveChat.Models;
using SimpleLiveChat.Models.Entity;

namespace SimpleLiveChat.Services.Configuration
{
    public class ServerStateHandler : IHostedService
    {
        private readonly IHostApplicationLifetime _hostApplication;
        private readonly IPublisher<ServerEvent> _publisher;
        public ServerStateHandler(IHostApplicationLifetime hostApplication, IServiceProvider serviceProvider)
        {
            _hostApplication = hostApplication;
            using (var scope = serviceProvider.CreateScope())
            {
                _publisher = scope.ServiceProvider.GetRequiredService<IPublisher<ServerEvent>>();
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _hostApplication.ApplicationStarted.Register(async () => await PushStartUpEvent());

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _hostApplication.ApplicationStopping.Register(async () => await PushShutDownEvent());

            return Task.CompletedTask;
        }

        private async Task PushStartUpEvent()
        {
            var @event = new ServerEvent().SetType<ServerEvent>(EventType.Listening);
            await _publisher.Publish(@event, "SubState");
        }

        private async Task PushShutDownEvent()
        {
            var @event = new ServerEvent().SetType<ServerEvent>(EventType.StopListening);
            await _publisher.Publish(@event, "SubState");
        }
    }
}