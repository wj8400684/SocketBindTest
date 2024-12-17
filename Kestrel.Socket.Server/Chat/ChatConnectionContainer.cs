using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;

namespace Kestrel.Socket.Server.Chat;

internal sealed class ChatConnectionContainer : IChatConnectionContainer
{
    private readonly ConcurrentDictionary<string, HubCallerContext> _connections = new();

    public bool Add(HubCallerContext connection)
    {
        return _connections.TryAdd(connection.ConnectionId, connection);
    }

    public bool Remove(HubCallerContext connection)
    {
        return _connections.TryRemove(connection.ConnectionId, out _);
    }

    public HubCallerContext? GetConnectionById(string connectionId)
    {
        _connections.TryGetValue(connectionId, out var connection);
        return connection;
    }

    public int GetConnectionCount()
    {
        return _connections.Count;
    }

    public IEnumerable<HubCallerContext> GetConnections(Predicate<HubCallerContext>? criteria = null)
    {
        using var enumerator = _connections.GetEnumerator();

        while (enumerator.MoveNext())
        {
            var s = enumerator.Current.Value;

            if (criteria == null || criteria(s))
                yield return s;
        }
    }
}