namespace SimpleLiveChat.Models.Entity
{
    public enum EventType
    {
        SystemNotif = 0,
        Message = 1,
        PrivateMessage = 2,
        NewChat = 3,
        StopListening = 4,
        Listening = 5
    }
}