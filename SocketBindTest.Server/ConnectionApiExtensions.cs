namespace SocketBindTest.Server;

public static class ConnectionApiExtensions
{
    public static void MapConnectionApi(this WebApplication app)
    {
        var connection = app.MapGroup("api/connection");

        connection.MatGetConnectionCount();
    }

    private static void MatGetConnectionCount(this RouteGroupBuilder group)
    {
        group.MapGet("count", (IConnectionContainer container) => Results.Ok($"在线客户端数量：{container.GetConnectionCount()}"));
    }
}