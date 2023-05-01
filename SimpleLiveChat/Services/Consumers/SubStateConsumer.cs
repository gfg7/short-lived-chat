using System.Collections.Concurrent;
using System.Text.Json;
using SimpleLiveChat.Interfaces.Entities;
using SimpleLiveChat.Interfaces.PublisherSubscriber;
using SimpleLiveChat.Models;
using SimpleLiveChat.Models.Entity;
using SimpleLiveChat.Services.Consumers.Base;
using StackExchange.Redis;

namespace SimpleLiveChat.Services.Consumers
{
    public class SubStateConsumer : BaseEventConsumer<IServerEvent>, IConsumingState
    {
        private ConcurrentBag<bool> _isListening = new();
        private CancellationTokenSource _eventSubscriptionCancellation = new();
        private Task _stopEventProcessing;
        private EventConsumer _consumer;

        public SubStateConsumer(ISubscriberProvider provider, ILogger<SubStateConsumer> logger, EventConsumer consumer) : base(provider, logger)
        {
            _consumer = consumer;
            _stopEventProcessing = StopListeningEvents(_eventSubscriptionCancellation.Token);
        }

        private async Task StopListeningEvents(CancellationToken token)
        {
            await Task.Delay(TimeSpan.FromSeconds(int.Parse(Environment.GetEnvironmentVariable("SUB_ECHO_TIMEOUT") ?? "60")));

            if (!token.IsCancellationRequested)
            {
                _logger.LogInformation($"Event broadcast disposing is starting");
                _isListening.Clear();
                _consumer.Dispose();
                return;
            }

            _logger.LogInformation($"Dispose of event broadcast is cancelled");
        }

        public override string Channel => "SubState";

        public override Action<RedisChannel, RedisValue> ConsumeEvent => (channel, value) =>
            {
                var @event = JsonSerializer.Deserialize<ServerEvent>(value);

                if (!IsExternalNodeEvent(@event))
                {
                    Consume(channel, @event).GetAwaiter().GetResult();
                }
            };

        public async override Task Consume(string channel, IServerEvent @event)
        {
            _logger.LogInformation($"Msg consumed on {Channel} channel", @event);

            if (@event.EventType == EventType.StopListening)
            {
                _logger.LogInformation($"Subscriber {@event.Node} is stopping");

                if (IsConsumingEvent() && (_stopEventProcessing.IsCompleted || _stopEventProcessing.Status == TaskStatus.WaitingForActivation))
                {
                    await _stopEventProcessing;
                }

                return;
            }

            if (@event.EventType == EventType.Listening)
            {
                if (!_stopEventProcessing.IsCompleted)
                {
                    _logger.LogInformation($"Subscriber {@event.Node} is still listening");
                    _eventSubscriptionCancellation.Cancel();
                    return;
                }

                if (!IsConsumingEvent())
                {
                    _logger.LogInformation($"Subscribers found, start up event broadcast");
                    _isListening.Add(true);
                    _consumer.Subscribe();
                }
            }
        }

        public bool IsConsumingEvent() => _isListening.Count > 0;
    }
}