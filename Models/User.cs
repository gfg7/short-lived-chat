using System.IO;
using System.CodeDom.Compiler;
namespace SimpleLiveChat.Models
{
    struct User
    {
        public readonly string Username{get;init;}
        public List<Chat> Chats {get;set;}

        public User(string username)
        {
            Username = username;
        }
    }
}