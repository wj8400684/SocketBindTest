using SuperSocket.ProtoBase;
using SuperSocket.Server;
using SuperSocket.Server.Abstractions.Session;
using SuperSocket.Server.Host;
using SuperSocket.WebApp;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AsSuperSocketHostBuilder<TextPackageInfo, LinePipelineFilter>()
    .UseInProcSessionContainer()
    .AsMinimalApiHostBuilder()
    .ConfigureHostBuilder();

var app = builder.Build();

app.MapGet("/api/connection/count", (ISessionContainer sessionContainer) => new ConnectionResponse(
    Msg: "操作成功",
    RefreshTime: DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
    Count: sessionContainer.GetSessionCount()));

app.Run();