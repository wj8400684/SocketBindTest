using Microsoft.AspNetCore.Connections;

namespace WebApplicationTarget.Middlewares;

public interface IConnectionContainer
{
    bool Add(ConnectionContext connection);
    
    bool Remove(ConnectionContext connection);

    ConnectionContext? GetConnectionById(string connectionId);

    int GetConnectionCount();

    IEnumerable<ConnectionContext> GetConnections(Predicate<ConnectionContext>? criteria = null);
}