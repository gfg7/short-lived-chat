using SimpleLiveChat.Interfaces.Entities;

namespace SimpleLiveChat.Interfaces.Hubs
{
    public interface INotifyHub 
    {
        Task Notify(ILocalEvent @event);
    }
}