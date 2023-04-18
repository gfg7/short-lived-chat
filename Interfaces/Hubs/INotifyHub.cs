using SimpleLiveChat.Interfaces.Entities;

namespace SimpleLiveChat.Interfaces.Hubs
{
    public interface INotifyHub : IHub
    {
        Task Notify(ILocalEvent @event);
    }
}