using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;
using WebApplicationTarget.Extensions;

namespace WebApplicationTarget.Setups;

internal sealed class KestrelOptionsSetup(IConfiguration configuration)
    : IConfigureOptions<KestrelServerOptions>
{
    public void Configure(KestrelServerOptions options)
    {
        var section = configuration.GetSection("Kestrel");

        options.Configure(section)
            .Endpoint("Telnet", endpoint => endpoint.ListenOptions
                .UseContainer()
                .UseTelnet());
    }
}