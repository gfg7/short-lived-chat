using SimpleLiveChat.Models.Entity;

namespace SimpleLiveChat.Interfaces.Entities
{
    public interface IExternalHubEvent : ILocalEvent, IHubEvent, IServerEvent { }
    public interface ILocalEvent
    {
        EventType EventType { get; set; }
        DateTime Time { get; set; }
    }
    public interface IServerEvent : ILocalEvent
    {
        string? Node { get; set; }
    }

    public interface IHubEvent : ILocalEvent
    {
        string ChatId { get; set; }
        object? Payload { get; set; }
        string? Invoker { get; set; }
    }
}