using Kestrel.Socket.Server.Chat;
using Kestrel.Socket.Server.Extensions;
using Kestrel.Socket.Server.Setups;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddSignalR()
    .AddJsonProtocol()
    .AddContainer();

builder.Services.ConfigureOptions<KestrelOptionsSetup>();
builder.Services.ConfigureOptions<JsonOptionsSetup>();
builder.Services.ConfigureOptions<SocketTransportOptionsSetup>();

builder.Services.AddConnectionContainer();

var app = builder.Build();

app.MapConnectionApi();
app.MapChatWebApi();

app.MapHub<ChatHub>("/api/chat");

app.Run();