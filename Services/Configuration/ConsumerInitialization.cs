using SimpleLiveChat.Interfaces.PublisherSubscrib;
using SimpleLiveChat.Services.Consumers;
using SimpleLiveChat.Services.Consumers.Base;
using SimpleLiveChat.Services.Hubs;

namespace SimpleLiveChat.Services.Configuration
{
    public static class ConsumerServices
    {
        public static IServiceCollection RegisterConsumers(this IServiceCollection services)
        {
            services.AddScoped<SubStateConsumer>();
            services.AddScoped<EventConsumer>();
            services.AddScoped<ExpiryConsumer>();

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
                var service = scope.ServiceProvider.GetRequiredService(consumer) as IBaseConsumer;
                service.Subscribe();
            }
        }

        public async Task Start(IServiceProvider provider)
        {
            Func<Type, bool> predicate = x => !x.IsAbstract && !x.IsInterface && typeof(IBaseConsumer).IsAssignableFrom(x) && !typeof(IDelayed<>).IsAssignableFrom(x);
            await Initialize(provider, predicate);
        }
    }
}