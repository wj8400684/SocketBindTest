using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.Options;

namespace SocketBindTest.Server;

internal sealed class SocketTransportOptionsSetup : IConfigureOptions<SocketTransportOptions>
{
    public void Configure(SocketTransportOptions options)
    {
        options.Backlog = 65535;
    }
}