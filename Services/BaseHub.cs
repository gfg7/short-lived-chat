using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.SignalR;

namespace SimpleLiveChat.Services
{
    abstract class BaseHub : Hub
    {
        protected string GetUsername() {
            return this.Context.User.Identity.Name;
        }
    }
}