using SimpleLiveChat.Interfaces.PublisherSubscrib;
using SimpleLiveChat.Interfaces.PublisherSubscriber;
using SimpleLiveChat.Services.Consumers;
using SimpleLiveChat.Services.Consumers.Base;
using SimpleLiveChat.Services.Hubs;

namespace SimpleLiveChat.Services.Configuration
{
    public static class ConsumerServices
    {
        public static IServiceCollection RegisterConsumers(this IServiceCollection services)
        {
            services.AddSingleton<SubStateConsumer>();
            services.AddSingleton<ExpiryConsumer>();

            services.AddScoped<EventConsumer>();
            services.AddScoped(typeof(IConsumingState), x => x.GetRequiredService<SubStateConsumer>());

            return services;
        }
    }

    public class ConsumerInitialization
    {
        private async Task Initialize(IServiceProvider provider, Func<Type, bool> predicate)
        {
            var scope = provider.CreateScope();
            var consumers = typeof(IBaseConsumer).Assembly.GetTypes()
                .Where(predicate);

            foreach (var consumer in consumers)
            {
                if (scope.ServiceProvider.GetRequiredService(consumer) is IBaseConsumer service)
                {
                    service.Subscribe();
                }
            }
        }

        public async Task Start(IServiceProvider provider)
        {
            Func<Type, bool> predicate = x => !x.IsAbstract && !x.IsInterface && typeof(IBaseConsumer).IsAssignableFrom(x) && !typeof(IDelayed).IsAssignableFrom(x);
            await Initialize(provider, predicate);
        }
    }
}