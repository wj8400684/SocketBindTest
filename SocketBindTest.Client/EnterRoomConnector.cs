using System.Net;
using SuperSocket.Client;
using SuperSocket.Connection;

namespace SocketBindTest.Client;

public sealed class EnterRoomConnector : ConnectorBase
{
    protected override ValueTask<ConnectState> ConnectAsync(EndPoint remoteEndPoint, ConnectState state, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}