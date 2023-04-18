using System.Diagnostics.SymbolStore;
using System.Security.Cryptography.X509Certificates;
using SimpleLiveChat.Interfaces;

namespace SimpleLiveChat.Models
{
    public record Chat : IEntity
    {
        public Guid Id {get;init;}
        public string Name {get;init;}

        public Chat(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
        }
    }
}