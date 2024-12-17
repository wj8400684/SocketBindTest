using Kestrel.Socket.Server;
using Kestrel.Socket.Server.Extensions;
using Kestrel.Socket.Server.Setups;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureOptions<KestrelOptionsSetup>();
builder.Services.ConfigureOptions<JsonOptionsSetup>();
builder.Services.ConfigureOptions<SocketTransportOptionsSetup>();

builder.Services.AddConnectionContainer();

var app = builder.Build();

app.MapConnectionApi();

app.Run();
