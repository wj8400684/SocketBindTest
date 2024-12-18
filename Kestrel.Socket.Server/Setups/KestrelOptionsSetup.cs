using Kestrel.Socket.Server.Extensions;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;

namespace Kestrel.Socket.Server.Setups;

internal sealed class KestrelOptionsSetup(IConfiguration configuration)
    : IConfigureOptions<KestrelServerOptions>
{
    public void Configure(KestrelServerOptions options)
    {
        var section = configuration.GetSection("Kestrel");
        
        options.Configure(section)
            .Endpoint("Telnet", endpoint => endpoint.ListenOptions
                .UseContainer()
                .UseTelnet())
            .Endpoint("Forward", endpoint => endpoint.ListenOptions
                .UseForward())
            .Endpoint("ForwardSSL", endpoint => endpoint.ListenOptions
                .UseTlsDetection()
                .UseForward());
    }
}