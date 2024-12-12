using System.Net;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;

namespace SocketBindTest.Server;

internal sealed class KestrelOptionsSetup(IConfiguration configuration,ForwardOptionsDb db)
    : IConfigureOptions<KestrelServerOptions>
{
    public void Configure(KestrelServerOptions options)
    {
        var kestrel = configuration.GetSection("Kestrel");
        var kestrelConfigLoad = options.Configure(kestrel);
        var endpoints = kestrel.GetSection("Endpoints").GetChildren();

        foreach (var section in endpoints)
        {
            var name = section.Key;
            var url = section.GetValue<string>("Url");
            var port = section.GetValue<int>("ForwardPort");
            if (port == 0)
                continue;

            var forwardOptions = ForwardOptions.Load(section);
            if (string.IsNullOrEmpty(forwardOptions.ForwardAddress) ||
                !IPAddress.TryParse(forwardOptions.ForwardAddress, out var forwardAddress))
                continue;

            forwardOptions.ForwardIpAddress = forwardAddress;
            forwardOptions.ForwardName = name;
            forwardOptions.ForwardPort = port;
            forwardOptions.ListenUrl = url;

            db.AddOption(forwardOptions);
            
            kestrelConfigLoad.Endpoint(forwardOptions.ForwardName,
                endpoint => endpoint.ListenOptions.UseConnectionHandler<ForwardConnectionHandler>());
        }
    }
}