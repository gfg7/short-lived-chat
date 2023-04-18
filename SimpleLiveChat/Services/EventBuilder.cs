using SimpleLiveChat.Interfaces.Entities;
using SimpleLiveChat.Models;
using SimpleLiveChat.Models.Entity;

namespace SimpleLiveChat.Services
{
    public static class EventBuilder
    {
        public static T SetType<T>(this T source, EventType type) where T : class, ILocalEvent
        {
            source.EventType = type;
            return source;
        }

        public static T SetChat<T>(this T source, string chatId) where T : class, IHubEvent
        {
            source.ChatId = chatId;
            return source;
        }

        public static T SetInvoker<T>(this T source, string username) where T : class, IHubEvent
        {
            source.Invoker = username;
            return source;
        }

        public static T SetNode<T>(this T source, string clientName) where T : class, IServerEvent
        {
            source.Node = clientName;
            return source;
        }

        public static T SetPayload<T>(this T source, object? obj) where T : class, IHubEvent
        {
            source.Payload = obj;
            return source;
        }

        public static T SetTime<T>(this T source, DateTime time) where T : class, ILocalEvent
        {
            source.Time = time;
            return source;
        }
    }
}