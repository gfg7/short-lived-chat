namespace SimpleLiveChat.Models
{
    struct Chat
    {
        public readonly Guid Id {get;init;}
        public readonly string Name {get;init;}

        public Chat(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
        }
    }
}