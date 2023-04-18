using System.Text.Json.Serialization;
using SimpleLiveChat.Interfaces.Entities;
using SimpleLiveChat.Models.Entity;

namespace SimpleLiveChat.Models
{
    public record Event : ServerEvent, IExternalHubEvent, IHubEvent, IServerEvent, ILocalEvent
    {
        public string? ChatId { get; set; }
        public string? Invoker { get; set; }
        public object? Payload {get;set;}
    }

    public record ServerEvent : IServerEvent, ILocalEvent
    {
        public string? Node { get; set; }
        public DateTime Time { get; set; } = DateTime.UtcNow;
        public EventType EventType { get; set; } = EventType.SystemNotif;
    }
}