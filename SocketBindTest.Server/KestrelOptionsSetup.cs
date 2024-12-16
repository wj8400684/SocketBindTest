using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;
using SocketBindTest.Server.ConnectionHandlers;

namespace SocketBindTest.Server;

internal sealed class KestrelOptionsSetup(
    IConfiguration configuration,
    ForwardOptionsDb db,
    ILogger<KestrelOptionsSetup> logger)
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
            if (name.Equals("http"))
                continue;

            var url = section.GetValue<string>("Url");
            var port = section.GetValue<int>("ForwardPort");
            var forwardOptions = ForwardOptions.Load(section);

            forwardOptions.ForwardName = name;
            forwardOptions.ForwardPort = port;
            forwardOptions.ListenUrl = url;

            if (forwardOptions.IsForward)
            {
                ArgumentNullException.ThrowIfNull(forwardOptions.ForwardAddress);

                if (IPAddress.TryParse(forwardOptions.ForwardAddress, out var forwardAddress))
                    forwardOptions.ForwardEndPoint = new IPEndPoint(forwardAddress, forwardOptions.ForwardPort);
                else
                    forwardOptions.ForwardEndPoint = new DnsEndPoint(forwardOptions.ForwardAddress,
                        forwardOptions.ForwardPort, AddressFamily.InterNetwork);

                db.AddOption(forwardOptions);

                kestrelConfigLoad.Endpoint(forwardOptions.ForwardName,
                    endpoint =>
                    {
                        endpoint.ListenOptions.Use(endpoint.ListenOptions.ApplicationServices
                            .GetRequiredService<InProcConnectionContainerMiddleware>());
                        endpoint.ListenOptions.UseTlsDetection();
                        endpoint.ListenOptions.UseConnectionHandler<ForwardConnectionHandler>();
                    });
                logger.LogInformation(
                    $"开始监听{forwardOptions.ListenUrl}=>{forwardOptions.ForwardPort}-ForwardConnectionHandler");
            }
            else
            {
                kestrelConfigLoad.Endpoint(forwardOptions.ForwardName,
                    endpoint =>
                    {
                        endpoint.ListenOptions.Use(endpoint.ListenOptions.ApplicationServices
                            .GetRequiredService<InProcConnectionContainerMiddleware>());
                        endpoint.ListenOptions.UseConnectionHandler<TelnetConnectionHandler>();
                    });
                logger.LogInformation($"开始监听{forwardOptions.ListenUrl}=>TelnetConnectionHandler");
            }
        }
    }
}