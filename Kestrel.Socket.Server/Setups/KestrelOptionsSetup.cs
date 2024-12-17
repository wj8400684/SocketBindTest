using Kestrel.Socket.Server.Extensions;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;

namespace Kestrel.Socket.Server.Setups;

internal sealed class KestrelOptionsSetup(IConfiguration configuration)
    : IConfigureOptions<KestrelServerOptions>
{
    public void Configure(KestrelServerOptions options)
    {
        //TheadPoolEx.ResetThreadPool(65535, 65535, 30000, 30000);
        //ThreadPool.SetMinThreads(10000, 10000);
        //ThreadPool.SetMaxThreads(10000,10000);
        var section = configuration.GetSection("Kestrel");
        options.Configure(section)
            .Endpoint("Telnet", endpoint => endpoint.ListenOptions
                .UseContainer()
                .UseTelnet());
    }
}