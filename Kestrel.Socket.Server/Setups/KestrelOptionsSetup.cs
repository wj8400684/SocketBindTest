using Kestrel.Socket.Server.Extensions;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;

namespace Kestrel.Socket.Server.Setups;

internal sealed class KestrelOptionsSetup(IConfiguration configuration)
    : IConfigureOptions<KestrelServerOptions>
{
    public void Configure(KestrelServerOptions options)
    {
        //TheadPoolEx.ResetThreadPool(100000, 100000, 555, 555);
        var section = configuration.GetSection("Kestrel");
        options.Configure(section)
            .Endpoint("Telnet", endpoint => endpoint.ListenOptions
                .UseContainer()
                .UseTelnet());
    }
}