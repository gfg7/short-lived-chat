using Microsoft.AspNetCore.SignalR;

namespace SimpleLiveChat.Services.Hubs
{
    public class HubContextWrapper
    {
        private IServiceProvider _provider;
        public HubContextWrapper(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IHubContext<I, C> Get<I, C>()
        where I : Hub<C>
        where C : class
        {
            IHubContext<I, C> service;
            using (var scope = _provider.CreateScope())
            {
                service = _provider.GetRequiredService<IHubContext<I, C>>();
            }

            return service;
        }
    }
}