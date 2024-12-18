using SuperSocket.ProtoBase;
using SuperSocket.Server;
using SuperSocket.Server.Abstractions;
using SuperSocket.Server.Abstractions.Session;
using SuperSocket.Server.Host;
using SuperSocket.WebApp;
using SuperSocket.WebApp.Dto;
using SuperSocket.WebApp.Server;
using SuperSocket.WebApp.Setups;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureOptions<JsonOptionsSetup>();

// builder.Host.AsSuperSocketHostBuilder<TextPackageInfo, LinePipelineFilter>()
//     .ConfigureSuperSocket(server =>
//     {
//         server.AddListener(new ListenOptions
//         {
//             BackLog = 512,
//             Ip = "Any",
//             Port = 9000
//         });
//     })
//     .UseInProcSessionContainer()
//     .AsMinimalApiHostBuilder()
//     .ConfigureHostBuilder();

builder.Host.AsMultipleServerHostBuilder()
    .AddServer<ChatServer, TextPackageInfo, LinePipelineFilter>(c =>
    {
        c.ConfigureSuperSocket(server =>
        {
            server.AddListener(new ListenOptions
            {
                BackLog = 512,
                Ip = "Any",
                Port = 9000
            });
        });
    })
    .AddServer<GameServer, TextPackageInfo, LinePipelineFilter>(c =>
    {
        c.ConfigureSuperSocket(server =>
        {
            server.AddListener(new ListenOptions
            {
                BackLog = 512,
                Ip = "Any",
                Port = 9001
            });
        });
    })
    .AsMinimalApiHostBuilder()
    .ConfigureHostBuilder();


var app = builder.Build();

app.MapGet("/api/connection/count", (ISessionContainer sessionContainer) => new ConnectionResponse(
    Msg: "操作成功",
    RefreshTime: DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
    Count: sessionContainer.GetSessionCount()));

app.Run();