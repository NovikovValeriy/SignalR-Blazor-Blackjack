using Blackjack.Components;
using Blackjack.Hubs;
using Blackjack.Services.Game;
using Blackjack.Services.Room;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();


builder.Services.AddSignalR();
builder.Services.AddTransient<IRoomService, RoomService>();
builder.Services.AddTransient<IGameService, GameService>();
builder.Services.AddMemoryCache();
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
else
{
    app.UseWebAssemblyDebugging();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapHub<GameHub>("/gameHub");

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Blackjack.Client._Imports).Assembly);

app.Run();
