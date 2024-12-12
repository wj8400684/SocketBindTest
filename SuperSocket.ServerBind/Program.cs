using SuperSocket.ProtoBase;
using SuperSocket.Server.Host;

var host = SuperSocketHostBuilder.Create<StringPackageInfo, CommandLinePipelineFilter>()
    .UsePackageHandler(async (s, p) =>
    {
        // handle packages
    })
    .ConfigureLogging((hostCtx, loggingBuilder) =>
    {
        loggingBuilder.AddConsole();
    })
    .Build();

await host.RunAsync();