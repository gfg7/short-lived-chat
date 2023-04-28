using System.Security.Claims;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.SignalR;
using SimpleLiveChat.Models;
using SimpleLiveChat.Models.Entity;
using SimpleLiveChat.Interfaces.PublisherSubscriber;
using SimpleLiveChat.Interfaces.Hubs;

namespace SimpleLiveChat.Services.Hubs
{
    public abstract class BaseHub<T> : Hub<T> where T : class
    {
        protected string? GetUsername()
        {
            return this.Context.User?.Identity?.Name;
        }

        public override Task OnConnectedAsync()
        {
            this.Context.User.Claims.Append(new("Host", "test"));
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var user = this.Context.User.Identity as ClaimsIdentity;
            var host = user.Claims.FirstOrDefault(x=>x.ValueType == "Host");
            user.TryRemoveClaim(host);
            return base.OnDisconnectedAsync(exception);
        }
    }
}