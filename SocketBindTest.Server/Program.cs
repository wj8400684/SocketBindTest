using Microsoft.AspNetCore.Connections;
using SocketBindTest.Server;

var builder = WebApplication.CreateSlimBuilder(args);

builder.WebHost.UseKestrel(options =>
    options.ListenAnyIP(8080, listenOptions => listenOptions.UseConnectionHandler<TelnetConnectionHandler>()));

var app = builder.Build();

app.Run();