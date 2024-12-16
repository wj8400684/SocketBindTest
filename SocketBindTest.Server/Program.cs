using KestrelFramework;
using SocketBindTest.Server;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddSingleton<ForwardOptionsDb>();
builder.Services.AddOptions<ForwardOptions>();
builder.Services.ConfigureOptions<KestrelOptionsSetup>();
builder.Services.ConfigureOptions<SocketTransportOptionsSetup>();

builder.Services.AddSingleton<InProcConnectionContainerMiddleware>();
builder.Services.AddSingleton<IKestrelMiddleware>(s => s.GetRequiredService<InProcConnectionContainerMiddleware>());
builder.Services.AddSingleton<IConnectionContainer>(s => s.GetRequiredService<InProcConnectionContainerMiddleware>());

var app = builder.Build();

var connection = app.MapGroup("api/connection");

connection.MapGet("count", (IConnectionContainer container) => Results.Ok($"在线客户端数量：{container.GetConnectionCount()}"));

app.Run();