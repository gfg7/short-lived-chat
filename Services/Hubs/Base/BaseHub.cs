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
    public abstract class BaseHub<T> : Hub<T> where T: class, IHub
    {
        protected string? GetUsername()
        {
            return this.Context.User?.Identity?.Name;
        }
    }
}