using Microsoft.AspNetCore.Connections;
using WebApplicationTarget;

var builder = WebApplication.CreateSlimBuilder(args);

builder.WebHost.UseKestrel(options =>
{
    options.ListenAnyIP(27727, listenOptions =>
    {
        listenOptions.UseConnectionHandler<TelnetConnectionHandler>();
    });
});

var app = builder.Build();

app.Run();
