using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.Options;

namespace Kestrel.Socket.Server.Setups;

internal sealed class SocketTransportOptionsSetup(IConfiguration configuration) : IConfigureOptions<SocketTransportOptions>
{
    public void Configure(SocketTransportOptions options)
    {
        configuration.GetSection("SocketTransportOptions").Bind(options);
    }
}