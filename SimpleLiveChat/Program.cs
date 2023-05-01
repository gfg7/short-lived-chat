using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.SignalR;
using SimpleLiveChat.Interfaces.PublisherSubscriber;
using SimpleLiveChat.Interfaces.Repository;
using SimpleLiveChat.Services.Configuration;
using SimpleLiveChat.Services.Hubs;
using SimpleLiveChat.Services.Hubs.Chats;
using SimpleLiveChat.Services.Publisher;
using SimpleLiveChat.Services.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHostedService<ServerStateHandler>();

builder.Services.AddLogging(o =>
{
    o.AddConsole(c =>
    {
        c.TimestampFormat = "[yyyy-MM-dd HH:mm:ss]";
    });
});
builder.Services.AddSingleton<IDatabaseProvider, RedisConnection>();
builder.Services.AddSingleton<ISubscriberProvider>(x => (RedisConnection)x.GetRequiredService<IDatabaseProvider>());

builder.Services.RegisterConsumers();
builder.Services.AddScoped(typeof(IPublisher<>), typeof(EventPublisher<>));

builder.Services.AddScoped(typeof(IStringKeyRepository<>), typeof(RedisRepository<>));
builder.Services.AddScoped(typeof(ITempStore<>), x=> x.GetRequiredService(typeof(IStringKeyRepository<>)));

builder.Services.AddScoped<HubContextWrapper>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(o =>
{
    o.LoginPath = "/startup";
});
builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var initializer = new ConsumerInitialization();
    await initializer.Start(app.Services);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health", new()
{
    AllowCachingResponses = false
});

app.MapHub<ChatHub>("/chat");
app.MapHub<MessageHub>("/msg");
app.MapHub<NotifyHub>("/notify");

app.MapPost("/startup", async (string username, HttpContext context) =>
{

    var claim = new Claim(ClaimsIdentity.DefaultNameClaimType, username);
    var identity = new ClaimsIdentity(new[] { claim }, "Cookies");
    var principal = new ClaimsPrincipal(identity);

    await context.SignInAsync(principal);
});

app.MapDelete("/leave", async (HttpContext context) =>
{
    await context.SignOutAsync();
});

app.Run();
