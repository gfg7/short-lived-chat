using SimpleLiveChat.Interfaces.Entities;
using SimpleLiveChat.Models;

namespace SimpleLiveChat.Interfaces.Hubs
{
    public interface IChatHub
    {
        Task CreateChat(IHubEvent @event);
        Task JoinIn(IHubEvent @event);
        Task Leave(IHubEvent @event);
    }
}