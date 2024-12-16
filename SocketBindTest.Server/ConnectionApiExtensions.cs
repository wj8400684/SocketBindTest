using SocketBindTest.Server.Dto;

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
        group.MapGet("count", (IConnectionContainer container) => new ConnectionDto
        {
            Msg = "操作成功",
            Count = container.GetConnectionCount(),
        });
    }
}