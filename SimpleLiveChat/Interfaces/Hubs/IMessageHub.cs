namespace SimpleLiveChat.Interfaces.Hubs
{
    public interface IMessageHub:IHub
    {
        Task Send(string chatId, string username, string message);
        Task SendPrivate(string chatId, string username, string message);
    }
}