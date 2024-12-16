using System.Collections.Concurrent;
using KestrelFramework;
using Microsoft.AspNetCore.Connections;

namespace SocketBindTest.Server;

public interface IConnectionContainer
{
    bool Add(ConnectionContext connection);
    
    bool Remove(ConnectionContext connection);

    ConnectionContext? GetConnectionById(string connectionId);

    int GetConnectionCount();

    IEnumerable<ConnectionContext> GetConnections(Predicate<ConnectionContext>? criteria = null);
}

internal sealed class InProcConnectionContainerMiddleware(ILogger<InProcConnectionContainerMiddleware> logger)
    : IKestrelMiddleware, IConnectionContainer
{
    private readonly ConcurrentDictionary<string, ConnectionContext> _connections = new();

    public async Task InvokeAsync(ConnectionDelegate next, ConnectionContext context)
    {
        logger.LogInformation($"[{context.RemoteEndPoint}]-[{context.ConnectionId}]：客户端连接");

        Add(context);

        try
        {
            await next(context);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning($"[{context.RemoteEndPoint}]-[{context.ConnectionId}]：任务已经取消");
        }
        catch (Exception e)
        {
            logger.LogError(e, $"[{context.RemoteEndPoint}]-[{context.ConnectionId}]：执行中间件发生错误");
        }
        finally
        {
            Remove(context);
            logger.LogInformation($"[{context.RemoteEndPoint}]-[{context.ConnectionId}]：客户端断开连接");
        }
    }

    public bool Add(ConnectionContext connection)
    {
        return _connections.TryAdd(connection.ConnectionId, connection);
    }

    public bool Remove(ConnectionContext connection)
    {
        return _connections.TryRemove(connection.ConnectionId, out _);
    }

    public ConnectionContext? GetConnectionById(string connectionId)
    {
        _connections.TryGetValue(connectionId, out var connection);
        return connection;
    }

    public int GetConnectionCount()
    {
        return _connections.Count;
    }

    public IEnumerable<ConnectionContext> GetConnections(Predicate<ConnectionContext>? criteria = null)
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