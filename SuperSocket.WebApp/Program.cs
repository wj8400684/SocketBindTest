using SuperSocket.ProtoBase;
using SuperSocket.Server;
using SuperSocket.Server.Abstractions.Session;
using SuperSocket.Server.Host;
using SuperSocket.WebApp;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AsSuperSocketHostBuilder<StringPackageInfo, CommandLinePipelineFilter>()
    .UseInProcSessionContainer()
    .AsMinimalApiHostBuilder()
    .ConfigureHostBuilder();

var app = builder.Build();

app.MapGet("/api/session/count", (ISessionContainer sessionContainer) => new SessionResponse(
    Msg: "操作成功",
    RefreshTime: DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
    Count: sessionContainer.GetSessionCount()));

app.Run();