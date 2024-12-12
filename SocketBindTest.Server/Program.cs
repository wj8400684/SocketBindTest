using SocketBindTest.Server;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ForwardOptionsDb>();
builder.Services.AddOptions<ForwardOptions>();
builder.Services.ConfigureOptions<KestrelOptionsSetup>();
builder.Services.ConfigureOptions<SocketTransportOptionsSetup>();

var app = builder.Build();

app.Run();