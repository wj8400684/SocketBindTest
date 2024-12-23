using ServiceSelf;
using WebApplicationTarget;
using WebApplicationTarget.Dto;
using WebApplicationTarget.Extensions;
using WebApplicationTarget.Middlewares;
using WebApplicationTarget.Setups;

if (!ServiceHelper.UseServiceSelf(args))
    return;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Host.UseServiceSelf();
builder.Services.AddConnections();
builder.Services.AddConnectionContainer();
builder.Services.AddSingleton<TelnetConnectionHandler>();
builder.Services.AddSingleton<WebSocketConnectionHandler>();
builder.Services.AddHostedService(s => s.GetRequiredService<WebSocketConnectionHandler>());

builder.Services.ConfigureOptions<KestrelOptionsSetup>();
builder.Services.ConfigureOptions<JsonOptionsSetup>();

var app = builder.Build();

app.MapGet("/api/connection/count", (IConnectionContainer container) => new ConnectionResponse(
    Msg: "操作成功",
    RefreshTime: DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
    Count: container.GetConnectionCount()));

app.MapConnectionHandler<TelnetConnectionHandler>("/api/telnet");
app.MapConnectionHandler<WebSocketConnectionHandler>("/api/websocket");

app.Run();