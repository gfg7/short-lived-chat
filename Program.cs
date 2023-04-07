using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using SimpleLiveChat.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSignalR();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddStackExchangeRedisCache(o =>
{
    o.InstanceName = Environment.GetEnvironmentVariable("REDIS_INSTANCE");
    o.Configuration = Environment.GetEnvironmentVariable("REDIS_HOST") ?? "localhost";
    
});

builder.Services.AddScoped(typeof(Repository<>));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(o=> {
    o.LoginPath="/startup";
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<ChatHub>("/chat");
app.MapHub<MessageHub>("/msg");

app.MapPost("/startup", async (string username, HttpContext context) => {
    
    var claim = new Claim(ClaimsIdentity.DefaultNameClaimType, username);
    var identity = new ClaimsIdentity(new[] {claim}, "Cookies");
    var principal = new ClaimsPrincipal(identity);
    
    await context.SignInAsync(principal);
});

app.Run();
