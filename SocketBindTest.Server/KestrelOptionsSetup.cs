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
            var forwardOptions = ForwardOptions.Load(section);
          
            forwardOptions.ForwardName = name;
            forwardOptions.ForwardPort = port;
            forwardOptions.ListenUrl = url;
           
            if (!string.IsNullOrEmpty(forwardOptions.ForwardAddress) &&
                IPAddress.TryParse(forwardOptions.ForwardAddress, out var forwardAddress))
            {
                forwardOptions.ForwardIpAddress = forwardAddress;
                db.AddOption(forwardOptions);
            
                kestrelConfigLoad.Endpoint(forwardOptions.ForwardName,
                    endpoint => endpoint.ListenOptions.UseConnectionHandler<ForwardConnectionHandler>());
            }
            else
            {
                kestrelConfigLoad.Endpoint(forwardOptions.ForwardName,
                    endpoint => endpoint.ListenOptions.UseConnectionHandler<TelnetConnectionHandler>());
            }
        }
    }
}