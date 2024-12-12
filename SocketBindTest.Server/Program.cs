using SocketBindTest.Server;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddOptions<ForwardOptions>();
builder.Services.ConfigureOptions<KestrelOptionsSetup>();

var app = builder.Build();

app.Run();