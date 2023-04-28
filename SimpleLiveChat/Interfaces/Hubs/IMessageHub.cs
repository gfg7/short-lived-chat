namespace SimpleLiveChat.Interfaces.Hubs
{
    public interface IMessageHub
    {
        Task Send(string chatId, string username, string message);
        Task SendPrivate(string chatId, string username, string message);
    }
}