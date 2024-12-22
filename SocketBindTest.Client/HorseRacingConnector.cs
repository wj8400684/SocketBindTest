using System.Net;
using SuperSocket.Client;

namespace SocketBindTest.Client;

public sealed class HorseRacingConnector : ConnectorBase
{
    protected override ValueTask<ConnectState> ConnectAsync(EndPoint remoteEndPoint, ConnectState state,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}